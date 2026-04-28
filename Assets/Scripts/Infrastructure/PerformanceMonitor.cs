using System.Collections.Generic;
using UnityEngine;
using PocketDungeons.Analytics;

namespace PocketDungeons.Infrastructure
{
    /// <summary>
    /// Monitors runtime performance. Tracks FPS, memory, battery.
    /// Reports drops below 60 FPS target. Adaptive quality system.
    /// Targets: 60 FPS on iPhone 12+, <300MB RAM, <150MB build.
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        public static PerformanceMonitor Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private float _sampleInterval = 1f;
        [SerializeField] private int _targetFPS = 60;
        [SerializeField] private float _fpsDropThreshold = 45f;
        [SerializeField] private int _maxFrameSamples = 60;

        private readonly Queue<float> _frameTimes = new();
        private float _sampleTimer;
        private float _currentFPS;
        private float _averageFPS;
        private int _fpsDropCount;

        public float CurrentFPS => _currentFPS;
        public float AverageFPS => _averageFPS;
        public int FPSDropCount => _fpsDropCount;

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = _targetFPS;
        }

        private void Update()
        {
            float frameTime = Time.unscaledDeltaTime;
            _frameTimes.Enqueue(frameTime);

            while (_frameTimes.Count > _maxFrameSamples)
                _frameTimes.Dequeue();

            _currentFPS = 1f / frameTime;

            _sampleTimer += frameTime;
            if (_sampleTimer >= _sampleInterval)
            {
                _sampleTimer = 0f;
                CalculateAverageFPS();

                if (_averageFPS < _fpsDropThreshold)
                {
                    _fpsDropCount++;
                    OnFPSDrop();
                }
            }
        }

        private void CalculateAverageFPS()
        {
            if (_frameTimes.Count == 0) return;

            float total = 0f;
            foreach (float t in _frameTimes)
                total += t;

            _averageFPS = _frameTimes.Count / total;
        }

        private void OnFPSDrop()
        {
            Debug.LogWarning($"[Perf] FPS drop: {_averageFPS:F1} (target: {_targetFPS})");

            AnalyticsTracker.Instance?.TrackFPSDrop(_averageFPS,
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            // Adaptive quality: reduce particle count, disable shadows, etc.
            if (_averageFPS < 30f)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.antiAliasing = 0;
            }
            else if (_averageFPS < 45f)
            {
                QualitySettings.antiAliasing = 0;
            }
        }

        public PerformanceReport GetReport()
        {
            return new PerformanceReport
            {
                AverageFPS = _averageFPS,
                FPSDropCount = _fpsDropCount,
                SystemMemoryMB = SystemInfo.systemMemorySize,
                GraphicsMemoryMB = SystemInfo.graphicsMemorySize,
                DeviceModel = SystemInfo.deviceModel,
                OperatingSystem = SystemInfo.operatingSystem
            };
        }
    }

    public struct PerformanceReport
    {
        public float AverageFPS;
        public int FPSDropCount;
        public int SystemMemoryMB;
        public int GraphicsMemoryMB;
        public string DeviceModel;
        public string OperatingSystem;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Launch
{
    /// <summary>
    /// Crash reporting and error tracking. Target: >99.5% crash-free sessions.
    /// Captures unhandled exceptions, logs to analytics, supports breadcrumbs.
    /// </summary>
    public class CrashReporter : MonoBehaviour
    {
        public static CrashReporter Instance { get; private set; }

        private readonly List<string> _breadcrumbs = new();
        private const int MaxBreadcrumbs = 50;

        private int _totalSessions;
        private int _crashedSessions;

        public float CrashFreeRate => _totalSessions > 0
            ? 1f - ((float)_crashedSessions / _totalSessions)
            : 1f;

        private void Awake()
        {
            Instance = this;
            _totalSessions++;
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public void AddBreadcrumb(string message)
        {
            string entry = $"[{DateTime.UtcNow:HH:mm:ss}] {message}";
            _breadcrumbs.Add(entry);

            if (_breadcrumbs.Count > MaxBreadcrumbs)
                _breadcrumbs.RemoveAt(0);
        }

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                _crashedSessions = Mathf.Min(_crashedSessions + 1, _totalSessions);

                var report = new CrashReport
                {
                    Message = condition,
                    StackTrace = stackTrace,
                    LogType = type,
                    Timestamp = DateTime.UtcNow.ToString("O"),
                    Breadcrumbs = new List<string>(_breadcrumbs),
                    DeviceModel = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
                    AppVersion = Application.version
                };

                SendReport(report);
            }
        }

        private void SendReport(CrashReport report)
        {
            Debug.Log($"[CrashReporter] {report.LogType}: {report.Message}");
            // TODO: Send to Firebase Crashlytics or custom backend

            Analytics.AnalyticsTracker.Instance?.TrackFPSDrop(0,
                $"CRASH: {report.Message.Substring(0, Mathf.Min(100, report.Message.Length))}");
        }

        public void LoadState(int totalSessions, int crashedSessions)
        {
            _totalSessions = totalSessions;
            _crashedSessions = crashedSessions;
        }
    }

    [Serializable]
    public struct CrashReport
    {
        public string Message;
        public string StackTrace;
        public LogType LogType;
        public string Timestamp;
        public List<string> Breadcrumbs;
        public string DeviceModel;
        public string OS;
        public string AppVersion;
    }
}

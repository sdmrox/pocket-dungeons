using System;
using UnityEngine;

namespace PocketDungeons.UI.Accessibility
{
    public enum ColorblindMode
    {
        None,
        Protanopia,   // Red-blind
        Deuteranopia, // Green-blind
        Tritanopia    // Blue-blind
    }

    [Serializable]
    public class AccessibilitySettings
    {
        public ColorblindMode ColorblindFilter = ColorblindMode.None;
        public float UIScale = 1.0f;
        public bool VoiceOverEnabled;
        public bool ReducedMotion;
        public bool HighContrast;
        public float TextSize = 1.0f;
        public bool ScreenReaderHints = true;
        public bool HapticFeedback = true;
    }

    public class AccessibilityManager : MonoBehaviour
    {
        public static AccessibilityManager Instance { get; private set; }

        [Header("UI Scale")]
        [SerializeField] private float _minUIScale = 0.75f;
        [SerializeField] private float _maxUIScale = 1.5f;

        [Header("Text")]
        [SerializeField] private float _minTextSize = 0.75f;
        [SerializeField] private float _maxTextSize = 2.0f;

        private AccessibilitySettings _settings = new();

        public AccessibilitySettings Settings => _settings;
        public event Action<AccessibilitySettings> OnSettingsChanged;

        private void Awake()
        {
            Instance = this;
        }

        public void SetColorblindMode(ColorblindMode mode)
        {
            _settings.ColorblindFilter = mode;
            OnSettingsChanged?.Invoke(_settings);
        }

        public void SetUIScale(float scale)
        {
            _settings.UIScale = Mathf.Clamp(scale, _minUIScale, _maxUIScale);
            OnSettingsChanged?.Invoke(_settings);
        }

        public void SetVoiceOverEnabled(bool enabled)
        {
            _settings.VoiceOverEnabled = enabled;
            OnSettingsChanged?.Invoke(_settings);
        }

        public void SetReducedMotion(bool enabled)
        {
            _settings.ReducedMotion = enabled;
            OnSettingsChanged?.Invoke(_settings);
        }

        public void SetHighContrast(bool enabled)
        {
            _settings.HighContrast = enabled;
            OnSettingsChanged?.Invoke(_settings);
        }

        public void SetTextSize(float size)
        {
            _settings.TextSize = Mathf.Clamp(size, _minTextSize, _maxTextSize);
            OnSettingsChanged?.Invoke(_settings);
        }

        public void SetHapticFeedback(bool enabled)
        {
            _settings.HapticFeedback = enabled;
            OnSettingsChanged?.Invoke(_settings);
        }

        public Color ApplyColorblindFilter(Color original)
        {
            return _settings.ColorblindFilter switch
            {
                ColorblindMode.Protanopia => SimulateProtanopia(original),
                ColorblindMode.Deuteranopia => SimulateDeuteranopia(original),
                ColorblindMode.Tritanopia => SimulateTritanopia(original),
                _ => original
            };
        }

        public float GetScreenShakeMultiplier()
        {
            return _settings.ReducedMotion ? 0f : 1f;
        }

        public float GetParticleMultiplier()
        {
            return _settings.ReducedMotion ? 0.25f : 1f;
        }

        public float GetAnimationSpeed()
        {
            return _settings.ReducedMotion ? 2f : 1f;
        }

        public float GetScaledFontSize(float baseFontSize)
        {
            return baseFontSize * _settings.TextSize * _settings.UIScale;
        }

        public bool ShouldUseHighContrastColors() => _settings.HighContrast;

        public void LoadSettings(AccessibilitySettings settings)
        {
            if (settings != null)
                _settings = settings;
            OnSettingsChanged?.Invoke(_settings);
        }

        private static Color SimulateProtanopia(Color c)
        {
            float r = 0.567f * c.r + 0.433f * c.g;
            float g = 0.558f * c.r + 0.442f * c.g;
            float b = 0.242f * c.g + 0.758f * c.b;
            return new Color(r, g, b, c.a);
        }

        private static Color SimulateDeuteranopia(Color c)
        {
            float r = 0.625f * c.r + 0.375f * c.g;
            float g = 0.7f * c.r + 0.3f * c.g;
            float b = 0.3f * c.g + 0.7f * c.b;
            return new Color(r, g, b, c.a);
        }

        private static Color SimulateTritanopia(Color c)
        {
            float r = 0.95f * c.r + 0.05f * c.b;
            float g = 0.433f * c.g + 0.567f * c.b;
            float b = 0.475f * c.g + 0.525f * c.b;
            return new Color(r, g, b, c.a);
        }
    }
}

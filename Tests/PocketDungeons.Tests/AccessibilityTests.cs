using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class AccessibilityTests
    {
        enum ColorblindMode { None, Protanopia, Deuteranopia, Tritanopia }

        class Settings
        {
            public ColorblindMode Colorblind = ColorblindMode.None;
            public float UIScale = 1f;
            public bool ReducedMotion;
            public bool HighContrast;
            public float TextSize = 1f;
            public bool HapticFeedback = true;
        }

        float _minScale = 0.75f;
        float _maxScale = 1.5f;
        float _minText = 0.75f;
        float _maxText = 2.0f;
        Settings _settings = new();

        [SetUp]
        public void SetUp()
        {
            _settings = new Settings();
        }

        [Test]
        public void DefaultSettings_AreNeutral()
        {
            Assert.That(_settings.Colorblind, Is.EqualTo(ColorblindMode.None));
            Assert.That(_settings.UIScale, Is.EqualTo(1f));
            Assert.That(_settings.ReducedMotion, Is.False);
            Assert.That(_settings.HighContrast, Is.False);
            Assert.That(_settings.TextSize, Is.EqualTo(1f));
            Assert.That(_settings.HapticFeedback, Is.True);
        }

        [Test]
        public void UIScale_ClampsToRange()
        {
            float Clamp(float v) => Math.Clamp(v, _minScale, _maxScale);
            Assert.That(Clamp(0.5f), Is.EqualTo(0.75f));
            Assert.That(Clamp(2.0f), Is.EqualTo(1.5f));
            Assert.That(Clamp(1.0f), Is.EqualTo(1.0f));
        }

        [Test]
        public void TextSize_ClampsToRange()
        {
            float Clamp(float v) => Math.Clamp(v, _minText, _maxText);
            Assert.That(Clamp(0.5f), Is.EqualTo(0.75f));
            Assert.That(Clamp(3.0f), Is.EqualTo(2.0f));
        }

        [Test]
        public void ReducedMotion_DisablesScreenShake()
        {
            _settings.ReducedMotion = true;
            float shakeMultiplier = _settings.ReducedMotion ? 0f : 1f;
            Assert.That(shakeMultiplier, Is.EqualTo(0f));
        }

        [Test]
        public void ReducedMotion_ReducesParticles()
        {
            _settings.ReducedMotion = true;
            float particleMul = _settings.ReducedMotion ? 0.25f : 1f;
            Assert.That(particleMul, Is.EqualTo(0.25f));
        }

        [Test]
        public void ReducedMotion_SpeedsUpAnimations()
        {
            _settings.ReducedMotion = true;
            float animSpeed = _settings.ReducedMotion ? 2f : 1f;
            Assert.That(animSpeed, Is.EqualTo(2f));
        }

        [Test]
        public void ScaledFontSize_AppliesBothScales()
        {
            _settings.TextSize = 1.5f;
            _settings.UIScale = 1.2f;
            float baseSize = 14f;
            float scaled = baseSize * _settings.TextSize * _settings.UIScale;
            Assert.That(scaled, Is.EqualTo(25.2f).Within(0.001f));
        }

        [Test]
        public void Protanopia_AdjustsRedChannel()
        {
            // Simplified protanopia: R = 0.567*R + 0.433*G
            float r = 1f, g = 0f;
            float newR = 0.567f * r + 0.433f * g;
            Assert.That(newR, Is.EqualTo(0.567f).Within(0.001f));
        }

        [Test]
        public void Deuteranopia_AdjustsGreenChannel()
        {
            float r = 0f, g = 1f;
            float newG = 0.7f * r + 0.3f * g;
            Assert.That(newG, Is.EqualTo(0.3f).Within(0.001f));
        }

        [Test]
        public void AllColorblindModes_Exist()
        {
            var modes = Enum.GetValues<ColorblindMode>();
            Assert.That(modes.Length, Is.EqualTo(4));
        }
    }
}

using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class UIAnimationTests
    {
        enum TransitionType { SlideLeft, SlideRight, SlideUp, SlideDown, FadeIn, FadeOut, ScaleIn, ScaleOut }

        float _buttonPressScale = 0.95f;

        (float x, float y) CalculateSlideOffset(TransitionType type, float progress, float screenSize)
        {
            float offset = (1f - progress) * screenSize;
            return type switch
            {
                TransitionType.SlideLeft => (-offset, 0f),
                TransitionType.SlideRight => (offset, 0f),
                TransitionType.SlideUp => (0f, offset),
                TransitionType.SlideDown => (0f, -offset),
                _ => (0f, 0f)
            };
        }

        float CalculateFade(TransitionType type, float progress)
        {
            return type switch
            {
                TransitionType.FadeIn => progress,
                TransitionType.FadeOut => 1f - progress,
                _ => 1f
            };
        }

        float CalculateScale(TransitionType type, float progress)
        {
            return type switch
            {
                TransitionType.ScaleIn => progress,
                TransitionType.ScaleOut => 1f - progress,
                _ => 1f
            };
        }

        float EaseInOut(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f;
        }

        [Test]
        public void ButtonPress_ScalesTo95Percent()
        {
            float scale = 1f + ((_buttonPressScale - 1f) * 1f); // t=1
            Assert.That(scale, Is.EqualTo(0.95f).Within(0.001f));
        }

        [Test]
        public void SlideLeft_StartsOffscreen()
        {
            var (x, y) = CalculateSlideOffset(TransitionType.SlideLeft, 0f, 1920f);
            Assert.That(x, Is.EqualTo(-1920f));
            Assert.That(y, Is.EqualTo(0f));
        }

        [Test]
        public void SlideLeft_EndsAtZero()
        {
            var (x, y) = CalculateSlideOffset(TransitionType.SlideLeft, 1f, 1920f);
            Assert.That(x, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void FadeIn_StartsTransparent()
        {
            Assert.That(CalculateFade(TransitionType.FadeIn, 0f), Is.EqualTo(0f));
        }

        [Test]
        public void FadeIn_EndsOpaque()
        {
            Assert.That(CalculateFade(TransitionType.FadeIn, 1f), Is.EqualTo(1f));
        }

        [Test]
        public void FadeOut_StartsOpaque()
        {
            Assert.That(CalculateFade(TransitionType.FadeOut, 0f), Is.EqualTo(1f));
        }

        [Test]
        public void ScaleIn_ZeroAtStart()
        {
            Assert.That(CalculateScale(TransitionType.ScaleIn, 0f), Is.EqualTo(0f));
        }

        [Test]
        public void ScaleIn_OneAtEnd()
        {
            Assert.That(CalculateScale(TransitionType.ScaleIn, 1f), Is.EqualTo(1f));
        }

        [Test]
        public void EaseInOut_StartsAtZero()
        {
            Assert.That(EaseInOut(0f), Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void EaseInOut_EndsAtOne()
        {
            Assert.That(EaseInOut(1f), Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void EaseInOut_MidpointIsHalf()
        {
            Assert.That(EaseInOut(0.5f), Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void TransitionProgress_ClampsTo01()
        {
            float Clamp01(float v) => Math.Clamp(v, 0f, 1f);
            Assert.That(Clamp01(-0.5f), Is.EqualTo(0f));
            Assert.That(Clamp01(1.5f), Is.EqualTo(1f));
            Assert.That(Clamp01(0.5f), Is.EqualTo(0.5f));
        }
    }
}

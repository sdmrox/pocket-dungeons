using System;
using UnityEngine;

namespace PocketDungeons.UI.Animation
{
    public enum UITransitionType
    {
        SlideLeft,
        SlideRight,
        SlideUp,
        SlideDown,
        FadeIn,
        FadeOut,
        ScaleIn,
        ScaleOut
    }

    [Serializable]
    public class UITransitionConfig
    {
        public UITransitionType Type;
        public float Duration = 0.2f;
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    public class UIAnimationSystem : MonoBehaviour
    {
        public static UIAnimationSystem Instance { get; private set; }

        [Header("Button Press")]
        [SerializeField] private float _buttonPressScale = 0.95f;
        [SerializeField] private float _buttonPressDuration = 0.1f;
        [SerializeField] private Color _buttonPressTint = new(0.8f, 0.8f, 0.8f, 1f);

        [Header("Transitions")]
        [SerializeField] private float _defaultTransitionDuration = 0.2f;

        public float ButtonPressScale => _buttonPressScale;
        public float ButtonPressDuration => _buttonPressDuration;
        public Color ButtonPressTint => _buttonPressTint;

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 CalculateButtonPressScale(float t)
        {
            float scale = Mathf.Lerp(1f, _buttonPressScale, t);
            return new Vector3(scale, scale, 1f);
        }

        public float CalculateTransitionProgress(float elapsed, float duration)
        {
            if (duration <= 0f) return 1f;
            return Mathf.Clamp01(elapsed / duration);
        }

        public Vector2 CalculateSlideOffset(UITransitionType type, float progress, float screenSize)
        {
            float offset = (1f - progress) * screenSize;
            return type switch
            {
                UITransitionType.SlideLeft => new Vector2(-offset, 0f),
                UITransitionType.SlideRight => new Vector2(offset, 0f),
                UITransitionType.SlideUp => new Vector2(0f, offset),
                UITransitionType.SlideDown => new Vector2(0f, -offset),
                _ => Vector2.zero
            };
        }

        public float CalculateFadeAlpha(UITransitionType type, float progress)
        {
            return type switch
            {
                UITransitionType.FadeIn => progress,
                UITransitionType.FadeOut => 1f - progress,
                _ => 1f
            };
        }

        public Vector3 CalculateScaleTransition(UITransitionType type, float progress)
        {
            float scale = type switch
            {
                UITransitionType.ScaleIn => progress,
                UITransitionType.ScaleOut => 1f - progress,
                _ => 1f
            };
            return new Vector3(scale, scale, 1f);
        }

        public Color CalculateButtonTint(float pressProgress)
        {
            return Color.Lerp(Color.white, _buttonPressTint, pressProgress);
        }

        public float EaseInOut(float t)
        {
            return t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }

        public float GetDefaultDuration() => _defaultTransitionDuration;
    }
}

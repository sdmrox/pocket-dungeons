using System.Collections;
using UnityEngine;

namespace PocketDungeons.Gameplay.Combat
{
    /// <summary>
    /// Camera screen shake with exponential decay.
    /// Duration 0.1-0.3s, amplitude 2-8 pixels proportional to damage.
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        public static ScreenShake Instance { get; private set; }

        [SerializeField] private float _maxAmplitude = 8f;
        [SerializeField] private float _decayRate = 15f;

        private Vector3 _originalPosition;
        private float _currentAmplitude;
        private bool _isShaking;

        private void Awake()
        {
            Instance = this;
            _originalPosition = transform.localPosition;
        }

        public void Shake(float duration, float amplitude)
        {
            amplitude = Mathf.Min(amplitude, _maxAmplitude);
            if (amplitude > _currentAmplitude || !_isShaking)
            {
                StopAllCoroutines();
                StartCoroutine(ShakeRoutine(duration, amplitude));
            }
        }

        /// <summary>
        /// Convenience: shake proportional to damage dealt.
        /// </summary>
        public void ShakeForDamage(int damage, bool isCrit)
        {
            float normalized = Mathf.Clamp01(damage / 50f);
            float amplitude = Mathf.Lerp(2f, _maxAmplitude, normalized);
            float duration = Mathf.Lerp(0.1f, 0.3f, normalized);

            if (isCrit)
            {
                amplitude *= 1.5f;
                duration *= 1.3f;
            }

            Shake(duration, amplitude);
        }

        private IEnumerator ShakeRoutine(float duration, float amplitude)
        {
            _isShaking = true;
            _currentAmplitude = amplitude;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float decay = Mathf.Exp(-_decayRate * (elapsed / duration));
                float x = Random.Range(-1f, 1f) * _currentAmplitude * decay * 0.01f;
                float y = Random.Range(-1f, 1f) * _currentAmplitude * decay * 0.01f;

                transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _originalPosition;
            _isShaking = false;
            _currentAmplitude = 0f;
        }
    }
}

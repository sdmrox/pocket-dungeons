using System.Collections;
using UnityEngine;
using TMPro;

namespace PocketDungeons.Gameplay.Combat
{
    /// <summary>
    /// Floating damage number popup. Floats up and fades out.
    /// Crits are gold and larger. Max 3 visible at a time (managed by DamagePopupManager).
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float _floatSpeed = 1.5f;
        [SerializeField] private float _floatDuration = 0.8f;
        [SerializeField] private float _fadeStartPercent = 0.5f;
        [SerializeField] private float _horizontalSpread = 0.3f;

        [Header("Sizing")]
        [SerializeField] private float _normalScale = 0.8f;
        [SerializeField] private float _critScale = 1.2f;
        [SerializeField] private float _punchScale = 1.3f;

        [Header("Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _critColor = new(1f, 0.84f, 0f);
        [SerializeField] private Color _healColor = new(0.2f, 1f, 0.3f);

        private TextMeshPro _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();
        }

        public void Initialize(int amount, bool isCrit, bool isHeal = false)
        {
            if (_text == null) return;

            _text.text = isHeal ? $"+{amount}" : amount.ToString();
            _text.color = isHeal ? _healColor : (isCrit ? _critColor : _normalColor);

            float scale = isCrit ? _critScale : _normalScale;
            transform.localScale = Vector3.one * scale;

            StartCoroutine(AnimatePopup(isCrit));
        }

        private IEnumerator AnimatePopup(bool isCrit)
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            float xOffset = Random.Range(-_horizontalSpread, _horizontalSpread);

            // Punch scale effect
            if (isCrit)
            {
                float punchDuration = 0.1f;
                float punchElapsed = 0f;
                Vector3 baseScale = transform.localScale;
                while (punchElapsed < punchDuration)
                {
                    float t = punchElapsed / punchDuration;
                    float s = Mathf.Lerp(_punchScale, 1f, t);
                    transform.localScale = baseScale * s;
                    punchElapsed += Time.deltaTime;
                    yield return null;
                }
                transform.localScale = baseScale;
            }

            while (elapsed < _floatDuration)
            {
                float t = elapsed / _floatDuration;

                // Float upward with slight horizontal drift
                transform.position = startPos + new Vector3(
                    xOffset * t,
                    _floatSpeed * t,
                    0f);

                // Fade out in second half
                if (t > _fadeStartPercent && _text != null)
                {
                    float fadeT = (t - _fadeStartPercent) / (1f - _fadeStartPercent);
                    Color c = _text.color;
                    c.a = 1f - fadeT;
                    _text.color = c;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}

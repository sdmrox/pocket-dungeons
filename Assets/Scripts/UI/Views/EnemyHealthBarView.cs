using UnityEngine;
using UnityEngine.UI;

namespace PocketDungeons.UI.Views
{
    /// <summary>
    /// Floating health bar above enemies. Shows on damage, fades after delay.
    /// Uses world-space canvas attached to enemy prefab.
    /// </summary>
    public class EnemyHealthBarView : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _damageSlider;
        [SerializeField] private float _damageFollowSpeed = 3f;
        [SerializeField] private float _showDuration = 3f;
        [SerializeField] private CanvasGroup _canvasGroup;

        private float _showTimer;
        private float _targetHealth = 1f;
        private float _displayedDamage = 1f;
        private bool _visible;

        private void Start()
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            // Damage bar lerp (trails behind actual health)
            if (_damageSlider != null && _displayedDamage > _targetHealth)
            {
                _displayedDamage = Mathf.MoveTowards(_displayedDamage, _targetHealth, _damageFollowSpeed * Time.deltaTime);
                _damageSlider.value = _displayedDamage;
            }

            // Fade out timer
            if (_visible)
            {
                _showTimer -= Time.deltaTime;
                if (_showTimer <= 0f)
                {
                    _visible = false;
                    if (_canvasGroup != null)
                        _canvasGroup.alpha = 0f;
                }
            }
        }

        public void UpdateHealth(float healthPercent)
        {
            _targetHealth = healthPercent;

            if (_healthSlider != null)
                _healthSlider.value = healthPercent;

            // Show the bar
            _visible = true;
            _showTimer = _showDuration;
            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;
        }
    }
}

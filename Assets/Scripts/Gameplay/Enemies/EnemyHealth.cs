using System;
using UnityEngine;
using PocketDungeons.Gameplay.Combat;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Enemy health component. Handles damage, death, damage popups, screen shake, and hit-stop.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 10;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _damagePopupPrefab;
        [SerializeField] private float _flashDuration = 0.08f;

        private int _currentHealth;
        private float _flashTimer;
        private Color _originalColor;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public float HealthPercent => (float)_currentHealth / _maxHealth;
        public bool IsAlive => _currentHealth > 0;

        public event Action<EnemyHealth> OnDeath;

        private void Awake()
        {
            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;
        }

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        private void Update()
        {
            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                if (_flashTimer <= 0f && _spriteRenderer != null)
                    _spriteRenderer.color = _originalColor;
            }
        }

        public void TakeDamage(int amount, bool isCrit, Vector3 sourcePosition)
        {
            if (!IsAlive || amount <= 0) return;

            _currentHealth = Mathf.Max(0, _currentHealth - amount);

            // Damage flash
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
                _flashTimer = _flashDuration;
            }

            // Damage popup
            if (_damagePopupPrefab != null)
            {
                var popup = Instantiate(_damagePopupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                var damagePopup = popup.GetComponent<DamagePopup>();
                damagePopup?.Initialize(amount, isCrit);
            }

            // Juice: screen shake + hit-stop
            ScreenShake.Instance?.ShakeForDamage(amount, isCrit);
            HitStop.Instance?.Stop(isCrit);

            if (_currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            OnDeath?.Invoke(this);
        }
    }
}

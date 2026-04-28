using System;
using UnityEngine;

namespace PocketDungeons.Gameplay.Player
{
    /// <summary>
    /// Player health with invincibility support, damage flash, and death handling.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _damageFlashDuration = 0.1f;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private int _currentHealth;
        private bool _isInvincible;
        private float _flashTimer;
        private Color _originalColor;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public float HealthPercent => (float)_currentHealth / _maxHealth;
        public bool IsAlive => _currentHealth > 0;
        public bool IsInvincible => _isInvincible;

        public event Action<int, int> OnHealthChanged; // current, max
        public event Action<int> OnDamageTaken; // damage amount
        public event Action OnDeath;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;
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

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive || _isInvincible || amount <= 0) return;

            _currentHealth = Mathf.Max(0, _currentHealth - amount);
            OnDamageTaken?.Invoke(amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.red;
                _flashTimer = _damageFlashDuration;
            }

            if (_currentHealth <= 0)
                OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0) return;

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void SetInvincible(bool invincible)
        {
            _isInvincible = invincible;
        }
    }
}

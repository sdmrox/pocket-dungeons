using System;
using System.Collections;
using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Base class for boss enemies. Multi-phase with intro sequence,
    /// phase transitions at HP thresholds, and death animation.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BossBase : MonoBehaviour
    {
        public enum BossPhase { Intro, Phase1, Phase2, Phase3, Death }

        [Header("Boss Config")]
        [SerializeField] protected int _baseHealth = 500;
        [SerializeField] protected int _baseDamage = 15;
        [SerializeField] protected float _phase2Threshold = 0.6f;
        [SerializeField] protected float _phase3Threshold = 0.3f;

        protected BossPhase _currentPhase = BossPhase.Intro;
        protected Transform _player;
        protected Rigidbody2D _rb;
        protected EnemyHealth _health;
        protected SpriteRenderer _spriteRenderer;

        public BossPhase CurrentPhase => _currentPhase;
        public string BossName { get; protected set; } = "Boss";

        public event Action<BossPhase> OnPhaseChanged;
        public event Action OnBossDefeated;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _health = GetComponent<EnemyHealth>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _health.OnDeath += OnDeath;
        }

        public virtual void Initialize(int scaledHealth, int scaledDamage, Transform player)
        {
            _baseHealth = scaledHealth;
            _baseDamage = scaledDamage;
            _player = player;
            _health.Initialize(scaledHealth);
            StartCoroutine(IntroSequence());
        }

        protected virtual IEnumerator IntroSequence()
        {
            _currentPhase = BossPhase.Intro;

            // Slow zoom effect placeholder
            yield return new WaitForSeconds(1.5f);

            _currentPhase = BossPhase.Phase1;
            OnPhaseChanged?.Invoke(_currentPhase);
        }

        protected virtual void Update()
        {
            if (_currentPhase == BossPhase.Intro || _currentPhase == BossPhase.Death) return;
            if (_player == null) return;

            // Check phase transitions
            float hp = _health.HealthPercent;
            if (hp <= _phase3Threshold && _currentPhase != BossPhase.Phase3)
            {
                _currentPhase = BossPhase.Phase3;
                OnPhaseChanged?.Invoke(_currentPhase);
                OnEnterPhase3();
            }
            else if (hp <= _phase2Threshold && _currentPhase == BossPhase.Phase1)
            {
                _currentPhase = BossPhase.Phase2;
                OnPhaseChanged?.Invoke(_currentPhase);
                OnEnterPhase2();
            }

            PerformBehavior();
        }

        protected abstract void PerformBehavior();
        protected virtual void OnEnterPhase2() { }
        protected virtual void OnEnterPhase3() { }

        protected virtual void OnDeath(EnemyHealth health)
        {
            _currentPhase = BossPhase.Death;
            _rb.linearVelocity = Vector2.zero;
            OnBossDefeated?.Invoke();
            StartCoroutine(DeathSequence());
        }

        protected virtual IEnumerator DeathSequence()
        {
            // Flash and shake
            for (int i = 0; i < 10; i++)
            {
                if (_spriteRenderer != null)
                    _spriteRenderer.color = i % 2 == 0 ? Color.white : Color.red;

                Combat.ScreenShake.Instance?.Shake(0.1f, 4f);
                yield return new WaitForSeconds(0.15f);
            }

            Combat.ScreenShake.Instance?.Shake(0.5f, 12f);
            yield return new WaitForSeconds(0.5f);

            Destroy(gameObject);
        }

        protected void OnDestroy()
        {
            if (_health != null)
                _health.OnDeath -= OnDeath;
        }
    }
}

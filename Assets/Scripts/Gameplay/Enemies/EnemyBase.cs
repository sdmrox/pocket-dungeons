using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Base class for all enemy behaviors. Handles common state: idle, chase, attack, dead.
    /// Subclasses override AI behavior methods.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyBase : MonoBehaviour
    {
        protected enum EnemyState { Idle, Chase, Attack, Dead }

        [Header("Base Settings")]
        [SerializeField] protected float _detectionRange = 8f;
        [SerializeField] protected float _attackRange = 1f;
        [SerializeField] protected float _moveSpeed = 2f;
        [SerializeField] protected float _attackCooldown = 1.5f;
        [SerializeField] protected int _damage = 3;

        protected EnemyState _state = EnemyState.Idle;
        protected Transform _player;
        protected Rigidbody2D _rb;
        protected EnemyHealth _health;
        protected float _attackTimer;
        protected SpriteRenderer _spriteRenderer;

        public EnemyData Data { get; private set; }

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _health = GetComponent<EnemyHealth>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _health.OnDeath += OnDeath;
        }

        public virtual void Initialize(EnemyData data, int scaledHealth, int scaledDamage, Transform player)
        {
            Data = data;
            _detectionRange = data.DetectionRange;
            _attackRange = data.AttackRange;
            _moveSpeed = data.MoveSpeed;
            _attackCooldown = data.AttackCooldown;
            _damage = scaledDamage;
            _player = player;
            _health.Initialize(scaledHealth);
            _state = EnemyState.Idle;
        }

        protected virtual void Update()
        {
            if (_state == EnemyState.Dead) return;
            if (_player == null) return;

            _attackTimer -= Time.deltaTime;
            float distToPlayer = Vector2.Distance(transform.position, _player.position);

            if (distToPlayer <= _attackRange && _attackTimer <= 0f)
            {
                _state = EnemyState.Attack;
                PerformAttack();
                _attackTimer = _attackCooldown;
            }
            else if (distToPlayer <= _detectionRange)
            {
                _state = EnemyState.Chase;
                PerformChase();
            }
            else
            {
                _state = EnemyState.Idle;
                PerformIdle();
            }

            UpdateFacing();
        }

        protected abstract void PerformChase();
        protected abstract void PerformAttack();

        protected virtual void PerformIdle()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        protected void UpdateFacing()
        {
            if (_player == null || _spriteRenderer == null) return;
            bool facingRight = _player.position.x > transform.position.x;
            _spriteRenderer.flipX = !facingRight;
        }

        protected virtual void OnDeath(EnemyHealth health)
        {
            _state = EnemyState.Dead;
            _rb.linearVelocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
        }

        protected void OnDestroy()
        {
            if (_health != null)
                _health.OnDeath -= OnDeath;
        }
    }
}

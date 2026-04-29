using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Ghost — phases through walls, periodically teleports near the player.
    /// Semi-transparent, only solid (damageable) when attacking.
    /// </summary>
    public class GhostEnemy : EnemyBase
    {
        [Header("Ghost")]
        [SerializeField] private float _teleportCooldown = 3f;
        [SerializeField] private float _teleportRange = 2f;
        [SerializeField] private float _phaseAlpha = 0.3f;
        [SerializeField] private float _solidAlpha = 0.9f;
        [SerializeField] private float _solidDuration = 1.5f;

        private float _teleportTimer;
        private bool _isPhased = true;
        private float _solidTimer;
        private Collider2D _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider2D>();
            SetPhased(true);
        }

        protected override void Update()
        {
            base.Update();

            if (_state == EnemyState.Dead) return;

            if (!_isPhased)
            {
                _solidTimer -= Time.deltaTime;
                if (_solidTimer <= 0f)
                    SetPhased(true);
            }

            _teleportTimer -= Time.deltaTime;
        }

        protected override void PerformChase()
        {
            if (_isPhased)
            {
                // Drift toward player slowly while phased
                Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
                _rb.linearVelocity = dir * _moveSpeed * 0.5f;

                if (_teleportTimer <= 0f)
                {
                    Teleport();
                    _teleportTimer = _teleportCooldown;
                }
            }
            else
            {
                // Solid — chase normally
                Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
                _rb.linearVelocity = dir * _moveSpeed;
            }
        }

        protected override void PerformAttack()
        {
            SetPhased(false);

            var playerHealth = _player.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible)
            {
                playerHealth.TakeDamage(_damage);
            }
        }

        private void Teleport()
        {
            Vector2 offset = Random.insideUnitCircle.normalized * _teleportRange;
            Vector2 targetPos = (Vector2)_player.position + offset;
            _rb.position = targetPos;
            SetPhased(false);
        }

        private void SetPhased(bool phased)
        {
            _isPhased = phased;

            if (_spriteRenderer != null)
            {
                Color c = _spriteRenderer.color;
                c.a = phased ? _phaseAlpha : _solidAlpha;
                _spriteRenderer.color = c;
            }

            // Phased enemies can't be hit and don't block player
            if (_collider != null)
                _collider.enabled = !phased;

            if (!phased)
                _solidTimer = _solidDuration;
        }
    }
}

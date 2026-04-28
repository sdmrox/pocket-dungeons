using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Shield Knight — blocks frontal attacks with a shield.
    /// Must be flanked to take damage. Advances slowly, periodic shield bash attack.
    /// </summary>
    public class ShieldKnightEnemy : EnemyBase
    {
        [Header("Shield Knight")]
        [SerializeField] private float _shieldArc = 120f;
        [SerializeField] private float _bashRange = 1.5f;
        [SerializeField] private float _bashKnockback = 5f;
        [SerializeField] private SpriteRenderer _shieldSprite;
        [SerializeField] private Color _shieldBlockColor = new(0.3f, 0.7f, 1f, 0.8f);

        private Vector2 _facingDirection;
        private bool _justBlocked;

        protected override void PerformChase()
        {
            _facingDirection = ((Vector2)_player.position - _rb.position).normalized;
            _rb.linearVelocity = _facingDirection * _moveSpeed;
        }

        protected override void PerformAttack()
        {
            _rb.linearVelocity = Vector2.zero;

            var playerHealth = _player.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible)
            {
                playerHealth.TakeDamage(_damage);

                // Bash knockback
                var playerRb = _player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockDir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                    playerRb.AddForce(knockDir * _bashKnockback, ForceMode2D.Impulse);
                }
            }
        }

        /// <summary>
        /// Override to check shield blocking. Called externally before applying damage.
        /// Returns true if the attack is blocked.
        /// </summary>
        public bool IsBlocking(Vector3 attackSourcePosition)
        {
            Vector2 toAttacker = ((Vector2)attackSourcePosition - _rb.position).normalized;
            float angle = Vector2.Angle(_facingDirection, toAttacker);

            bool blocked = angle < _shieldArc * 0.5f;

            if (blocked)
            {
                _justBlocked = true;

                // Visual feedback
                if (_shieldSprite != null)
                    _shieldSprite.color = _shieldBlockColor;

                Combat.ScreenShake.Instance?.Shake(0.05f, 2f);
            }

            return blocked;
        }

        protected override void Update()
        {
            base.Update();

            if (_justBlocked)
            {
                _justBlocked = false;
                if (_shieldSprite != null)
                    _shieldSprite.color = Color.white;
            }
        }
    }
}

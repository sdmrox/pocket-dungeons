using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Slime — walks toward player, melee attack on contact.
    /// Simple chase AI. Slow but persistent.
    /// </summary>
    public class SlimeEnemy : EnemyBase
    {
        [Header("Slime")]
        [SerializeField] private float _hopForce = 3f;
        [SerializeField] private float _hopInterval = 0.8f;

        private float _hopTimer;

        protected override void PerformChase()
        {
            _hopTimer -= Time.deltaTime;

            if (_hopTimer <= 0f)
            {
                Vector2 direction = ((Vector2)_player.position - _rb.position).normalized;
                _rb.linearVelocity = direction * _moveSpeed;
                _hopTimer = _hopInterval;
            }
        }

        protected override void PerformAttack()
        {
            _rb.linearVelocity = Vector2.zero;

            var playerHealth = _player.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible)
            {
                playerHealth.TakeDamage(_damage);
            }
        }
    }
}

using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Bat — fast, erratic movement. Random direction changes with occasional bursts toward player.
    /// Hard to hit but low health.
    /// </summary>
    public class BatEnemy : EnemyBase
    {
        [Header("Bat")]
        [SerializeField] private float _directionChangeInterval = 0.5f;
        [SerializeField] private float _burstChance = 0.3f;
        [SerializeField] private float _burstSpeedMultiplier = 2f;
        [SerializeField] private float _wobbleAmplitude = 1.5f;

        private float _dirTimer;
        private Vector2 _currentDirection;
        private bool _isBursting;

        protected override void PerformChase()
        {
            _dirTimer -= Time.deltaTime;

            if (_dirTimer <= 0f)
            {
                _dirTimer = _directionChangeInterval + Random.Range(-0.2f, 0.2f);

                if (Random.value < _burstChance)
                {
                    // Burst toward player
                    _currentDirection = ((Vector2)_player.position - _rb.position).normalized;
                    _isBursting = true;
                }
                else
                {
                    // Random wander with bias toward player
                    Vector2 toPlayer = ((Vector2)_player.position - _rb.position).normalized;
                    Vector2 randomDir = Random.insideUnitCircle.normalized;
                    _currentDirection = (toPlayer * 0.4f + randomDir * 0.6f).normalized;
                    _isBursting = false;
                }
            }

            // Add wobble for erratic feel
            float wobble = Mathf.Sin(Time.time * 10f) * _wobbleAmplitude;
            Vector2 perpendicular = new(-_currentDirection.y, _currentDirection.x);
            Vector2 finalDir = _currentDirection + perpendicular * wobble * 0.1f;

            float speed = _isBursting ? _moveSpeed * _burstSpeedMultiplier : _moveSpeed;
            _rb.linearVelocity = finalDir.normalized * speed;
        }

        protected override void PerformAttack()
        {
            // Bat attacks by flying into the player (contact damage)
            var playerHealth = _player.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible)
            {
                playerHealth.TakeDamage(_damage);
            }

            // Bounce away after hitting
            Vector2 bounceDir = ((Vector2)transform.position - (Vector2)_player.position).normalized;
            _rb.linearVelocity = bounceDir * _moveSpeed * _burstSpeedMultiplier;
        }
    }
}

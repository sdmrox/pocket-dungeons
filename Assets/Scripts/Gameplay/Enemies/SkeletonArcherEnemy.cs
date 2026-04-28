using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Skeleton Archer — stands still, shoots projectiles at the player.
    /// Keeps distance; retreats if player gets too close.
    /// </summary>
    public class SkeletonArcherEnemy : EnemyBase
    {
        [Header("Archer")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 6f;
        [SerializeField] private float _preferredDistance = 5f;
        [SerializeField] private float _retreatSpeed = 3f;
        [SerializeField] private Transform _firePoint;

        protected override void PerformChase()
        {
            float dist = Vector2.Distance(transform.position, _player.position);

            if (dist < _preferredDistance * 0.6f)
            {
                // Too close — retreat
                Vector2 awayDir = ((Vector2)transform.position - (Vector2)_player.position).normalized;
                _rb.linearVelocity = awayDir * _retreatSpeed;
            }
            else if (dist > _preferredDistance * 1.2f)
            {
                // Too far — approach slowly
                Vector2 toPlayer = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                _rb.linearVelocity = toPlayer * _moveSpeed * 0.5f;
            }
            else
            {
                // Good range — stop and aim
                _rb.linearVelocity = Vector2.zero;
            }
        }

        protected override void PerformAttack()
        {
            _rb.linearVelocity = Vector2.zero;

            if (_projectilePrefab == null) return;

            Transform spawnPoint = _firePoint != null ? _firePoint : transform;
            Vector2 direction = ((Vector2)_player.position - (Vector2)spawnPoint.position).normalized;

            var projectile = Instantiate(_projectilePrefab, spawnPoint.position, Quaternion.identity);
            var rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * _projectileSpeed;
            }

            var proj = projectile.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Initialize(_damage, direction);
            }
        }
    }
}

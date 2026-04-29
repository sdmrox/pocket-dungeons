using UnityEngine;

namespace PocketDungeons.Gameplay.Player
{
    /// <summary>
    /// Auto-attack system: targets nearest enemy in range, attacks on cooldown.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private int _baseDamage = 10;
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _attackCooldown = 0.8f;
        [SerializeField] private float _critChance = 0.05f;
        [SerializeField] private float _critMultiplier = 2f;

        [Header("Detection")]
        [SerializeField] private LayerMask _enemyLayer;

        private float _cooldownTimer;
        private Transform _currentTarget;

        public Transform CurrentTarget => _currentTarget;
        public float AttackRange => _attackRange;
        public float CooldownProgress => 1f - Mathf.Clamp01(_cooldownTimer / _attackCooldown);

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            _currentTarget = FindNearestEnemy();

            if (_currentTarget != null && _cooldownTimer <= 0f)
            {
                Attack(_currentTarget);
                _cooldownTimer = _attackCooldown;
            }
        }

        private Transform FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayer);
            if (hits.Length == 0) return null;

            Transform nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = hit.transform;
                }
            }

            return nearest;
        }

        private void Attack(Transform target)
        {
            var enemyHealth = target.GetComponent<Gameplay.Enemies.EnemyHealth>();
            if (enemyHealth == null) return;

            bool isCrit = Random.value < _critChance;
            int damage = isCrit ? Mathf.RoundToInt(_baseDamage * _critMultiplier) : _baseDamage;

            enemyHealth.TakeDamage(damage, isCrit, transform.position);
        }

        public void SetStats(int damage, float range, float speed, float critChance, float critMult)
        {
            _baseDamage = damage;
            _attackRange = range;
            _attackCooldown = 1f / speed;
            _critChance = critChance;
            _critMultiplier = critMult;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}

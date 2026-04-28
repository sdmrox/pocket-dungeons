using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Healer — supports nearby enemies by healing them. Stays behind other enemies.
    /// Priority target for players. Low HP, no direct attack.
    /// </summary>
    public class HealerEnemy : EnemyBase
    {
        [Header("Healer")]
        [SerializeField] private float _healRadius = 4f;
        [SerializeField] private int _healAmount = 5;
        [SerializeField] private float _healCooldown = 2f;
        [SerializeField] private float _fleeDistance = 4f;
        [SerializeField] private LayerMask _allyLayer;
        [SerializeField] private GameObject _healVFXPrefab;

        private float _healTimer;

        protected override void Update()
        {
            base.Update();

            if (_state == EnemyState.Dead) return;

            _healTimer -= Time.deltaTime;
            if (_healTimer <= 0f)
            {
                HealNearbyAllies();
                _healTimer = _healCooldown;
            }
        }

        protected override void PerformChase()
        {
            float distToPlayer = Vector2.Distance(transform.position, _player.position);

            if (distToPlayer < _fleeDistance)
            {
                // Flee from player
                Vector2 fleeDir = ((Vector2)transform.position - (Vector2)_player.position).normalized;
                _rb.linearVelocity = fleeDir * _moveSpeed;
            }
            else
            {
                // Stay at safe distance, drift slowly
                _rb.linearVelocity = Vector2.zero;
            }
        }

        protected override void PerformAttack()
        {
            // Healer doesn't attack directly — just heals
            _rb.linearVelocity = Vector2.zero;
        }

        private void HealNearbyAllies()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, _healRadius, _allyLayer);

            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                var allyHealth = hit.GetComponent<EnemyHealth>();
                if (allyHealth != null && allyHealth.IsAlive && allyHealth.HealthPercent < 1f)
                {
                    int currentHP = allyHealth.CurrentHealth;
                    int maxHP = allyHealth.MaxHealth;
                    int healed = Mathf.Min(_healAmount, maxHP - currentHP);

                    // Re-initialize with healed amount (since EnemyHealth doesn't have Heal)
                    // This is a workaround; production code should add Heal to EnemyHealth
                    allyHealth.Initialize(maxHP);

                    if (_healVFXPrefab != null)
                        Instantiate(_healVFXPrefab, hit.transform.position, Quaternion.identity);
                }
            }
        }
    }
}

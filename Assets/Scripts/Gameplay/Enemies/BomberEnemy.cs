using System.Collections;
using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Bomber — rushes toward player and explodes on contact or when close enough.
    /// Deals heavy AoE damage. Dies on explosion. Flashes red before detonating.
    /// </summary>
    public class BomberEnemy : EnemyBase
    {
        [Header("Bomber")]
        [SerializeField] private float _rushSpeed = 6f;
        [SerializeField] private float _explosionRadius = 2f;
        [SerializeField] private float _fuseTime = 1f;
        [SerializeField] private float _explosionDamageMultiplier = 3f;
        [SerializeField] private float _triggerDistance = 1.5f;
        [SerializeField] private GameObject _explosionVFXPrefab;

        private bool _triggered;
        private bool _exploded;

        protected override void PerformChase()
        {
            if (_exploded) return;

            Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
            _rb.linearVelocity = dir * _rushSpeed;

            float dist = Vector2.Distance(transform.position, _player.position);
            if (dist < _triggerDistance && !_triggered)
            {
                _triggered = true;
                StartCoroutine(FuseRoutine());
            }
        }

        protected override void PerformAttack()
        {
            if (!_triggered)
            {
                _triggered = true;
                StartCoroutine(FuseRoutine());
            }
        }

        private IEnumerator FuseRoutine()
        {
            _rb.linearVelocity = Vector2.zero;
            float elapsed = 0f;

            // Flash red increasingly fast
            while (elapsed < _fuseTime)
            {
                float rate = Mathf.Lerp(4f, 20f, elapsed / _fuseTime);
                bool flash = Mathf.Sin(elapsed * rate * Mathf.PI) > 0;

                if (_spriteRenderer != null)
                    _spriteRenderer.color = flash ? Color.red : Color.white;

                elapsed += Time.deltaTime;
                yield return null;
            }

            Explode();
        }

        private void Explode()
        {
            if (_exploded) return;
            _exploded = true;

            int explosionDamage = Mathf.RoundToInt(_damage * _explosionDamageMultiplier);

            // Damage player if in range
            float distToPlayer = Vector2.Distance(transform.position, _player.position);
            if (distToPlayer < _explosionRadius)
            {
                var playerHealth = _player.GetComponent<Player.PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsInvincible)
                {
                    float falloff = 1f - (distToPlayer / _explosionRadius);
                    int finalDamage = Mathf.RoundToInt(explosionDamage * falloff);
                    playerHealth.TakeDamage(finalDamage);
                }
            }

            Combat.ScreenShake.Instance?.Shake(0.3f, 8f);

            if (_explosionVFXPrefab != null)
                Instantiate(_explosionVFXPrefab, transform.position, Quaternion.identity);

            _health.TakeDamage(_health.MaxHealth, false, transform.position);
        }
    }
}

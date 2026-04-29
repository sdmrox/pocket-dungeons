using System.Collections;
using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Crypt King — First boss (Floor 10). Skeleton lord with a massive sword.
    /// Phase 1: Slash combos + summon skeletons
    /// Phase 2: Ground slam AoE + faster attacks
    /// Phase 3: Enraged — all attacks faster, summons more minions, screen darkens
    /// </summary>
    public class CryptKingBoss : BossBase
    {
        [Header("Crypt King")]
        [SerializeField] private float _slashRange = 2f;
        [SerializeField] private float _slamRadius = 3.5f;
        [SerializeField] private float _summonCooldown = 8f;
        [SerializeField] private int _minionsPerSummon = 2;
        [SerializeField] private GameObject _minionPrefab;
        [SerializeField] private GameObject _slamVFXPrefab;
        [SerializeField] private LayerMask _playerLayer;

        private float _attackTimer;
        private float _summonTimer;
        private float _attackCooldown = 1.5f;
        private int _comboCount;

        protected override void Awake()
        {
            base.Awake();
            BossName = "Crypt King";
        }

        protected override void PerformBehavior()
        {
            _attackTimer -= Time.deltaTime;
            _summonTimer -= Time.deltaTime;

            float dist = Vector2.Distance(transform.position, _player.position);

            // Summon minions
            if (_summonTimer <= 0f)
            {
                StartCoroutine(SummonMinions());
                _summonTimer = _summonCooldown;
            }

            // Move toward player
            if (dist > _slashRange)
            {
                Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
                float speed = _currentPhase == BossPhase.Phase3 ? 4f : 2.5f;
                _rb.linearVelocity = dir * speed;
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;

                if (_attackTimer <= 0f)
                {
                    if (_currentPhase >= BossPhase.Phase2 && _comboCount >= 2)
                    {
                        StartCoroutine(GroundSlam());
                        _comboCount = 0;
                    }
                    else
                    {
                        SlashAttack();
                        _comboCount++;
                    }
                    _attackTimer = _attackCooldown;
                }
            }

            UpdateFacing();
        }

        private void SlashAttack()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, _slashRange, _playerLayer);
            foreach (var hit in hits)
            {
                var playerHealth = hit.GetComponent<Player.PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsInvincible)
                {
                    playerHealth.TakeDamage(_baseDamage);
                    Combat.ScreenShake.Instance?.ShakeForDamage(_baseDamage, false);
                }
            }
        }

        private IEnumerator GroundSlam()
        {
            // Telegraph — raise sprite
            Vector3 origPos = _spriteRenderer.transform.localPosition;
            _spriteRenderer.transform.localPosition = origPos + Vector3.up * 0.3f;

            yield return new WaitForSeconds(0.4f);

            // Slam down
            _spriteRenderer.transform.localPosition = origPos;
            Combat.ScreenShake.Instance?.Shake(0.4f, 10f);

            if (_slamVFXPrefab != null)
                Instantiate(_slamVFXPrefab, transform.position, Quaternion.identity);

            var hits = Physics2D.OverlapCircleAll(transform.position, _slamRadius, _playerLayer);
            int slamDamage = Mathf.RoundToInt(_baseDamage * 1.5f);

            foreach (var hit in hits)
            {
                var playerHealth = hit.GetComponent<Player.PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsInvincible)
                {
                    playerHealth.TakeDamage(slamDamage);

                    var rb = hit.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 knockDir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
                        rb.AddForce(knockDir * 10f, ForceMode2D.Impulse);
                    }
                }
            }
        }

        private IEnumerator SummonMinions()
        {
            if (_minionPrefab == null) yield break;

            int count = _currentPhase == BossPhase.Phase3 ? _minionsPerSummon * 2 : _minionsPerSummon;

            for (int i = 0; i < count; i++)
            {
                Vector2 offset = Random.insideUnitCircle.normalized * 3f;
                Vector3 spawnPos = transform.position + (Vector3)offset;
                Instantiate(_minionPrefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
            }
        }

        protected override void OnEnterPhase2()
        {
            _attackCooldown = 1.2f;
            _summonCooldown = 6f;
        }

        protected override void OnEnterPhase3()
        {
            _attackCooldown = 0.8f;
            _summonCooldown = 4f;
            _minionsPerSummon = 3;

            // Visual rage effect
            if (_spriteRenderer != null)
                _spriteRenderer.color = new Color(1f, 0.6f, 0.6f);
        }

        private void UpdateFacing()
        {
            if (_player == null || _spriteRenderer == null) return;
            _spriteRenderer.flipX = _player.position.x < transform.position.x;
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.Player
{
    /// <summary>
    /// Hero ability system. Each hero class has a unique ability on cooldown.
    /// Warrior: Shield Bash (AoE stun + knockback)
    /// Archer: Rain of Arrows (area damage)
    /// Mage: Meteor Strike (high damage + burn)
    /// </summary>
    public class HeroAbility : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float _cooldown = 8f;
        [SerializeField] private LayerMask _enemyLayer;

        private HeroClass _heroClass;
        private int _baseDamage;
        private float _cooldownTimer;
        private bool _isActive;

        public float CooldownProgress => 1f - Mathf.Clamp01(_cooldownTimer / _cooldown);
        public bool IsReady => _cooldownTimer <= 0f && !_isActive;

        public event Action OnAbilityUsed;
        public event Action OnAbilityReady;

        public void Initialize(HeroData hero)
        {
            _heroClass = hero.Class;
            _cooldown = hero.AbilityCooldown;
            _baseDamage = hero.BaseDamage * 3;
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0f)
                    OnAbilityReady?.Invoke();
            }
        }

        public void TryActivate()
        {
            if (!IsReady) return;

            _cooldownTimer = _cooldown;
            OnAbilityUsed?.Invoke();

            switch (_heroClass)
            {
                case HeroClass.Warrior:
                    StartCoroutine(ShieldBash());
                    break;
                case HeroClass.Archer:
                    StartCoroutine(RainOfArrows());
                    break;
                case HeroClass.Mage:
                    StartCoroutine(MeteorStrike());
                    break;
            }
        }

        private IEnumerator ShieldBash()
        {
            _isActive = true;
            float radius = 2.5f;

            var hits = Physics2D.OverlapCircleAll(transform.position, radius, _enemyLayer);
            foreach (var hit in hits)
            {
                var enemyHealth = hit.GetComponent<Enemies.EnemyHealth>();
                enemyHealth?.TakeDamage(_baseDamage, false, transform.position);

                var rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 knockDir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
                    rb.AddForce(knockDir * 8f, ForceMode2D.Impulse);
                }
            }

            yield return new WaitForSeconds(0.3f);
            _isActive = false;
        }

        private IEnumerator RainOfArrows()
        {
            _isActive = true;
            float radius = 3f;
            int waves = 3;

            for (int i = 0; i < waves; i++)
            {
                var hits = Physics2D.OverlapCircleAll(transform.position, radius, _enemyLayer);
                int damagePerWave = _baseDamage / waves;

                foreach (var hit in hits)
                {
                    var enemyHealth = hit.GetComponent<Enemies.EnemyHealth>();
                    enemyHealth?.TakeDamage(damagePerWave, false, transform.position);
                }

                yield return new WaitForSeconds(0.2f);
            }

            _isActive = false;
        }

        private IEnumerator MeteorStrike()
        {
            _isActive = true;

            yield return new WaitForSeconds(0.5f);

            float radius = 2f;
            var hits = Physics2D.OverlapCircleAll(transform.position, radius, _enemyLayer);
            foreach (var hit in hits)
            {
                var enemyHealth = hit.GetComponent<Enemies.EnemyHealth>();
                enemyHealth?.TakeDamage(_baseDamage * 2, true, transform.position);
            }

            Combat.ScreenShake.Instance?.Shake(0.4f, 10f);

            _isActive = false;
        }
    }
}

using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Golem — slow tank that charges in a straight line.
    /// Telegraphs charge with a wind-up, then rushes. Vulnerable during recovery.
    /// </summary>
    public class GolemEnemy : EnemyBase
    {
        [Header("Golem")]
        [SerializeField] private float _chargeSpeed = 10f;
        [SerializeField] private float _windUpDuration = 0.8f;
        [SerializeField] private float _chargeDuration = 0.6f;
        [SerializeField] private float _recoveryDuration = 1.2f;

        private enum GolemPhase { Walk, WindUp, Charge, Recovery }
        private GolemPhase _phase = GolemPhase.Walk;
        private float _phaseTimer;
        private Vector2 _chargeDirection;

        protected override void PerformChase()
        {
            switch (_phase)
            {
                case GolemPhase.Walk:
                    Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
                    _rb.linearVelocity = dir * _moveSpeed;

                    float dist = Vector2.Distance(transform.position, _player.position);
                    if (dist < _attackRange * 2f && _attackTimer <= 0f)
                    {
                        _phase = GolemPhase.WindUp;
                        _phaseTimer = _windUpDuration;
                        _chargeDirection = dir;
                        _rb.linearVelocity = Vector2.zero;
                    }
                    break;

                case GolemPhase.WindUp:
                    _rb.linearVelocity = Vector2.zero;
                    _phaseTimer -= Time.deltaTime;

                    // Shake sprite to telegraph
                    if (_spriteRenderer != null)
                    {
                        float shake = Mathf.Sin(Time.time * 40f) * 0.05f;
                        _spriteRenderer.transform.localPosition = new Vector3(shake, 0, 0);
                    }

                    if (_phaseTimer <= 0f)
                    {
                        _phase = GolemPhase.Charge;
                        _phaseTimer = _chargeDuration;
                        if (_spriteRenderer != null)
                            _spriteRenderer.transform.localPosition = Vector3.zero;
                    }
                    break;

                case GolemPhase.Charge:
                    _rb.linearVelocity = _chargeDirection * _chargeSpeed;
                    _phaseTimer -= Time.deltaTime;

                    if (_phaseTimer <= 0f)
                    {
                        _phase = GolemPhase.Recovery;
                        _phaseTimer = _recoveryDuration;
                        _rb.linearVelocity = Vector2.zero;
                        _attackTimer = _attackCooldown;
                    }
                    break;

                case GolemPhase.Recovery:
                    _rb.linearVelocity = Vector2.zero;
                    _phaseTimer -= Time.deltaTime;

                    if (_phaseTimer <= 0f)
                        _phase = GolemPhase.Walk;
                    break;
            }
        }

        protected override void PerformAttack()
        {
            // Charge handles damage via collision
            var playerHealth = _player.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible && _phase == GolemPhase.Charge)
            {
                playerHealth.TakeDamage(_damage * 2);
                _phase = GolemPhase.Recovery;
                _phaseTimer = _recoveryDuration;
                _rb.linearVelocity = Vector2.zero;
            }
        }
    }
}

using System.Collections;
using UnityEngine;

namespace PocketDungeons.Gameplay.Player
{
    /// <summary>
    /// Dodge roll with invincibility frames. Triggered by swipe input.
    /// 2-second cooldown, ghost trail visual during dodge.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerDodge : MonoBehaviour
    {
        [Header("Dodge Settings")]
        [SerializeField] private float _dodgeSpeed = 12f;
        [SerializeField] private float _dodgeDuration = 0.25f;
        [SerializeField] private float _cooldown = 2f;
        [SerializeField] private float _invincibilityDuration = 0.35f;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _dodgeColor = new(1f, 1f, 1f, 0.4f);
        [SerializeField] private GameObject _ghostTrailPrefab;
        [SerializeField] private float _ghostSpawnInterval = 0.05f;

        private PlayerController _controller;
        private Rigidbody2D _rb;
        private float _cooldownTimer;
        private bool _isDodging;
        private PlayerHealth _health;

        public bool IsDodging => _isDodging;
        public bool CanDodge => _cooldownTimer <= 0f && !_isDodging;
        public float CooldownProgress => 1f - Mathf.Clamp01(_cooldownTimer / _cooldown);

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _rb = GetComponent<Rigidbody2D>();
            _health = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
        }

        public void TryDodge(Vector2 direction)
        {
            if (!CanDodge) return;
            if (direction.sqrMagnitude < 0.1f)
                direction = _controller.IsMoving ? _controller.MoveInput : Vector2.right;

            StartCoroutine(DodgeRoutine(direction.normalized));
        }

        private IEnumerator DodgeRoutine(Vector2 direction)
        {
            _isDodging = true;
            _cooldownTimer = _cooldown;
            _controller.SetInputEnabled(false);

            if (_health != null)
                _health.SetInvincible(true);

            Color originalColor = Color.white;
            if (_spriteRenderer != null)
            {
                originalColor = _spriteRenderer.color;
                _spriteRenderer.color = _dodgeColor;
            }

            float elapsed = 0f;
            float ghostTimer = 0f;

            while (elapsed < _dodgeDuration)
            {
                _rb.linearVelocity = direction * _dodgeSpeed;

                ghostTimer += Time.deltaTime;
                if (_ghostTrailPrefab != null && ghostTimer >= _ghostSpawnInterval)
                {
                    ghostTimer = 0f;
                    SpawnGhost();
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rb.linearVelocity = Vector2.zero;

            if (_spriteRenderer != null)
                _spriteRenderer.color = originalColor;

            // Brief extra invincibility after dodge ends
            yield return new WaitForSeconds(_invincibilityDuration - _dodgeDuration);

            if (_health != null)
                _health.SetInvincible(false);

            _controller.SetInputEnabled(true);
            _isDodging = false;
        }

        private void SpawnGhost()
        {
            var ghost = Instantiate(_ghostTrailPrefab, transform.position, Quaternion.identity);
            var sr = ghost.GetComponent<SpriteRenderer>();
            if (sr != null && _spriteRenderer != null)
            {
                sr.sprite = _spriteRenderer.sprite;
                sr.color = new Color(0.5f, 0.8f, 1f, 0.5f);
                sr.transform.localScale = _spriteRenderer.transform.lossyScale;
            }
            Destroy(ghost, 0.3f);
        }
    }
}

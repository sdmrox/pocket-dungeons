using UnityEngine;
using PocketDungeons.Core.Services;

namespace PocketDungeons.Gameplay.Player
{
    /// <summary>
    /// Handles player movement via virtual joystick input and auto-attack targeting.
    /// Supports 8-directional movement with smooth acceleration.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _acceleration = 50f;
        [SerializeField] private float _deceleration = 40f;

        [Header("References")]
        [SerializeField] private Transform _spriteTransform;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private Vector2 _currentVelocity;
        private bool _facingRight = true;
        private bool _inputEnabled = true;

        public Vector2 Position => _rb.position;
        public Vector2 MoveInput => _moveInput;
        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void SetMoveInput(Vector2 input)
        {
            if (!_inputEnabled) return;
            _moveInput = Vector2.ClampMagnitude(input, 1f);
        }

        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
            if (!enabled) _moveInput = Vector2.zero;
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            UpdateFacing();
        }

        private void ApplyMovement()
        {
            Vector2 targetVelocity = _moveInput * _moveSpeed;
            float rate = _moveInput.sqrMagnitude > 0.01f ? _acceleration : _deceleration;
            _currentVelocity = Vector2.MoveTowards(_currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
            _rb.linearVelocity = _currentVelocity;
        }

        private void UpdateFacing()
        {
            if (Mathf.Abs(_moveInput.x) < 0.1f) return;

            bool shouldFaceRight = _moveInput.x > 0;
            if (shouldFaceRight != _facingRight)
            {
                _facingRight = shouldFaceRight;
                if (_spriteTransform != null)
                {
                    Vector3 scale = _spriteTransform.localScale;
                    scale.x = _facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                    _spriteTransform.localScale = scale;
                }
            }
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            _rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }
}

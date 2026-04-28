using UnityEngine;

namespace PocketDungeons.Gameplay.Enemies
{
    /// <summary>
    /// Enemy projectile. Travels in a direction, damages player on hit, destroyed on wall contact.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 5f;
        [SerializeField] private LayerMask _wallLayer;

        private int _damage;
        private float _timer;

        public void Initialize(int damage, Vector2 direction)
        {
            _damage = damage;
            _timer = _lifetime;

            // Rotate sprite to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Hit player
            var playerHealth = other.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible)
            {
                playerHealth.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }

            // Hit wall
            if ((_wallLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

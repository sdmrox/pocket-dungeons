using UnityEngine;

namespace PocketDungeons.Gameplay.Loot
{
    public enum LootType
    {
        Gold,
        HealthPotion
    }

    /// <summary>
    /// Individual loot drop. Auto-collected when player enters radius.
    /// Gold uses magnet/auto-pull effect.
    /// </summary>
    public class LootDrop : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private LootType _type;
        [SerializeField] private int _value = 1;
        [SerializeField] private float _collectRadius = 0.5f;
        [SerializeField] private float _magnetRadius = 2f;
        [SerializeField] private float _magnetSpeed = 8f;
        [SerializeField] private float _bounceForce = 3f;
        [SerializeField] private float _lifetime = 30f;

        private Transform _player;
        private Rigidbody2D _rb;
        private bool _collected;
        private float _spawnBounceTimer = 0.3f;

        public LootType Type => _type;
        public int Value => _value;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Initialize(LootType type, int value, Transform player)
        {
            _type = type;
            _value = value;
            _player = player;

            // Bounce on spawn for satisfying feel
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            if (_rb != null)
                _rb.AddForce(randomDir * _bounceForce, ForceMode2D.Impulse);

            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            if (_collected || _player == null) return;

            _spawnBounceTimer -= Time.deltaTime;
            if (_spawnBounceTimer > 0f) return;

            float dist = Vector2.Distance(transform.position, _player.position);

            // Magnet pull for gold
            if (_type == LootType.Gold && dist < _magnetRadius)
            {
                Vector2 dir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                transform.position += (Vector3)(dir * _magnetSpeed * Time.deltaTime);
            }

            // Collect
            if (dist < _collectRadius)
            {
                Collect();
            }
        }

        private void Collect()
        {
            _collected = true;
            LootManager.Instance?.CollectLoot(this);
            Destroy(gameObject);
        }
    }
}

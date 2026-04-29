using System;
using UnityEngine;

namespace PocketDungeons.Gameplay.Loot
{
    /// <summary>
    /// Tracks collected loot for the current run. Spawns loot on enemy death.
    /// </summary>
    public class LootManager : MonoBehaviour
    {
        public static LootManager Instance { get; private set; }

        [Header("Loot Prefabs")]
        [SerializeField] private GameObject _goldPrefab;
        [SerializeField] private GameObject _healthPotionPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private float _spreadRadius = 0.5f;

        private int _goldCollected;
        private int _healthPotionsUsed;
        private Transform _player;

        public int GoldCollected => _goldCollected;
        public event Action<int> OnGoldChanged;
        public event Action<int> OnHealthPotionCollected;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(Transform player)
        {
            _player = player;
            _goldCollected = 0;
            _healthPotionsUsed = 0;
        }

        public void SpawnLoot(Vector3 position, int goldAmount, float healthPotionChance)
        {
            // Spawn gold
            for (int i = 0; i < goldAmount; i++)
            {
                SpawnDrop(LootType.Gold, 1, position);
            }

            // Chance to spawn health potion
            if (UnityEngine.Random.value < healthPotionChance)
            {
                SpawnDrop(LootType.HealthPotion, 20, position);
            }
        }

        private void SpawnDrop(LootType type, int value, Vector3 position)
        {
            GameObject prefab = type == LootType.Gold ? _goldPrefab : _healthPotionPrefab;
            if (prefab == null) return;

            Vector2 offset = UnityEngine.Random.insideUnitCircle * _spreadRadius;
            var go = Instantiate(prefab, position + (Vector3)offset, Quaternion.identity);
            var drop = go.GetComponent<LootDrop>();
            drop?.Initialize(type, value, _player);
        }

        public void CollectLoot(LootDrop drop)
        {
            switch (drop.Type)
            {
                case LootType.Gold:
                    _goldCollected += drop.Value;
                    OnGoldChanged?.Invoke(_goldCollected);
                    break;

                case LootType.HealthPotion:
                    _healthPotionsUsed++;
                    OnHealthPotionCollected?.Invoke(drop.Value);
                    break;
            }
        }
    }
}

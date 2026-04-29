using System.Collections.Generic;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.Dungeon
{
    /// <summary>
    /// Populates dungeon rooms with enemies and loot based on floor depth and biome.
    /// </summary>
    public class RoomPopulator : MonoBehaviour
    {
        [Header("Enemy Spawning")]
        [SerializeField] private int _baseEnemiesPerRoom = 3;
        [SerializeField] private float _enemiesPerFloorIncrease = 0.5f;
        [SerializeField] private int _maxEnemiesPerRoom = 8;
        [SerializeField] private float _spawnPadding = 1.5f;

        [Header("Loot")]
        [SerializeField] private GameObject _goldPrefab;
        [SerializeField] private GameObject _healthPotionPrefab;
        [SerializeField] [Range(0f, 1f)] private float _lootRoomChance = 0.3f;

        public List<EnemySpawnInfo> PopulateRoom(RectInt room, int floorDepth, EnemyData[] availableEnemies)
        {
            var spawns = new List<EnemySpawnInfo>();

            int enemyCount = Mathf.Min(
                Mathf.RoundToInt(_baseEnemiesPerRoom + _enemiesPerFloorIncrease * (floorDepth - 1)),
                _maxEnemiesPerRoom);

            if (availableEnemies == null || availableEnemies.Length == 0)
                return spawns;

            for (int i = 0; i < enemyCount; i++)
            {
                var enemyData = SelectWeightedEnemy(availableEnemies, floorDepth);
                var position = GetRandomPositionInRoom(room);

                spawns.Add(new EnemySpawnInfo
                {
                    Data = enemyData,
                    Position = position,
                    ScaledHealth = enemyData.GetScaledHealth(floorDepth),
                    ScaledDamage = enemyData.GetScaledDamage(floorDepth)
                });
            }

            return spawns;
        }

        private EnemyData SelectWeightedEnemy(EnemyData[] enemies, int floorDepth)
        {
            float totalWeight = 0f;
            var eligible = new List<EnemyData>();

            foreach (var enemy in enemies)
            {
                if (floorDepth >= enemy.MinFloorToSpawn)
                {
                    eligible.Add(enemy);
                    totalWeight += enemy.SpawnWeight;
                }
            }

            if (eligible.Count == 0)
                return enemies[0];

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var enemy in eligible)
            {
                cumulative += enemy.SpawnWeight;
                if (roll <= cumulative)
                    return enemy;
            }

            return eligible[eligible.Count - 1];
        }

        private Vector2 GetRandomPositionInRoom(RectInt room)
        {
            return new Vector2(
                Random.Range(room.x + _spawnPadding, room.x + room.width - _spawnPadding),
                Random.Range(room.y + _spawnPadding, room.y + room.height - _spawnPadding));
        }
    }

    public struct EnemySpawnInfo
    {
        public EnemyData Data;
        public Vector2 Position;
        public int ScaledHealth;
        public int ScaledDamage;
    }
}

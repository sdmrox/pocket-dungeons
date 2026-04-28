using System;
using UnityEngine;

namespace PocketDungeons.Data
{
    [Serializable]
    public struct LootEntry
    {
        public ScriptableObject Item;
        public Rarity MinRarity;
        [Range(0f, 100f)]
        public float Weight;
    }

    [CreateAssetMenu(fileName = "NewLootTable", menuName = "Pocket Dungeons/Data/Loot Table")]
    public class LootTableData : ScriptableObject
    {
        [Header("Loot Entries")]
        public LootEntry[] Entries;

        [Header("Drop Settings")]
        [Range(0, 5)]
        public int MinDrops = 1;
        [Range(0, 5)]
        public int MaxDrops = 3;
        [Range(0f, 1f)]
        public float NothingDropChance = 0.1f;

        public LootEntry Roll()
        {
            if (Entries == null || Entries.Length == 0)
                return default;

            float totalWeight = 0f;
            foreach (var entry in Entries)
                totalWeight += entry.Weight;

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var entry in Entries)
            {
                cumulative += entry.Weight;
                if (roll <= cumulative)
                    return entry;
            }

            return Entries[^1];
        }
    }
}

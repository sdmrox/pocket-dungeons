using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Meta.Bestiary
{
    [Serializable]
    public class BestiaryEntry
    {
        public string EnemyId;
        public string DisplayName;
        public string Lore;
        public int KillCount;
        public bool IsDiscovered;

        public int LoreTier => KillCount switch
        {
            >= 100 => 3,
            >= 25 => 2,
            >= 1 => 1,
            _ => 0
        };

        public float DamageBonus => KillCount switch
        {
            >= 100 => 0.10f,
            >= 50 => 0.05f,
            >= 10 => 0.02f,
            _ => 0f
        };
    }

    public class BestiaryManager : MonoBehaviour
    {
        public static BestiaryManager Instance { get; private set; }

        private readonly Dictionary<string, BestiaryEntry> _entries = new();

        public event Action<string, int> OnEnemyKillRecorded;
        public event Action<string> OnNewDiscovery;
        public event Action<string, int> OnLoreTierUnlocked;

        private void Awake()
        {
            Instance = this;
        }

        public void RecordKill(string enemyId, string displayName)
        {
            if (!_entries.TryGetValue(enemyId, out var entry))
            {
                entry = new BestiaryEntry
                {
                    EnemyId = enemyId,
                    DisplayName = displayName,
                    IsDiscovered = true
                };
                _entries[enemyId] = entry;
                OnNewDiscovery?.Invoke(enemyId);
            }

            int previousTier = entry.LoreTier;
            entry.KillCount++;
            OnEnemyKillRecorded?.Invoke(enemyId, entry.KillCount);

            if (entry.LoreTier > previousTier)
                OnLoreTierUnlocked?.Invoke(enemyId, entry.LoreTier);
        }

        public void SetLore(string enemyId, string lore)
        {
            if (_entries.TryGetValue(enemyId, out var entry))
                entry.Lore = lore;
        }

        public BestiaryEntry GetEntry(string enemyId)
        {
            return _entries.TryGetValue(enemyId, out var entry) ? entry : null;
        }

        public float GetDamageBonus(string enemyId)
        {
            return _entries.TryGetValue(enemyId, out var entry) ? entry.DamageBonus : 0f;
        }

        public int GetKillCount(string enemyId)
        {
            return _entries.TryGetValue(enemyId, out var entry) ? entry.KillCount : 0;
        }

        public int DiscoveredCount => _entries.Count;
        public int TotalKills
        {
            get
            {
                int total = 0;
                foreach (var entry in _entries.Values)
                    total += entry.KillCount;
                return total;
            }
        }

        public float GetCompletionPercentage(int totalEnemyTypes)
        {
            if (totalEnemyTypes <= 0) return 0f;
            return (float)_entries.Count / totalEnemyTypes;
        }

        public void LoadState(Dictionary<string, BestiaryEntry> entries)
        {
            _entries.Clear();
            if (entries != null)
            {
                foreach (var kvp in entries)
                    _entries[kvp.Key] = kvp.Value;
            }
        }

        public Dictionary<string, BestiaryEntry> GetAllEntries() => new(_entries);
    }
}

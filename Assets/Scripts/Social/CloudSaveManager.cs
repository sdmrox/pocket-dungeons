using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Social
{
    /// <summary>
    /// CloudKit save synchronization. Saves player progress to iCloud.
    /// Handles conflict resolution with last-write-wins + merge for collections.
    /// Auto-syncs on app launch, manual sync available.
    /// </summary>
    public class CloudSaveManager : MonoBehaviour
    {
        public static CloudSaveManager Instance { get; private set; }

        [Serializable]
        public class SaveData
        {
            public int TotalGold;
            public int Gems;
            public Dictionary<string, int> TownUpgradeLevels = new();
            public List<string> UnlockedHeroes = new();
            public List<string> UnlockedCosmetics = new();
            public int PrestigeLevel;
            public int TotalFloorsCleared;
            public int TotalEnemiesKilled;
            public int TotalRunsPlayed;
            public int BattlePassLevel;
            public int BattlePassXP;
            public bool IsBattlePassPremium;
            public List<string> CompletedAchievements = new();
            public Dictionary<string, string> ABTestAssignments = new();
            public string LastSaveTimestamp;
        }

        private SaveData _localData = new();
        private bool _isSyncing;

        public SaveData LocalData => _localData;
        public bool IsSyncing => _isSyncing;

        public event Action OnSaveCompleted;
        public event Action OnLoadCompleted;
        public event Action<string> OnSyncError;

        private void Awake()
        {
            Instance = this;
        }

        public void SaveToCloud()
        {
            if (_isSyncing) return;

            _isSyncing = true;
            _localData.LastSaveTimestamp = DateTime.UtcNow.ToString("O");

            string json = JsonUtility.ToJson(_localData);

            Debug.Log($"[CloudSave] Saving {json.Length} bytes to CloudKit");
            // TODO: CKDatabase.save(CKRecord)

            _isSyncing = false;
            OnSaveCompleted?.Invoke();
        }

        public void LoadFromCloud()
        {
            if (_isSyncing) return;

            _isSyncing = true;
            Debug.Log("[CloudSave] Loading from CloudKit...");
            // TODO: CKDatabase.fetch(CKRecord.ID)

            _isSyncing = false;
            OnLoadCompleted?.Invoke();
        }

        public void SaveLocal()
        {
            string json = JsonUtility.ToJson(_localData);
            PlayerPrefs.SetString("save_data", json);
            PlayerPrefs.Save();
        }

        public void LoadLocal()
        {
            string json = PlayerPrefs.GetString("save_data", "");
            if (!string.IsNullOrEmpty(json))
            {
                _localData = JsonUtility.FromJson<SaveData>(json);
            }
        }

        public SaveData MergeConflict(SaveData local, SaveData cloud)
        {
            // Last-write-wins for scalar values
            SaveData winner = string.Compare(local.LastSaveTimestamp, cloud.LastSaveTimestamp) > 0
                ? local : cloud;

            // Union merge for collections
            var merged = new SaveData
            {
                TotalGold = Mathf.Max(local.TotalGold, cloud.TotalGold),
                Gems = Mathf.Max(local.Gems, cloud.Gems),
                PrestigeLevel = Mathf.Max(local.PrestigeLevel, cloud.PrestigeLevel),
                TotalFloorsCleared = Mathf.Max(local.TotalFloorsCleared, cloud.TotalFloorsCleared),
                TotalEnemiesKilled = Mathf.Max(local.TotalEnemiesKilled, cloud.TotalEnemiesKilled),
                TotalRunsPlayed = Mathf.Max(local.TotalRunsPlayed, cloud.TotalRunsPlayed),
                BattlePassLevel = Mathf.Max(local.BattlePassLevel, cloud.BattlePassLevel),
                BattlePassXP = winner.BattlePassXP,
                IsBattlePassPremium = local.IsBattlePassPremium || cloud.IsBattlePassPremium,
                LastSaveTimestamp = DateTime.UtcNow.ToString("O")
            };

            // Merge town upgrades (take max level per building)
            merged.TownUpgradeLevels = new Dictionary<string, int>(local.TownUpgradeLevels);
            foreach (var kvp in cloud.TownUpgradeLevels)
            {
                if (merged.TownUpgradeLevels.TryGetValue(kvp.Key, out int localLevel))
                    merged.TownUpgradeLevels[kvp.Key] = Mathf.Max(localLevel, kvp.Value);
                else
                    merged.TownUpgradeLevels[kvp.Key] = kvp.Value;
            }

            // Union merge for unlocked items
            merged.UnlockedHeroes = MergeLists(local.UnlockedHeroes, cloud.UnlockedHeroes);
            merged.UnlockedCosmetics = MergeLists(local.UnlockedCosmetics, cloud.UnlockedCosmetics);
            merged.CompletedAchievements = MergeLists(local.CompletedAchievements, cloud.CompletedAchievements);

            return merged;
        }

        private List<string> MergeLists(List<string> a, List<string> b)
        {
            var set = new HashSet<string>(a);
            foreach (var item in b)
                set.Add(item);
            return new List<string>(set);
        }
    }
}

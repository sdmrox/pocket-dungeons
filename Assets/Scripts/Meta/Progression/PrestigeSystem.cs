using System;
using UnityEngine;

namespace PocketDungeons.Meta.Progression
{
    /// <summary>
    /// Three-layer progression system:
    /// Layer 1: Within-run (power-ups, floor depth)
    /// Layer 2: Between-run (town upgrades, hero XP, gear)
    /// Layer 3: Long-term (prestige, mastery, Battle Pass)
    /// This class handles Layer 3 prestige resets.
    /// </summary>
    public class PrestigeSystem : MonoBehaviour
    {
        public static PrestigeSystem Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private int _prestigeUnlockFloor = 50;
        [SerializeField] private float _prestigeBonusPerLevel = 0.05f;
        [SerializeField] private int _maxPrestigeLevel = 10;

        private int _currentPrestigeLevel;
        private int _totalFloorsEverCleared;
        private int _totalEnemiesEverKilled;

        public int PrestigeLevel => _currentPrestigeLevel;
        public float PrestigeBonus => _currentPrestigeLevel * _prestigeBonusPerLevel;
        public bool CanPrestige => _totalFloorsEverCleared >= _prestigeUnlockFloor * (_currentPrestigeLevel + 1);
        public int TotalFloorsEverCleared => _totalFloorsEverCleared;
        public int TotalEnemiesEverKilled => _totalEnemiesEverKilled;

        public event Action<int> OnPrestige; // new prestige level

        private void Awake()
        {
            Instance = this;
        }

        public void RecordRunStats(int floorsCleared, int enemiesKilled)
        {
            _totalFloorsEverCleared += floorsCleared;
            _totalEnemiesEverKilled += enemiesKilled;
        }

        public bool TryPrestige()
        {
            if (!CanPrestige || _currentPrestigeLevel >= _maxPrestigeLevel)
                return false;

            _currentPrestigeLevel++;

            // Reset town upgrades but keep prestige bonus
            // Town reset handled by TownManager.Reset() called externally

            OnPrestige?.Invoke(_currentPrestigeLevel);
            return true;
        }

        public void LoadState(int prestigeLevel, int totalFloors, int totalEnemies)
        {
            _currentPrestigeLevel = prestigeLevel;
            _totalFloorsEverCleared = totalFloors;
            _totalEnemiesEverKilled = totalEnemies;
        }
    }
}

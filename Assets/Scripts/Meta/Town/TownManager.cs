using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Meta.Town
{
    /// <summary>
    /// Manages persistent town upgrades between runs.
    /// Gold earned in runs is spent here for permanent stat bonuses.
    /// </summary>
    public class TownManager : MonoBehaviour
    {
        public static TownManager Instance { get; private set; }

        [SerializeField] private TownUpgradeData[] _allUpgrades;

        private readonly Dictionary<UpgradeType, int> _upgradeLevels = new();
        private int _playerGold;

        public int PlayerGold => _playerGold;
        public event Action<int> OnGoldChanged;
        public event Action<UpgradeType, int> OnUpgradePurchased;

        private void Awake()
        {
            Instance = this;

            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                _upgradeLevels[type] = 0;
            }
        }

        public void AddGold(int amount)
        {
            _playerGold += amount;
            OnGoldChanged?.Invoke(_playerGold);
        }

        public bool TryUpgrade(TownUpgradeData upgrade)
        {
            int currentLevel = GetUpgradeLevel(upgrade.Type);
            if (currentLevel >= upgrade.MaxLevel) return false;

            int cost = upgrade.GetCost(currentLevel);
            if (_playerGold < cost) return false;

            _playerGold -= cost;
            _upgradeLevels[upgrade.Type] = currentLevel + 1;

            OnGoldChanged?.Invoke(_playerGold);
            OnUpgradePurchased?.Invoke(upgrade.Type, currentLevel + 1);
            return true;
        }

        public int GetUpgradeLevel(UpgradeType type)
        {
            return _upgradeLevels.TryGetValue(type, out int level) ? level : 0;
        }

        public float GetUpgradeBonus(TownUpgradeData upgrade)
        {
            return upgrade.GetBonus(GetUpgradeLevel(upgrade.Type));
        }

        public TownBonuses GetAllBonuses()
        {
            var bonuses = new TownBonuses();

            foreach (var upgrade in _allUpgrades)
            {
                float bonus = GetUpgradeBonus(upgrade);
                switch (upgrade.Type)
                {
                    case UpgradeType.Blacksmith: bonuses.DamageBonus += bonus; break;
                    case UpgradeType.Armory: bonuses.HealthBonus += (int)bonus; break;
                    case UpgradeType.Alchemist: bonuses.PotionDropBonus += bonus; break;
                    case UpgradeType.Treasury: bonuses.GoldFindBonus += bonus; break;
                    case UpgradeType.Enchanter: bonuses.CritChanceBonus += bonus; break;
                    case UpgradeType.Cartographer: bonuses.MapRevealBonus += bonus; break;
                }
            }

            return bonuses;
        }

        public void LoadState(Dictionary<UpgradeType, int> levels, int gold)
        {
            _playerGold = gold;
            foreach (var kvp in levels)
                _upgradeLevels[kvp.Key] = kvp.Value;
        }
    }

    [Serializable]
    public class TownBonuses
    {
        public float DamageBonus;
        public int HealthBonus;
        public float PotionDropBonus;
        public float GoldFindBonus;
        public float CritChanceBonus;
        public float MapRevealBonus;
    }
}

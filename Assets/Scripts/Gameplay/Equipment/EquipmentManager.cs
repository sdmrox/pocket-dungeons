using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.Equipment
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; }

        private readonly Dictionary<EquipmentSlot, EquipmentData> _equipped = new();
        private readonly List<EquipmentData> _inventory = new();

        public IReadOnlyDictionary<EquipmentSlot, EquipmentData> Equipped => _equipped;
        public IReadOnlyList<EquipmentData> Inventory => _inventory;

        public event Action<EquipmentSlot, EquipmentData> OnEquipped;
        public event Action<EquipmentSlot, EquipmentData> OnUnequipped;
        public event Action<EquipmentData> OnItemAdded;

        private void Awake()
        {
            Instance = this;
        }

        public bool Equip(EquipmentData item)
        {
            if (item == null) return false;

            if (_equipped.TryGetValue(item.Slot, out var current))
            {
                _inventory.Add(current);
                OnUnequipped?.Invoke(item.Slot, current);
            }

            _equipped[item.Slot] = item;
            _inventory.Remove(item);
            OnEquipped?.Invoke(item.Slot, item);
            return true;
        }

        public bool Unequip(EquipmentSlot slot)
        {
            if (!_equipped.TryGetValue(slot, out var item)) return false;

            _equipped.Remove(slot);
            _inventory.Add(item);
            OnUnequipped?.Invoke(slot, item);
            return true;
        }

        public void AddToInventory(EquipmentData item)
        {
            if (item == null) return;
            _inventory.Add(item);
            OnItemAdded?.Invoke(item);
        }

        public bool RemoveFromInventory(EquipmentData item)
        {
            return _inventory.Remove(item);
        }

        public EquipmentStats GetTotalStats()
        {
            var stats = new EquipmentStats();
            foreach (var item in _equipped.Values)
            {
                stats.BonusDamage += item.BonusDamage;
                stats.BonusHealth += item.BonusHealth;
                stats.BonusCritChance += item.BonusCritChance;
                stats.BonusMoveSpeed += item.BonusMoveSpeed;
                stats.BonusAttackSpeed += item.BonusAttackSpeed;
                stats.BonusDefense += item.BonusDefense;
            }

            ApplySetBonuses(stats);
            return stats;
        }

        private void ApplySetBonuses(EquipmentStats stats)
        {
            var setCounts = new Dictionary<SetBonus, int>();
            foreach (var item in _equipped.Values)
            {
                if (item.SetType == SetBonus.None) continue;
                setCounts.TryGetValue(item.SetType, out int count);
                setCounts[item.SetType] = count + 1;
            }

            foreach (var kvp in setCounts)
            {
                var setData = _equipped.Values.FirstOrDefault(e => e.SetType == kvp.Key);
                if (setData == null || kvp.Value < setData.SetPiecesRequired) continue;

                switch (kvp.Key)
                {
                    case SetBonus.Warrior:
                        stats.BonusHealth += Mathf.RoundToInt(stats.BonusHealth * 0.15f);
                        stats.BonusDamage += Mathf.RoundToInt(stats.BonusDamage * 0.10f);
                        break;
                    case SetBonus.Shadow:
                        stats.BonusCritChance += 0.20f;
                        stats.BonusMoveSpeed += 0.10f;
                        break;
                    case SetBonus.Arcane:
                        stats.AbilityDamageMultiplier = 1.25f;
                        stats.CooldownReduction = 0.15f;
                        break;
                    case SetBonus.Guardian:
                        stats.BonusDefense += 0.30f;
                        stats.BonusHealth += Mathf.RoundToInt(stats.BonusHealth * 0.10f);
                        break;
                    case SetBonus.Fortune:
                        stats.GoldFindBonus = 0.25f;
                        stats.LootQualityBonus = 0.15f;
                        break;
                }
            }
        }

        public EquipmentData GetEquipped(EquipmentSlot slot)
        {
            return _equipped.TryGetValue(slot, out var item) ? item : null;
        }

        public int GetInventoryCount() => _inventory.Count;

        public void ClearAll()
        {
            _equipped.Clear();
            _inventory.Clear();
        }
    }

    [Serializable]
    public class EquipmentStats
    {
        public int BonusDamage;
        public int BonusHealth;
        public float BonusCritChance;
        public float BonusMoveSpeed;
        public float BonusAttackSpeed;
        public float BonusDefense;
        public float AbilityDamageMultiplier = 1f;
        public float CooldownReduction;
        public float GoldFindBonus;
        public float LootQualityBonus;
    }
}

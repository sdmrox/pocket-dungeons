using UnityEngine;

namespace PocketDungeons.Data
{
    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Accessory
    }

    public enum SetBonus
    {
        None,
        Warrior,    // +15% HP, +10% damage
        Shadow,     // +20% crit chance, +10% dodge
        Arcane,     // +25% ability damage, -15% cooldown
        Guardian,   // +30% defense, +10% HP
        Fortune     // +25% gold find, +15% loot quality
    }

    [CreateAssetMenu(fileName = "NewEquipment", menuName = "Pocket Dungeons/Data/Equipment")]
    public class EquipmentData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        public string Description;
        public Sprite Icon;
        public EquipmentSlot Slot;
        public Rarity Rarity;

        [Header("Stats")]
        public int BonusDamage;
        public int BonusHealth;
        public float BonusCritChance;
        public float BonusMoveSpeed;
        public float BonusAttackSpeed;
        public float BonusDefense;

        [Header("Set")]
        public SetBonus SetType = SetBonus.None;
        public int SetPiecesRequired = 2;

        [Header("Crafting")]
        public int SellValue = 10;
        public int CraftCost = 50;
        public EquipmentData[] CraftIngredients;

        public Color RarityColor => Rarity switch
        {
            Rarity.Common => Color.white,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.blue,
            Rarity.Epic => new Color(0.6f, 0f, 0.8f),
            Rarity.Legendary => new Color(1f, 0.84f, 0f),
            _ => Color.white
        };
    }
}

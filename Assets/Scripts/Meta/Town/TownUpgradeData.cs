using UnityEngine;

namespace PocketDungeons.Meta.Town
{
    public enum UpgradeType
    {
        Blacksmith,     // +damage per level
        Armory,         // +max HP per level
        Alchemist,      // Better potion drops
        Treasury,       // +gold find %
        TrainingGrounds,// Unlock new heroes
        Enchanter,      // +crit chance
        Cartographer,   // Reveal map, better room layouts
        Tavern          // +starting power-up
    }

    [CreateAssetMenu(fileName = "NewTownUpgrade", menuName = "Pocket Dungeons/Meta/Town Upgrade")]
    public class TownUpgradeData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        [TextArea(1, 3)]
        public string Description;
        public Sprite Icon;
        public UpgradeType Type;

        [Header("Levels")]
        public int MaxLevel = 10;
        public int[] GoldCostPerLevel;
        public float[] BonusPerLevel;

        [Header("Unlock")]
        public int RequiredTownHallLevel = 1;

        public int GetCost(int currentLevel)
        {
            if (GoldCostPerLevel == null || currentLevel >= GoldCostPerLevel.Length)
                return int.MaxValue;
            return GoldCostPerLevel[currentLevel];
        }

        public float GetBonus(int currentLevel)
        {
            if (BonusPerLevel == null || currentLevel <= 0 || currentLevel > BonusPerLevel.Length)
                return 0f;
            return BonusPerLevel[currentLevel - 1];
        }
    }
}

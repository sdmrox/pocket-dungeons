using UnityEngine;

namespace PocketDungeons.Data
{
    public enum PowerUpCategory
    {
        Offensive,  // Damage, crit, attack speed
        Defensive,  // Health, armor, regen
        Utility,    // Move speed, gold bonus, luck
        Special     // Unique synergy effects
    }

    [CreateAssetMenu(fileName = "NewPowerUp", menuName = "Pocket Dungeons/Data/Power-Up")]
    public class PowerUpData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;
        public Sprite Icon;
        public Rarity Rarity;
        public PowerUpCategory Category;

        [Header("Stat Modifiers")]
        public float DamageMultiplier = 1f;
        public float AttackSpeedMultiplier = 1f;
        public float MoveSpeedMultiplier = 1f;
        public int BonusHealth = 0;
        public float CritChanceBonus = 0f;
        public float GoldMultiplier = 1f;

        [Header("Stacking")]
        public bool Stackable = true;
        public int MaxStacks = 5;

        [Header("Synergies")]
        [Tooltip("Power-ups that enhance this one when combined")]
        public PowerUpData[] SynergyWith;
        [TextArea(1, 2)]
        public string SynergyDescription;
    }
}

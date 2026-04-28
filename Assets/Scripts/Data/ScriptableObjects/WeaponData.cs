using UnityEngine;

namespace PocketDungeons.Data
{
    public enum WeaponType
    {
        Sword,
        Bow,
        Staff,
        Dagger,
        Hammer,
        Spear
    }

    public enum Rarity
    {
        Common,     // 60% drop weight
        Uncommon,   // 25%
        Rare,       // 10%
        Epic,       // 4%
        Legendary   // 1%
    }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Pocket Dungeons/Data/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        public Sprite Icon;
        public WeaponType Type;
        public Rarity Rarity;

        [Header("Stats")]
        public int BaseDamage = 5;
        public float AttackSpeed = 1f;
        public float Range = 1.5f;

        [Header("Special")]
        [Range(0f, 1f)]
        public float CritChance = 0.05f;
        public float CritMultiplier = 2f;
        public float KnockbackForce = 0f;

        [Header("Visuals")]
        public Color RarityColor => Rarity switch
        {
            Rarity.Common => Color.white,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.blue,
            Rarity.Epic => new Color(0.6f, 0f, 0.8f), // Purple
            Rarity.Legendary => new Color(1f, 0.84f, 0f), // Gold
            _ => Color.white
        };
    }
}

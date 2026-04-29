using UnityEngine;

namespace PocketDungeons.Data
{
    public enum HeroClass
    {
        Warrior,    // Melee — sword + shield bash
        Archer,     // Ranged — bow + rain of arrows
        Mage        // Magic — staff + meteor strike
    }

    [CreateAssetMenu(fileName = "NewHero", menuName = "Pocket Dungeons/Data/Hero")]
    public class HeroData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        public HeroClass Class;
        public Sprite Portrait;
        public Sprite InGameSprite;
        public RuntimeAnimatorController Animator;

        [Header("Base Stats")]
        public int BaseHealth = 100;
        public int BaseDamage = 10;
        public float MoveSpeed = 5f;
        public float AttackRange = 1.5f;
        public float AttackSpeed = 1f;

        [Header("Ability")]
        public string AbilityName;
        [TextArea(1, 3)]
        public string AbilityDescription;
        public float AbilityCooldown = 8f;
        public Sprite AbilityIcon;

        [Header("Unlock")]
        public bool UnlockedByDefault = false;
        [TextArea(1, 2)]
        public string UnlockConditionDescription;

        [Header("Leveling")]
        public float HealthPerLevel = 5f;
        public float DamagePerLevel = 1f;
        public int MaxLevel = 50;
    }
}

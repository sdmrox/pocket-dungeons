using UnityEngine;

namespace PocketDungeons.Data
{
    public enum EnemyAIType
    {
        MeleeChase,     // Slime — walk toward player, melee attack
        RangedStationary, // Skeleton Archer — stand still, shoot projectiles
        ErraticRush,    // Bat — random movement with bursts toward player
        TankCharge,     // Golem — slow, charges in straight line
        Phasing,        // Ghost — phases through walls, teleports
        Exploder,       // Bomber — runs toward player, explodes on contact
        Support,        // Healer — heals nearby enemies
        Shielded        // Shield Knight — blocks frontal attacks, vulnerable from behind
    }

    [CreateAssetMenu(fileName = "NewEnemy", menuName = "Pocket Dungeons/Data/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        public Sprite Icon;
        public RuntimeAnimatorController Animator;

        [Header("Stats")]
        public int BaseHealth = 10;
        public int BaseDamage = 3;
        public float MoveSpeed = 2f;
        public float AttackRange = 1f;
        public float AttackCooldown = 1.5f;

        [Header("Scaling (per floor)")]
        public float HealthScalePerFloor = 0.15f;
        public float DamageScalePerFloor = 0.1f;

        [Header("AI")]
        public EnemyAIType AIType;
        public float DetectionRange = 8f;
        public float IdleWanderRadius = 3f;

        [Header("Loot")]
        public int MinGoldDrop = 1;
        public int MaxGoldDrop = 5;
        [Range(0f, 1f)]
        public float HealthPotionDropChance = 0.1f;

        [Header("Spawn")]
        public int MinFloorToSpawn = 1;
        [Range(0f, 1f)]
        public float SpawnWeight = 1f;

        public int GetScaledHealth(int floor)
        {
            return Mathf.RoundToInt(BaseHealth * (1 + HealthScalePerFloor * (floor - 1)));
        }

        public int GetScaledDamage(int floor)
        {
            return Mathf.RoundToInt(BaseDamage * (1 + DamageScalePerFloor * (floor - 1)));
        }
    }
}

using UnityEngine;

namespace PocketDungeons.Gameplay.Balance
{
    /// <summary>
    /// Centralized balance configuration. All tuneable game parameters.
    /// Remote-configurable via Firebase for live tuning without app update.
    /// </summary>
    [CreateAssetMenu(fileName = "BalanceConfig", menuName = "Pocket Dungeons/Config/Balance")]
    public class BalanceConfig : ScriptableObject
    {
        [Header("Player Base Stats")]
        public int WarriorBaseHP = 120;
        public int WarriorBaseDamage = 15;
        public float WarriorAttackSpeed = 0.8f;
        public int ArcherBaseHP = 80;
        public int ArcherBaseDamage = 12;
        public float ArcherAttackSpeed = 0.5f;
        public int MageBaseHP = 70;
        public int MageBaseDamage = 18;
        public float MageAttackSpeed = 1.0f;

        [Header("Combat")]
        public float BaseCritChance = 0.05f;
        public float CritMultiplier = 2f;
        public float DodgeDuration = 0.25f;
        public float DodgeCooldown = 2f;
        public float DodgeInvincibility = 0.35f;

        [Header("Dungeon Scaling")]
        public float EnemyHPScalePerFloor = 0.15f;
        public float EnemyDamageScalePerFloor = 0.10f;
        public float EnemyCountScalePerFloor = 0.5f;
        public int BossFloorInterval = 10;

        [Header("Loot")]
        public float GoldDropChance = 0.7f;
        public float PotionDropChance = 0.15f;
        public float GoldMagnetRadius = 2f;
        public float GoldMagnetSpeed = 8f;

        [Header("Economy")]
        public int BaseGoldPerEnemy = 5;
        public float GoldPerFloorMultiplier = 1.1f;
        public int StarterPackGold = 500;
        public int StarterPackGems = 100;

        [Header("Progression")]
        public int XPPerEnemyKill = 10;
        public int XPPerFloorClear = 50;
        public int XPPerBossKill = 200;
        public float XPScalePerLevel = 1.15f;

        [Header("Session Targets")]
        public float TargetRunDurationSeconds = 150f; // 2.5 min average
        public float MinRunDurationSeconds = 90f;
        public float MaxRunDurationSeconds = 240f;

        [Header("Difficulty Curve")]
        public AnimationCurve DifficultyRamp;
        public float EasyModeMultiplier = 0.7f;
        public float HardModeMultiplier = 1.3f;

        public int GetScaledEnemyHP(int baseHP, int floor)
        {
            return Mathf.RoundToInt(baseHP * (1f + EnemyHPScalePerFloor * (floor - 1)));
        }

        public int GetScaledEnemyDamage(int baseDamage, int floor)
        {
            return Mathf.RoundToInt(baseDamage * (1f + EnemyDamageScalePerFloor * (floor - 1)));
        }

        public int GetEnemyCount(int baseCount, int floor)
        {
            return Mathf.Min(baseCount + Mathf.RoundToInt(EnemyCountScalePerFloor * (floor - 1)), 15);
        }

        public int GetXPForLevel(int level)
        {
            return Mathf.RoundToInt(100 * Mathf.Pow(XPScalePerLevel, level - 1));
        }
    }
}

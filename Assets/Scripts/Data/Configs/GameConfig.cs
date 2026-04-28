using UnityEngine;

namespace PocketDungeons.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Pocket Dungeons/Config/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Player Defaults")]
        public int StartingGold = 0;
        public int StartingGems = 0;

        [Header("Dungeon")]
        public int DungeonGridWidth = 40;
        public int DungeonGridHeight = 40;
        public int MinRoomSize = 6;
        public int MaxRoomSize = 12;
        public int CorridorWidth = 2;
        public int FloorsPerBoss = 10;

        [Header("Combat")]
        public float DodgeCooldown = 2f;
        public float DodgeDuration = 0.3f;
        public float InvincibilityDuration = 0.5f;

        [Header("Juice")]
        public float ScreenShakeMinDuration = 0.1f;
        public float ScreenShakeMaxDuration = 0.3f;
        public float ScreenShakeMinAmplitude = 2f;
        public float ScreenShakeMaxAmplitude = 8f;
        public int HitStopFrames = 2;
        public int CritHitStopFrames = 4;
        public float LastKillSlowMoDuration = 0.3f;
        public float LastKillSlowMoScale = 0.3f;
        public int MaxVisibleDamageNumbers = 3;

        [Header("Loot")]
        public float GoldAutoCollectRadius = 2f;
        public float LootDropSpreadRadius = 0.5f;

        [Header("Performance Targets")]
        public int TargetFrameRate = 60;
        public int MaxEnemiesOnScreen = 50;
        public int MaxProjectilesOnScreen = 100;
        public int MaxVFXOnScreen = 30;

        [Header("Monetization")]
        public int FreeRevivesPerDay = 0;
        public int GemReviveCost = 50;
        public int MaxRewardedAdsPerDay = 6;
        public int StarterPackUnlockLevel = 5;
    }
}

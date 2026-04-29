namespace PocketDungeons.Analytics
{
    /// <summary>
    /// Standard analytics event names per the business analytics skill taxonomy.
    /// All events use snake_case naming for Firebase/Amplitude compatibility.
    /// </summary>
    public static class AnalyticsEventNames
    {
        // Session
        public const string SessionStart = "session_start";
        public const string SessionEnd = "session_end";

        // Engagement
        public const string RunStarted = "run_started";
        public const string RunCompleted = "run_completed";
        public const string FloorCleared = "floor_cleared";
        public const string BossEncountered = "boss_encountered";
        public const string BossDefeated = "boss_defeated";
        public const string PlayerDeath = "player_death";

        // Combat
        public const string EnemyKilled = "enemy_killed";
        public const string DamageDealt = "damage_dealt";
        public const string DamageTaken = "damage_taken";
        public const string AbilityUsed = "ability_used";
        public const string DodgeUsed = "dodge_used";
        public const string PowerUpSelected = "powerup_selected";

        // Loot
        public const string LootCollected = "loot_collected";
        public const string GoldEarned = "gold_earned";

        // Meta
        public const string TownUpgradePurchased = "town_upgrade_purchased";
        public const string QuestCompleted = "quest_completed";
        public const string BattlePassLevelUp = "battle_pass_level_up";
        public const string BattlePassRewardClaimed = "bp_reward_claimed";
        public const string HeroSelected = "hero_selected";
        public const string HeroUnlocked = "hero_unlocked";

        // Monetization
        public const string IAPPurchase = "iap_purchase";
        public const string IAPFailed = "iap_failed";
        public const string AdWatched = "ad_watched";
        public const string AdFailed = "ad_failed";
        public const string StoreOpened = "store_opened";

        // Onboarding
        public const string TutorialStepCompleted = "tutorial_step_completed";
        public const string TutorialSkipped = "tutorial_skipped";

        // Retention
        public const string DailyLogin = "daily_login";
        public const string ConsecutiveLoginDays = "consecutive_login_days";

        // Technical
        public const string LoadTime = "load_time";
        public const string FPSDrop = "fps_drop";
        public const string CrashRecovery = "crash_recovery";
    }
}

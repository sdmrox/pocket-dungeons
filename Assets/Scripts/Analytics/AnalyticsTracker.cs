using System.Collections.Generic;
using UnityEngine;
using PocketDungeons.Core.Services;

namespace PocketDungeons.Analytics
{
    /// <summary>
    /// Central analytics dispatcher. Wraps IAnalyticsService with typed helper methods.
    /// All game systems call through here rather than directly to the service.
    /// </summary>
    public class AnalyticsTracker : MonoBehaviour
    {
        public static AnalyticsTracker Instance { get; private set; }

        private IAnalyticsService _analytics;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Services.TryGet(out _analytics);
        }

        public void TrackRunStarted(string heroName, int heroLevel)
        {
            Log(AnalyticsEventNames.RunStarted, new()
            {
                { "hero", heroName },
                { "hero_level", heroLevel }
            });
        }

        public void TrackRunCompleted(int floorsCleared, int enemiesKilled, int goldCollected,
            float timeSeconds, bool defeatedBoss, string deathCause)
        {
            Log(AnalyticsEventNames.RunCompleted, new()
            {
                { "floors_cleared", floorsCleared },
                { "enemies_killed", enemiesKilled },
                { "gold_collected", goldCollected },
                { "time_seconds", timeSeconds },
                { "defeated_boss", defeatedBoss },
                { "death_cause", deathCause ?? "none" }
            });
        }

        public void TrackFloorCleared(int floor, float timeSeconds, int enemiesKilled)
        {
            Log(AnalyticsEventNames.FloorCleared, new()
            {
                { "floor", floor },
                { "time_seconds", timeSeconds },
                { "enemies_killed", enemiesKilled }
            });
        }

        public void TrackPlayerDeath(int floor, string enemyType, int healthAtDeath)
        {
            Log(AnalyticsEventNames.PlayerDeath, new()
            {
                { "floor", floor },
                { "enemy_type", enemyType },
                { "health_at_death", healthAtDeath }
            });
        }

        public void TrackPowerUpSelected(string powerUpName, string rarity, int floorSelected)
        {
            Log(AnalyticsEventNames.PowerUpSelected, new()
            {
                { "powerup_name", powerUpName },
                { "rarity", rarity },
                { "floor", floorSelected }
            });
        }

        public void TrackTownUpgrade(string upgradeType, int newLevel, int goldSpent)
        {
            Log(AnalyticsEventNames.TownUpgradePurchased, new()
            {
                { "upgrade_type", upgradeType },
                { "new_level", newLevel },
                { "gold_spent", goldSpent }
            });
        }

        public void TrackIAPPurchase(string productId, string productType, string price)
        {
            Log(AnalyticsEventNames.IAPPurchase, new()
            {
                { "product_id", productId },
                { "product_type", productType },
                { "price", price }
            });
        }

        public void TrackAdWatched(string placement, int adsTodayCount)
        {
            Log(AnalyticsEventNames.AdWatched, new()
            {
                { "placement", placement },
                { "ads_today", adsTodayCount }
            });
        }

        public void TrackFPSDrop(float fps, string scene)
        {
            Log(AnalyticsEventNames.FPSDrop, new()
            {
                { "fps", fps },
                { "scene", scene }
            });
        }

        private void Log(string eventName, Dictionary<string, object> parameters = null)
        {
            _analytics?.LogEvent(eventName, parameters);
        }
    }
}

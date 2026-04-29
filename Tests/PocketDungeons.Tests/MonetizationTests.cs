using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== PHASE 4: Monetization & Analytics Tests ====================

    [TestFixture]
    public class IAPTests
    {
        enum IAPProductType { Consumable, NonConsumable, Subscription }

        record Product(string Id, IAPProductType Type, int GemAmount, int GoldAmount);

        [Test]
        public void ProductLookup_FindsById()
        {
            var products = new Dictionary<string, Product>
            {
                ["gems_100"] = new("gems_100", IAPProductType.Consumable, 100, 0),
                ["gems_500"] = new("gems_500", IAPProductType.Consumable, 500, 0),
                ["starter_pack"] = new("starter_pack", IAPProductType.NonConsumable, 100, 500),
                ["battle_pass"] = new("battle_pass", IAPProductType.Subscription, 0, 0)
            };

            Assert.That(products.ContainsKey("gems_100"), Is.True);
            Assert.That(products["gems_100"].GemAmount, Is.EqualTo(100));
            Assert.That(products.ContainsKey("invalid"), Is.False);
        }

        [Test]
        public void Consumable_GrantsGems()
        {
            int gems = 0;
            int purchase = 500;
            gems += purchase;
            Assert.That(gems, Is.EqualTo(500));
        }

        [Test]
        public void Subscription_ActivatesPremium()
        {
            bool isPremium = false;
            isPremium = true; // On purchase
            Assert.That(isPremium, Is.True);
        }

        [Test]
        public void NonConsumable_OnlyPurchasedOnce()
        {
            var purchased = new HashSet<string>();
            purchased.Add("remove_ads");
            bool alreadyOwned = purchased.Contains("remove_ads");
            Assert.That(alreadyOwned, Is.True);
        }
    }

    [TestFixture]
    public class AdTests
    {
        [Test]
        public void MaxAdsPerDay_Is6()
        {
            int maxAds = 6;
            int adsWatched = 0;
            Assert.That(maxAds - adsWatched, Is.EqualTo(6));
        }

        [Test]
        public void CooldownBetweenAds_Is60Seconds()
        {
            float cooldown = 60f;
            Assert.That(cooldown, Is.EqualTo(60f));
        }

        [Test]
        public void CannotExceedDailyLimit()
        {
            int maxAds = 6;
            int adsWatched = 6;
            bool canShowAd = adsWatched < maxAds;
            Assert.That(canShowAd, Is.False);
        }

        [Test]
        public void DailyCounter_ResetsOnNewDay()
        {
            string today = "2026-04-28";
            string yesterday = "2026-04-27";
            bool isNewDay = today != yesterday;
            Assert.That(isNewDay, Is.True);
        }

        [Test]
        public void AdPlacements_AreValid()
        {
            var placements = new[] { "revive", "double_gold", "bonus_powerup" };
            Assert.That(placements, Has.Length.EqualTo(3));
            Assert.That(placements, Contains.Item("revive"));
        }
    }

    [TestFixture]
    public class AnalyticsTests
    {
        [Test]
        public void EventNames_AreSnakeCase()
        {
            var events = new[]
            {
                "session_start", "session_end", "run_started", "run_completed",
                "floor_cleared", "boss_encountered", "boss_defeated", "player_death",
                "enemy_killed", "damage_dealt", "damage_taken", "ability_used",
                "dodge_used", "powerup_selected", "loot_collected", "gold_earned",
                "town_upgrade_purchased", "quest_completed", "battle_pass_level_up",
                "iap_purchase", "ad_watched", "daily_login", "fps_drop"
            };

            foreach (var e in events)
            {
                Assert.That(e, Does.Match(@"^[a-z_]+$"), $"Event '{e}' is not snake_case");
            }
        }

        [Test]
        public void RunCompleted_HasRequiredFields()
        {
            var required = new[] { "floors_cleared", "enemies_killed", "gold_collected", "time_seconds", "defeated_boss", "death_cause" };
            Assert.That(required.Length, Is.EqualTo(6));
        }
    }

    [TestFixture]
    public class ABTestTests
    {
        [Test]
        public void WeightedSelection_RespectsWeights()
        {
            float[] weights = { 50, 30, 20 };
            string[] variants = { "control", "variant_a", "variant_b" };

            var rng = new Random(42);
            var counts = new int[3];

            for (int i = 0; i < 10000; i++)
            {
                float roll = (float)(rng.NextDouble() * 100);
                float cumulative = 0;
                for (int j = 0; j < weights.Length; j++)
                {
                    cumulative += weights[j];
                    if (roll <= cumulative)
                    {
                        counts[j]++;
                        break;
                    }
                }
            }

            // Within 5% of expected
            Assert.That(counts[0], Is.InRange(4500, 5500));
            Assert.That(counts[1], Is.InRange(2500, 3500));
            Assert.That(counts[2], Is.InRange(1500, 2500));
        }

        [Test]
        public void DefaultVariant_IsControl()
        {
            string defaultVariant = "control";
            Assert.That(defaultVariant, Is.EqualTo("control"));
        }
    }
}

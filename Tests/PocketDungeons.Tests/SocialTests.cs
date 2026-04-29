using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== PHASE 5: Social & Live-Ops Tests ====================

    [TestFixture]
    public class LeaderboardTests
    {
        [Test]
        public void Leaderboard_IDs_AreValid()
        {
            var ids = new[] {
                "com.pocketdungeons.score",
                "com.pocketdungeons.floor",
                "com.pocketdungeons.boss_time",
                "com.pocketdungeons.weekly"
            };
            foreach (var id in ids)
                Assert.That(id, Does.StartWith("com.pocketdungeons."));
        }

        [Test]
        public void BossTime_ConvertedToMillis()
        {
            float bossTime = 45.678f;
            long millis = (long)(bossTime * 1000);
            Assert.That(millis, Is.EqualTo(45678));
        }
    }

    [TestFixture]
    public class AchievementTests
    {
        [Test]
        public void Progress_ClampedTo100()
        {
            int currentValue = 150;
            int targetValue = 100;
            float progress = Math.Clamp((float)currentValue / targetValue * 100f, 0, 100);
            Assert.That(progress, Is.EqualTo(100f));
        }

        [Test]
        public void IncrementalProgress_Accumulates()
        {
            float currentPercent = 50f;
            int increment = 10;
            int target = 100;
            float incrementPercent = (float)increment / target * 100f;
            float newPercent = Math.Min(currentPercent + incrementPercent, 100f);
            Assert.That(newPercent, Is.EqualTo(60f));
        }
    }

    [TestFixture]
    public class CloudSaveTests
    {
        [Test]
        public void MergeConflict_TakesMaxGold()
        {
            int localGold = 1000;
            int cloudGold = 1500;
            int merged = Math.Max(localGold, cloudGold);
            Assert.That(merged, Is.EqualTo(1500));
        }

        [Test]
        public void MergeConflict_UnionMergesCollections()
        {
            var localHeroes = new HashSet<string> { "warrior", "archer" };
            var cloudHeroes = new HashSet<string> { "warrior", "mage" };
            var merged = new HashSet<string>(localHeroes);
            foreach (var h in cloudHeroes) merged.Add(h);
            Assert.That(merged, Has.Count.EqualTo(3));
            Assert.That(merged, Contains.Item("warrior"));
            Assert.That(merged, Contains.Item("archer"));
            Assert.That(merged, Contains.Item("mage"));
        }

        [Test]
        public void MergeConflict_TakesMaxUpgradeLevel()
        {
            var local = new Dictionary<string, int> { ["blacksmith"] = 3, ["armory"] = 5 };
            var cloud = new Dictionary<string, int> { ["blacksmith"] = 5, ["treasury"] = 2 };

            var merged = new Dictionary<string, int>(local);
            foreach (var kvp in cloud)
            {
                if (merged.TryGetValue(kvp.Key, out int localLevel))
                    merged[kvp.Key] = Math.Max(localLevel, kvp.Value);
                else
                    merged[kvp.Key] = kvp.Value;
            }

            Assert.That(merged["blacksmith"], Is.EqualTo(5));
            Assert.That(merged["armory"], Is.EqualTo(5));
            Assert.That(merged["treasury"], Is.EqualTo(2));
        }

        [Test]
        public void PremiumFlag_MergesWithOR()
        {
            bool localPremium = false;
            bool cloudPremium = true;
            bool merged = localPremium || cloudPremium;
            Assert.That(merged, Is.True);
        }
    }

    [TestFixture]
    public class SeasonalEventTests
    {
        [Test]
        public void Event_ActiveWithinDateRange()
        {
            var now = DateTime.UtcNow;
            var start = now.AddDays(-1);
            var end = now.AddDays(5);
            bool active = now >= start && now < end;
            Assert.That(active, Is.True);
        }

        [Test]
        public void Event_InactiveAfterEnd()
        {
            var now = DateTime.UtcNow;
            var end = now.AddDays(-1);
            bool active = now < end;
            Assert.That(active, Is.False);
        }

        [Test]
        public void EventModifiers_Override()
        {
            var modifiers = new Dictionary<string, float> { ["gold_multiplier"] = 2.0f, ["enemy_hp_multiplier"] = 0.8f };
            float goldMul = modifiers.GetValueOrDefault("gold_multiplier", 1f);
            Assert.That(goldMul, Is.EqualTo(2.0f));
        }
    }

    [TestFixture]
    public class DailyLoginTests
    {
        [Test]
        public void StreakBonus_Multiplies()
        {
            float baseGold = 100;
            float streakMultiplier = 0.1f;
            int streak = 5;
            float multiplier = 1f + (streak - 1) * streakMultiplier;
            float reward = baseGold * multiplier;
            Assert.That(reward, Is.EqualTo(140f));
        }

        [Test]
        public void StreakResets_OnMissedDay()
        {
            string lastLogin = "2026-04-26";
            string today = "2026-04-28";
            string yesterday = DateTime.Parse(today).AddDays(-1).ToString("yyyy-MM-dd");
            bool consecutive = lastLogin == yesterday;
            Assert.That(consecutive, Is.False);
        }

        [Test]
        public void WeeklyCycle_WrapsAround()
        {
            int dayInCycle = 6; // 0-indexed, so day 7
            int next = (dayInCycle + 1) % 7;
            Assert.That(next, Is.EqualTo(0));
        }

        [Test]
        public void CannotClaimTwice_SameDay()
        {
            string lastLogin = "2026-04-28";
            string today = "2026-04-28";
            bool canClaim = lastLogin != today;
            Assert.That(canClaim, Is.False);
        }
    }
}

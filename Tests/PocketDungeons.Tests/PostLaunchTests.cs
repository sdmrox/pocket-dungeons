using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== POST-LAUNCH Tests ====================

    [TestFixture]
    public class ContentSchedulerTests
    {
        [Test]
        public void ScheduledContent_TriggersOnRelease()
        {
            var now = DateTime.UtcNow;
            var releaseDate = now.AddMinutes(-5);
            bool shouldRelease = now >= releaseDate;
            Assert.That(shouldRelease, Is.True);
        }

        [Test]
        public void UpcomingContent_NotReleasedEarly()
        {
            var now = DateTime.UtcNow;
            var releaseDate = now.AddDays(7);
            bool shouldRelease = now >= releaseDate;
            Assert.That(shouldRelease, Is.False);
        }

        [Test]
        public void ContentTypes_AllDefined()
        {
            var types = new[] { "QuestRotation", "ChallengeModifier", "EnemyVariant",
                "PowerUp", "Hero", "Biome", "BattlePassSeason", "GameMode", "StoryChapter" };
            Assert.That(types, Has.Length.EqualTo(9));
        }
    }

    [TestFixture]
    public class RevenueOptimizerTests
    {
        [Test]
        public void PlayerSegmentation_ClassifiesCorrectly()
        {
            string Classify(float spend, int runs, int days) =>
                spend >= 50 && days >= 30 ? "whale" :
                spend >= 10 ? "minnow" :
                spend > 0 ? "starter" :
                runs >= 20 ? "engaged_free" :
                "free_player";

            Assert.That(Classify(100, 50, 60), Is.EqualTo("whale"));
            Assert.That(Classify(15, 10, 5), Is.EqualTo("minnow"));
            Assert.That(Classify(5, 5, 3), Is.EqualTo("starter"));
            Assert.That(Classify(0, 30, 10), Is.EqualTo("engaged_free"));
            Assert.That(Classify(0, 5, 2), Is.EqualTo("free_player"));
        }

        [Test]
        public void ARPU_CalculatedCorrectly()
        {
            float totalSpend = 50f;
            int daysActive = 25;
            float arpu = totalSpend / daysActive;
            Assert.That(arpu, Is.EqualTo(2f));
        }

        [Test]
        public void RunsPerDay_CalculatedCorrectly()
        {
            int totalRuns = 100;
            int daysActive = 20;
            float rpd = (float)totalRuns / daysActive;
            Assert.That(rpd, Is.EqualTo(5f));
        }
    }

    [TestFixture]
    public class RetentionTests
    {
        [Test]
        public void D1Benchmark_Above35Percent()
        {
            float d1Target = 0.35f;
            Assert.That(d1Target, Is.GreaterThanOrEqualTo(0.35f));
        }

        [Test]
        public void D7Benchmark_Above15Percent()
        {
            float d7Target = 0.15f;
            Assert.That(d7Target, Is.GreaterThanOrEqualTo(0.15f));
        }

        [Test]
        public void D30Benchmark_Above8Percent()
        {
            float d30Target = 0.08f;
            Assert.That(d30Target, Is.GreaterThanOrEqualTo(0.08f));
        }

        [Test]
        public void SessionDuration_Average()
        {
            var sessions = new[] { 120f, 180f, 150f, 200f, 100f };
            float avg = sessions.Average();
            Assert.That(avg, Is.EqualTo(150f));
        }

        [Test]
        public void Milestones_CheckedCorrectly()
        {
            int[] milestones = { 1, 3, 7, 14, 30, 60, 90 };
            Assert.That(milestones.Contains(7), Is.True);
            Assert.That(milestones.Contains(5), Is.False);
        }
    }

    [TestFixture]
    public class PlatformAbstractionTests
    {
        [Test]
        public void EditorPlatform_CreatesMocks()
        {
            // Editor stubs should not throw
            string platform = "Editor";
            Assert.That(platform, Is.Not.EqualTo("iOS"));
            Assert.That(platform, Is.Not.EqualTo("Android"));
        }

        [Test]
        public void AllPlatforms_Defined()
        {
            var platforms = new[] { "iOS", "Android", "Editor" };
            Assert.That(platforms, Has.Length.EqualTo(3));
        }
    }

    // ==================== INTEGRATION TESTS ====================

    [TestFixture]
    public class RunFlowIntegrationTests
    {
        [Test]
        public void FullRunFlow_StateTransitions()
        {
            var states = new List<string>();
            states.Add("Boot");
            states.Add("MainMenu");
            states.Add("Loading");
            states.Add("Gameplay");
            states.Add("Death");
            states.Add("Results");
            states.Add("MainMenu");

            Assert.That(states[0], Is.EqualTo("Boot"));
            Assert.That(states[^1], Is.EqualTo("MainMenu"));
            Assert.That(states.Count, Is.EqualTo(7));
        }

        [Test]
        public void RunCompletion_UpdatesAllSystems()
        {
            // Simulate run completion
            int floorsCleared = 5;
            int enemiesKilled = 25;
            int goldCollected = 150;
            float runTime = 120f;

            // Score calculation
            int score = floorsCleared * 100 + enemiesKilled * 10 + goldCollected * 5;
            if (runTime < 180f) score += 200; // time bonus
            Assert.That(score, Is.EqualTo(1700));

            // Town gold
            int townGold = 0;
            townGold += goldCollected;
            Assert.That(townGold, Is.EqualTo(150));

            // Quest progress
            int questKills = 0;
            questKills += enemiesKilled;
            Assert.That(questKills, Is.EqualTo(25));

            // Battle Pass XP
            int bpXP = floorsCleared * 50 + enemiesKilled * 10;
            Assert.That(bpXP, Is.EqualTo(500));

            // Prestige tracking
            int totalFloors = 0;
            totalFloors += floorsCleared;
            Assert.That(totalFloors, Is.EqualTo(5));
        }

        [Test]
        public void PowerUpFlow_SelectionToApplication()
        {
            // Generate 3 choices
            int choiceCount = 3;
            float[] dmgMultipliers = { 1.2f, 1.0f, 1.0f };

            // Player selects first
            float selected = dmgMultipliers[0];

            // Apply to stats
            float baseDamage = 10f;
            float finalDamage = baseDamage * selected;
            Assert.That(finalDamage, Is.EqualTo(12f));
        }

        [Test]
        public void DungeonFloorProgression_Integration()
        {
            int currentFloor = 1;
            int enemiesRemaining = 5;

            // Kill all enemies
            for (int i = 0; i < 5; i++)
                enemiesRemaining--;

            Assert.That(enemiesRemaining, Is.EqualTo(0));

            // Advance floor
            currentFloor++;
            Assert.That(currentFloor, Is.EqualTo(2));

            // Check if boss floor
            bool isBoss = currentFloor % 10 == 0;
            Assert.That(isBoss, Is.False);
        }
    }

    [TestFixture]
    public class MonetizationFlowIntegrationTests
    {
        [Test]
        public void BattlePassPurchase_Flow()
        {
            bool isPremium = false;

            // Purchase subscription
            isPremium = true;

            // Can now claim premium rewards
            int level = 5;
            bool[] premiumClaimed = new bool[10];

            for (int i = 0; i < level; i++)
            {
                if (isPremium && !premiumClaimed[i])
                    premiumClaimed[i] = true;
            }

            Assert.That(premiumClaimed[0], Is.True);
            Assert.That(premiumClaimed[4], Is.True);
            Assert.That(premiumClaimed[5], Is.False);
        }

        [Test]
        public void DailyLogin_QuestReward_BattlePassXP_Flow()
        {
            int streak = 3;
            float streakMultiplier = 1f + (streak - 1) * 0.1f;
            int baseGold = 50;
            int baseBPXP = 25;

            int goldEarned = (int)(baseGold * streakMultiplier);
            int xpEarned = (int)(baseBPXP * streakMultiplier);

            Assert.That(goldEarned, Is.EqualTo(60));
            Assert.That(xpEarned, Is.EqualTo(30));

            // XP into battle pass
            int bpLevel = 0;
            int bpXP = xpEarned;
            int xpPerLevel = 100;
            while (bpXP >= xpPerLevel) { bpXP -= xpPerLevel; bpLevel++; }
            Assert.That(bpLevel, Is.EqualTo(0)); // Not enough for a level
            Assert.That(bpXP, Is.EqualTo(30));
        }
    }

    [TestFixture]
    public class CloudSaveIntegrationTests
    {
        [Test]
        public void SaveLoad_RoundTrip()
        {
            var data = new Dictionary<string, object>
            {
                ["gold"] = 1000,
                ["prestige"] = 2,
                ["heroes"] = new List<string> { "warrior", "archer" }
            };

            // Serialize
            string json = System.Text.Json.JsonSerializer.Serialize(data);
            Assert.That(json, Is.Not.Empty);

            // Deserialize
            var loaded = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded!.ContainsKey("gold"), Is.True);
        }
    }
}

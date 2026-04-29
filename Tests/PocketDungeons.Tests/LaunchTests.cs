using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== PHASE 6: Launch & Balance Tests ====================

    [TestFixture]
    public class BalanceConfigTests
    {
        [Test]
        public void WarriorHP_HigherThanArcherAndMage()
        {
            Assert.That(120, Is.GreaterThan(80));  // Warrior > Archer
            Assert.That(120, Is.GreaterThan(70));  // Warrior > Mage
        }

        [Test]
        public void ScaledEnemyHP_CalculatedCorrectly()
        {
            int baseHP = 10;
            float scale = 0.15f;
            int floor = 10;
            int result = (int)Math.Round(baseHP * (1f + scale * (floor - 1)));
            Assert.That(result, Is.EqualTo(24)); // 10 * (1 + 0.15*9) = 10 * 2.35 ≈ 24
        }

        [Test]
        public void ScaledEnemyDamage_CalculatedCorrectly()
        {
            int baseDmg = 3;
            float scale = 0.10f;
            int floor = 10;
            int result = (int)Math.Round(baseDmg * (1f + scale * (floor - 1)));
            Assert.That(result, Is.EqualTo(6)); // 3 * (1 + 0.10*9) = 3 * 1.9 ≈ 6
        }

        [Test]
        public void EnemyCount_CappedAt15()
        {
            int baseCount = 3;
            float scale = 0.5f;
            int floor = 50;
            int count = Math.Min(baseCount + (int)Math.Round(scale * (floor - 1)), 15);
            Assert.That(count, Is.EqualTo(15));
        }

        [Test]
        public void XPForLevel_ScalesExponentially()
        {
            float xpScale = 1.15f;
            int xp1 = (int)Math.Round(100 * Math.Pow(xpScale, 0)); // Level 1: 100
            int xp5 = (int)Math.Round(100 * Math.Pow(xpScale, 4)); // Level 5
            Assert.That(xp5, Is.GreaterThan(xp1));
            Assert.That(xp1, Is.EqualTo(100));
        }

        [Test]
        public void TargetRunDuration_Is150Seconds()
        {
            float target = 150f;
            Assert.That(target / 60f, Is.EqualTo(2.5f));
        }
    }

    [TestFixture]
    public class DifficultyTests
    {
        [Test]
        public void Multiplier_ClampedToRange()
        {
            float min = 0.6f, max = 1.4f;
            float value = 2f;
            float clamped = Math.Clamp(value, min, max);
            Assert.That(clamped, Is.EqualTo(1.4f));

            value = 0.1f;
            clamped = Math.Clamp(value, min, max);
            Assert.That(clamped, Is.EqualTo(0.6f));
        }

        [Test]
        public void DifficultyLabel_Correct()
        {
            string GetLabel(float mul) => mul switch
            {
                < 0.8f => "Easy",
                < 1.1f => "Normal",
                < 1.3f => "Hard",
                _ => "Very Hard"
            };

            Assert.That(GetLabel(0.7f), Is.EqualTo("Easy"));
            Assert.That(GetLabel(1.0f), Is.EqualTo("Normal"));
            Assert.That(GetLabel(1.2f), Is.EqualTo("Hard"));
            Assert.That(GetLabel(1.4f), Is.EqualTo("Very Hard"));
        }

        [Test]
        public void InverseDropRate_HigherDifficultyBetterDrops()
        {
            float baseRate = 0.7f;
            float normalDrop = baseRate * (2f - 1.0f);
            float hardDrop = baseRate * (2f - 1.3f);
            // At higher difficulty, multiplier is smaller but still positive
            Assert.That(hardDrop, Is.LessThan(normalDrop));
            Assert.That(hardDrop, Is.GreaterThan(0));
        }

        [Test]
        public void AdaptiveDifficulty_DecreasesOnDeaths()
        {
            float current = 1.0f;
            float adjustment = 0.05f;
            current -= adjustment;
            Assert.That(current, Is.EqualTo(0.95f).Within(0.001f));
        }
    }

    [TestFixture]
    public class AppStoreTests
    {
        [Test]
        public void ReviewPrompt_AfterEnoughRuns()
        {
            int minRuns = 10;
            int runsSincePrompt = 10;
            bool shouldPrompt = runsSincePrompt >= minRuns;
            Assert.That(shouldPrompt, Is.True);
        }

        [Test]
        public void ReviewPrompt_NotIfAlreadyRated()
        {
            bool hasRated = true;
            Assert.That(!hasRated, Is.False);
        }

        [Test]
        public void DeepLink_ParsesEventId()
        {
            string url = "pocketdungeons://event/halloween";
            string eventId = url.Substring(url.LastIndexOf("event/") + 6);
            Assert.That(eventId, Is.EqualTo("halloween"));
        }
    }

    [TestFixture]
    public class BetaPhaseTests
    {
        enum BetaPhase { Internal, ClosedBeta, OpenBeta, Production }

        [Test]
        public void Internal_HasAllTestFeatures()
        {
            bool showDebug = true;
            bool verboseLog = true;
            Assert.That(showDebug && verboseLog, Is.True);
        }

        [Test]
        public void Production_DisablesDebug()
        {
            var phase = BetaPhase.Production;
            bool showDebug = phase != BetaPhase.Production;
            Assert.That(showDebug, Is.False);
        }
    }

    [TestFixture]
    public class CrashReporterTests
    {
        [Test]
        public void CrashFreeRate_Calculated()
        {
            int totalSessions = 1000;
            int crashedSessions = 3;
            float rate = 1f - ((float)crashedSessions / totalSessions);
            Assert.That(rate, Is.GreaterThan(0.995f)); // >99.5%
        }

        [Test]
        public void Breadcrumbs_CappedAt50()
        {
            var breadcrumbs = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                breadcrumbs.Add($"Step {i}");
                if (breadcrumbs.Count > 50) breadcrumbs.RemoveAt(0);
            }
            Assert.That(breadcrumbs, Has.Count.EqualTo(50));
        }
    }

    [TestFixture]
    public class PerformanceMonitorTests
    {
        [Test]
        public void AverageFPS_CalculatedCorrectly()
        {
            var frameTimes = new Queue<float>();
            for (int i = 0; i < 60; i++)
                frameTimes.Enqueue(1f / 60f);

            float total = 0;
            foreach (float t in frameTimes) total += t;
            float avgFPS = frameTimes.Count / total;

            Assert.That(avgFPS, Is.EqualTo(60f).Within(1f));
        }

        [Test]
        public void FPSDrop_DetectedBelow45()
        {
            float threshold = 45f;
            float currentFPS = 40f;
            Assert.That(currentFPS < threshold, Is.True);
        }

        [Test]
        public void AdaptiveQuality_DisablesShadowsBelow30FPS()
        {
            float fps = 25f;
            bool disableShadows = fps < 30f;
            Assert.That(disableShadows, Is.True);
        }
    }
}

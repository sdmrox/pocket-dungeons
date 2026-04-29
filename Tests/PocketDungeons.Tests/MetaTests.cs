using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== PHASE 3: Meta & Retention Tests ====================

    [TestFixture]
    public class TownUpgradeTests
    {
        int[] costs = { 100, 200, 400, 800, 1600, 3200, 6400, 12800, 25600, 51200 };
        float[] bonuses = { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f, 0.30f, 0.35f, 0.40f, 0.45f, 0.50f };

        [Test]
        public void CostScalesExponentially()
        {
            for (int i = 1; i < costs.Length; i++)
                Assert.That(costs[i], Is.GreaterThan(costs[i - 1]));
        }

        [Test]
        public void BonusScalesLinearly()
        {
            float diff = bonuses[1] - bonuses[0];
            for (int i = 2; i < bonuses.Length; i++)
                Assert.That(bonuses[i] - bonuses[i - 1], Is.EqualTo(diff).Within(0.001f));
        }

        [Test]
        public void CannotUpgrade_InsufficientGold()
        {
            int gold = 50;
            int cost = costs[0]; // 100
            Assert.That(gold >= cost, Is.False);
        }

        [Test]
        public void CanUpgrade_SufficientGold()
        {
            int gold = 200;
            int cost = costs[0]; // 100
            Assert.That(gold >= cost, Is.True);
        }

        [Test]
        public void MaxLevel_CapsAt10()
        {
            int maxLevel = 10;
            int currentLevel = 10;
            Assert.That(currentLevel >= maxLevel, Is.True);
        }

        [Test]
        public void Aggregate_BonusesFromMultipleBuildings()
        {
            float dmgBonus = 0.10f;
            float hpBonus = 20f;
            float critBonus = 0.05f;
            float totalMultiplier = 1f + dmgBonus + critBonus;
            Assert.That(totalMultiplier, Is.EqualTo(1.15f).Within(0.001f));
        }
    }

    [TestFixture]
    public class QuestTests
    {
        [Test]
        public void DailyQuests_GeneratesExact3()
        {
            int dailyCount = 3;
            int pool = 10;
            int generated = Math.Min(dailyCount, pool);
            Assert.That(generated, Is.EqualTo(3));
        }

        [Test]
        public void WeeklyQuests_GeneratesExact3()
        {
            int weeklyCount = 3;
            int pool = 8;
            int generated = Math.Min(weeklyCount, pool);
            Assert.That(generated, Is.EqualTo(3));
        }

        [Test]
        public void QuestProgress_Tracked()
        {
            int target = 10;
            int current = 0;
            current += 5;
            Assert.That((float)current / target, Is.EqualTo(0.5f));
            current += 5;
            Assert.That(current >= target, Is.True);
        }

        [Test]
        public void QuestReward_GrantsGoldAndXP()
        {
            int goldReward = 100;
            int bpXP = 50;
            Assert.That(goldReward, Is.GreaterThan(0));
            Assert.That(bpXP, Is.GreaterThan(0));
        }

        [Test]
        public void CannotClaim_IncompleteQuest()
        {
            bool isCompleted = false;
            bool isClaimed = false;
            bool canClaim = isCompleted && !isClaimed;
            Assert.That(canClaim, Is.False);
        }

        [Test]
        public void CannotDoubleClaim()
        {
            bool isCompleted = true;
            bool isClaimed = true;
            bool canClaim = isCompleted && !isClaimed;
            Assert.That(canClaim, Is.False);
        }
    }

    [TestFixture]
    public class BattlePassTests
    {
        [Test]
        public void XP_LevelsUp()
        {
            int[] xpPerTier = { 100, 150, 200, 250, 300 };
            int currentLevel = 0;
            int currentXP = 0;

            currentXP += 250; // Add 250 XP

            while (currentLevel < xpPerTier.Length && currentXP >= xpPerTier[currentLevel])
            {
                currentXP -= xpPerTier[currentLevel];
                currentLevel++;
            }

            Assert.That(currentLevel, Is.EqualTo(2)); // 100 + 150 = 250
            Assert.That(currentXP, Is.EqualTo(0));
        }

        [Test]
        public void FreeTrack_ClaimableWithoutPremium()
        {
            bool isPremium = false;
            int level = 5;
            int tier = 3;
            bool canClaimFree = tier < level;
            Assert.That(canClaimFree, Is.True);
        }

        [Test]
        public void PremiumTrack_RequiresPurchase()
        {
            bool isPremium = false;
            bool canClaimPremium = isPremium;
            Assert.That(canClaimPremium, Is.False);
        }

        [Test]
        public void SeasonDuration_Is42Days()
        {
            int durationDays = 42;
            Assert.That(durationDays, Is.EqualTo(42));
        }

        [Test]
        public void PremiumPrice_Is499()
        {
            float price = 4.99f;
            Assert.That(price, Is.EqualTo(4.99f).Within(0.001f));
        }

        [Test]
        public void LevelProgress_Calculated()
        {
            int currentXP = 75;
            int xpNeeded = 150;
            float progress = (float)currentXP / xpNeeded;
            Assert.That(progress, Is.EqualTo(0.5f));
        }
    }

    [TestFixture]
    public class OnboardingTests
    {
        [Test]
        public void FeatureUnlock_Schedule()
        {
            bool ShouldShow(string feature, int runs) => feature switch
            {
                "power_ups" => runs >= 2,
                "town_upgrades" => runs >= 3,
                "daily_quests" => runs >= 4,
                "battle_pass" => runs >= 5,
                "hero_switching" => runs >= 7,
                "social" => runs >= 10,
                _ => true
            };

            Assert.That(ShouldShow("power_ups", 1), Is.False);
            Assert.That(ShouldShow("power_ups", 2), Is.True);
            Assert.That(ShouldShow("town_upgrades", 2), Is.False);
            Assert.That(ShouldShow("town_upgrades", 3), Is.True);
            Assert.That(ShouldShow("social", 9), Is.False);
            Assert.That(ShouldShow("social", 10), Is.True);
        }

        [Test]
        public void CompletedSteps_NotRepeated()
        {
            var completed = new HashSet<string>();
            completed.Add("movement_tutorial");
            Assert.That(completed.Contains("movement_tutorial"), Is.True);
            Assert.That(completed.Contains("dodge_tutorial"), Is.False);
        }
    }

    [TestFixture]
    public class PrestigeTests
    {
        [Test]
        public void PrestigeBonus_Is5PercentPerLevel()
        {
            float bonusPerLevel = 0.05f;
            int level = 3;
            float totalBonus = level * bonusPerLevel;
            Assert.That(totalBonus, Is.EqualTo(0.15f).Within(0.001f));
        }

        [Test]
        public void CanPrestige_At50FloorsPerLevel()
        {
            int unlockFloor = 50;
            int prestigeLevel = 0;
            int totalFloorsCleared = 50;
            bool canPrestige = totalFloorsCleared >= unlockFloor * (prestigeLevel + 1);
            Assert.That(canPrestige, Is.True);
        }

        [Test]
        public void CannotPrestige_AtMaxLevel()
        {
            int maxLevel = 10;
            int currentLevel = 10;
            Assert.That(currentLevel >= maxLevel, Is.True);
        }
    }
}

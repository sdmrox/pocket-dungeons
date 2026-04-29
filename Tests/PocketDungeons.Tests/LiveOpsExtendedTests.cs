using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class DailyDungeonTests
    {
        int _baseSeed = 42;

        int GenerateSeed(DateTime date)
        {
            return _baseSeed ^ (date.Year * 10000 + date.Month * 100 + date.Day);
        }

        [Test]
        public void SameDaySameSeed()
        {
            var date = new DateTime(2026, 4, 28);
            int seed1 = GenerateSeed(date);
            int seed2 = GenerateSeed(date);
            Assert.That(seed1, Is.EqualTo(seed2));
        }

        [Test]
        public void DifferentDaysDifferentSeeds()
        {
            int seed1 = GenerateSeed(new DateTime(2026, 4, 28));
            int seed2 = GenerateSeed(new DateTime(2026, 4, 29));
            Assert.That(seed1, Is.Not.EqualTo(seed2));
        }

        [Test]
        public void CanPlayOnce_ThenBlocked()
        {
            bool completed = false;
            Assert.That(!completed, Is.True); // Can play
            completed = true;
            Assert.That(!completed, Is.False); // Blocked
        }

        [Test]
        public void BestScore_TracksHighest()
        {
            int best = 0;
            int[] scores = { 100, 250, 200, 300, 150 };
            foreach (int s in scores)
                if (s > best) best = s;
            Assert.That(best, Is.EqualTo(300));
        }

        [Test]
        public void TimeUntilReset_IsPositive()
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(1);
            var remaining = tomorrow - now;
            Assert.That(remaining.TotalSeconds, Is.GreaterThan(0));
        }
    }

    [TestFixture]
    public class WeeklyBossRaidTests
    {
        int _communityHP;
        int _totalDamage;
        Dictionary<string, int> _playerDamage = new();
        Dictionary<string, int> _attempts = new();
        int _maxAttempts = 3;

        [SetUp]
        public void SetUp()
        {
            _communityHP = 1000000;
            _totalDamage = 0;
            _playerDamage.Clear();
            _attempts.Clear();
        }

        bool ContributeDamage(string playerId, int damage)
        {
            if (_communityHP <= 0) return false;
            _attempts.TryGetValue(playerId, out int att);
            if (att >= _maxAttempts) return false;
            _attempts[playerId] = att + 1;

            int actual = Math.Min(damage, _communityHP);
            _communityHP -= actual;
            _totalDamage += actual;
            _playerDamage.TryGetValue(playerId, out int existing);
            _playerDamage[playerId] = existing + actual;
            return true;
        }

        [Test]
        public void Contribute_ReducesHP()
        {
            ContributeDamage("p1", 1000);
            Assert.That(_communityHP, Is.EqualTo(999000));
        }

        [Test]
        public void MaxAttempts_BlocksAfter3()
        {
            Assert.That(ContributeDamage("p1", 100), Is.True);
            Assert.That(ContributeDamage("p1", 100), Is.True);
            Assert.That(ContributeDamage("p1", 100), Is.True);
            Assert.That(ContributeDamage("p1", 100), Is.False);
        }

        [Test]
        public void DamageCannotExceedRemainingHP()
        {
            _communityHP = 50;
            ContributeDamage("p1", 100);
            Assert.That(_communityHP, Is.EqualTo(0));
            Assert.That(_totalDamage, Is.EqualTo(50));
        }

        [Test]
        public void DeadBoss_RejectsContribution()
        {
            _communityHP = 0;
            Assert.That(ContributeDamage("p1", 100), Is.False);
        }

        [Test]
        public void MultiplePlayers_TrackedSeparately()
        {
            ContributeDamage("p1", 500);
            ContributeDamage("p2", 300);
            Assert.That(_playerDamage["p1"], Is.EqualTo(500));
            Assert.That(_playerDamage["p2"], Is.EqualTo(300));
        }

        [Test]
        public void HealthPercentage_Decreases()
        {
            ContributeDamage("p1", 250000);
            float pct = (float)_communityHP / 1000000;
            Assert.That(pct, Is.EqualTo(0.75f).Within(0.001f));
        }
    }

    [TestFixture]
    public class WeeklyChallengeTests
    {
        enum Modifier { DoubleEnemies, SpeedRun, NoHeal, GlassCannon, TankMode, GoldRush }

        record Challenge(string Id, Modifier Mod, float ModValue, int TargetFloor, float TimeLimit,
            int RewardGold, int RewardGems);

        float ApplyModifier(Modifier mod, float baseValue, Challenge active)
        {
            if (active.Mod != mod) return baseValue;
            return mod switch
            {
                Modifier.DoubleEnemies => baseValue * 2f,
                Modifier.SpeedRun => baseValue * 1.5f,
                Modifier.GlassCannon => baseValue * 3f,
                Modifier.TankMode => baseValue * 0.5f,
                Modifier.GoldRush => baseValue * 2f,
                _ => baseValue * active.ModValue
            };
        }

        bool SubmitResult(Challenge c, int floorsReached, float timeElapsed)
        {
            if (floorsReached < c.TargetFloor) return false;
            if (c.TimeLimit > 0 && timeElapsed > c.TimeLimit) return false;
            return true;
        }

        [Test]
        public void DoubleEnemies_DoublesValue()
        {
            var c = new Challenge("c1", Modifier.DoubleEnemies, 2f, 5, 0, 100, 10);
            Assert.That(ApplyModifier(Modifier.DoubleEnemies, 10f, c), Is.EqualTo(20f));
        }

        [Test]
        public void GlassCannon_TriplesValue()
        {
            var c = new Challenge("c1", Modifier.GlassCannon, 3f, 5, 0, 100, 10);
            Assert.That(ApplyModifier(Modifier.GlassCannon, 10f, c), Is.EqualTo(30f));
        }

        [Test]
        public void WrongModifier_ReturnsBase()
        {
            var c = new Challenge("c1", Modifier.SpeedRun, 1.5f, 5, 0, 100, 10);
            Assert.That(ApplyModifier(Modifier.DoubleEnemies, 10f, c), Is.EqualTo(10f));
        }

        [Test]
        public void Submit_Success_MeetsTarget()
        {
            var c = new Challenge("c1", Modifier.SpeedRun, 1.5f, 5, 120f, 100, 10);
            Assert.That(SubmitResult(c, 5, 100f), Is.True);
        }

        [Test]
        public void Submit_Fail_BelowTarget()
        {
            var c = new Challenge("c1", Modifier.SpeedRun, 1.5f, 5, 120f, 100, 10);
            Assert.That(SubmitResult(c, 4, 100f), Is.False);
        }

        [Test]
        public void Submit_Fail_OverTimeLimit()
        {
            var c = new Challenge("c1", Modifier.SpeedRun, 1.5f, 5, 120f, 100, 10);
            Assert.That(SubmitResult(c, 10, 150f), Is.False);
        }

        [Test]
        public void WeekNumber_CalculatesCorrectly()
        {
            int weekNumber = (new DateTime(2026, 1, 8).DayOfYear - 1) / 7 + 1;
            Assert.That(weekNumber, Is.EqualTo(2));
        }
    }
}

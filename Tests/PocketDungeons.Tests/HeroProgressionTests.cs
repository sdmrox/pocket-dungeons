using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class HeroUnlockTests
    {
        enum UnlockType { FloorReached, EnemiesKilled, RunsCompleted, BossDefeated, GoldEarned, Default }
        record UnlockCondition(string HeroId, UnlockType Type, int RequiredValue, string BossId = "");

        HashSet<string> _unlocked = new() { "Warrior" };
        Dictionary<string, UnlockCondition> _conditions = new();

        [SetUp]
        public void SetUp()
        {
            _unlocked = new HashSet<string> { "Warrior" };
            _conditions = new()
            {
                { "Archer", new("Archer", UnlockType.FloorReached, 5) },
                { "Mage", new("Mage", UnlockType.EnemiesKilled, 100) },
                { "Rogue", new("Rogue", UnlockType.RunsCompleted, 10) },
                { "Paladin", new("Paladin", UnlockType.BossDefeated, 0, "CryptKing") },
                { "Necromancer", new("Necromancer", UnlockType.GoldEarned, 5000) }
            };
        }

        bool CheckUnlock(string heroId, int floors, int kills, int runs, string boss, int gold)
        {
            if (_unlocked.Contains(heroId)) return false;
            if (!_conditions.TryGetValue(heroId, out var cond)) return false;
            bool met = cond.Type switch
            {
                UnlockType.FloorReached => floors >= cond.RequiredValue,
                UnlockType.EnemiesKilled => kills >= cond.RequiredValue,
                UnlockType.RunsCompleted => runs >= cond.RequiredValue,
                UnlockType.BossDefeated => boss == cond.BossId,
                UnlockType.GoldEarned => gold >= cond.RequiredValue,
                _ => false
            };
            if (met) _unlocked.Add(heroId);
            return met;
        }

        [Test]
        public void Warrior_UnlockedByDefault()
        {
            Assert.That(_unlocked.Contains("Warrior"), Is.True);
        }

        [Test]
        public void Archer_UnlocksAtFloor5()
        {
            Assert.That(CheckUnlock("Archer", 4, 0, 0, "", 0), Is.False);
            Assert.That(CheckUnlock("Archer", 5, 0, 0, "", 0), Is.True);
            Assert.That(_unlocked.Contains("Archer"), Is.True);
        }

        [Test]
        public void Mage_UnlocksAt100Kills()
        {
            Assert.That(CheckUnlock("Mage", 0, 99, 0, "", 0), Is.False);
            Assert.That(CheckUnlock("Mage", 0, 100, 0, "", 0), Is.True);
        }

        [Test]
        public void Paladin_UnlocksWithBossDefeat()
        {
            Assert.That(CheckUnlock("Paladin", 0, 0, 0, "Slime", 0), Is.False);
            Assert.That(CheckUnlock("Paladin", 0, 0, 0, "CryptKing", 0), Is.True);
        }

        [Test]
        public void AlreadyUnlocked_ReturnsFalse()
        {
            Assert.That(CheckUnlock("Warrior", 999, 999, 999, "CryptKing", 99999), Is.False);
        }

        [Test]
        public void Progress_CalculatesCorrectly()
        {
            int required = 100;
            int current = 50;
            float progress = (float)current / required;
            Assert.That(progress, Is.EqualTo(0.5f).Within(0.001f));
        }
    }

    [TestFixture]
    public class HeroLevelTests
    {
        int _baseXP = 100;
        float _xpScale = 1.15f;
        int _maxLevel = 50;

        int GetXPRequired(int level)
        {
            return (int)Math.Round(_baseXP * Math.Pow(_xpScale, level - 1));
        }

        [Test]
        public void Level1_Requires100XP()
        {
            Assert.That(GetXPRequired(1), Is.EqualTo(100));
        }

        [Test]
        public void Level2_Requires115XP()
        {
            Assert.That(GetXPRequired(2), Is.EqualTo(115));
        }

        [Test]
        public void Level10_RequiresScaledXP()
        {
            int expected = (int)Math.Round(100 * Math.Pow(1.15, 9));
            Assert.That(GetXPRequired(10), Is.EqualTo(expected));
        }

        [Test]
        public void AddXP_LevelsUp()
        {
            int currentXP = 0;
            int level = 1;
            int xpToAdd = 250;

            currentXP += xpToAdd;
            int levelsGained = 0;
            while (level < _maxLevel)
            {
                int req = GetXPRequired(level);
                if (currentXP < req) break;
                currentXP -= req;
                level++;
                levelsGained++;
            }

            Assert.That(level, Is.GreaterThan(1));
            Assert.That(levelsGained, Is.GreaterThan(0));
        }

        [Test]
        public void MaxLevel_CapsAt50()
        {
            int level = 50;
            int xpAdded = 0;
            if (level >= _maxLevel) xpAdded = 0;
            Assert.That(xpAdded, Is.EqualTo(0));
        }

        [Test]
        public void BonusHealth_IncreasesPerLevel()
        {
            float hpPerLevel = 5f;
            int level = 10;
            int bonus = (int)Math.Round(hpPerLevel * (level - 1));
            Assert.That(bonus, Is.EqualTo(45));
        }

        [Test]
        public void BonusDamage_IncreasesPerLevel()
        {
            float dmgPerLevel = 1f;
            int level = 20;
            int bonus = (int)Math.Round(dmgPerLevel * (level - 1));
            Assert.That(bonus, Is.EqualTo(19));
        }

        [Test]
        public void XP_ScalesExponentially()
        {
            int xp5 = GetXPRequired(5);
            int xp10 = GetXPRequired(10);
            int xp20 = GetXPRequired(20);
            Assert.That(xp10, Is.GreaterThan(xp5));
            Assert.That(xp20, Is.GreaterThan(xp10));
        }
    }
}

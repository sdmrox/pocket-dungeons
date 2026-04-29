using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class BestiaryTests
    {
        class BestiaryEntry
        {
            public string EnemyId;
            public int KillCount;
            public bool IsDiscovered;
            public int LoreTier => KillCount switch { >= 100 => 3, >= 25 => 2, >= 1 => 1, _ => 0 };
            public float DamageBonus => KillCount switch { >= 100 => 0.10f, >= 50 => 0.05f, >= 10 => 0.02f, _ => 0f };
        }

        Dictionary<string, BestiaryEntry> _entries = new();

        [SetUp]
        public void SetUp()
        {
            _entries.Clear();
        }

        void RecordKill(string enemyId)
        {
            if (!_entries.TryGetValue(enemyId, out var entry))
            {
                entry = new BestiaryEntry { EnemyId = enemyId, IsDiscovered = true };
                _entries[enemyId] = entry;
            }
            entry.KillCount++;
        }

        [Test]
        public void FirstKill_DiscoverEnemy()
        {
            RecordKill("slime");
            Assert.That(_entries.ContainsKey("slime"), Is.True);
            Assert.That(_entries["slime"].IsDiscovered, Is.True);
            Assert.That(_entries["slime"].KillCount, Is.EqualTo(1));
        }

        [Test]
        public void LoreTier0_AtZeroKills()
        {
            var entry = new BestiaryEntry { KillCount = 0 };
            Assert.That(entry.LoreTier, Is.EqualTo(0));
        }

        [Test]
        public void LoreTier1_At1Kill()
        {
            var entry = new BestiaryEntry { KillCount = 1 };
            Assert.That(entry.LoreTier, Is.EqualTo(1));
        }

        [Test]
        public void LoreTier2_At25Kills()
        {
            var entry = new BestiaryEntry { KillCount = 25 };
            Assert.That(entry.LoreTier, Is.EqualTo(2));
        }

        [Test]
        public void LoreTier3_At100Kills()
        {
            var entry = new BestiaryEntry { KillCount = 100 };
            Assert.That(entry.LoreTier, Is.EqualTo(3));
        }

        [Test]
        public void DamageBonus_ZeroUnder10Kills()
        {
            var entry = new BestiaryEntry { KillCount = 9 };
            Assert.That(entry.DamageBonus, Is.EqualTo(0f));
        }

        [Test]
        public void DamageBonus_2PercentAt10Kills()
        {
            var entry = new BestiaryEntry { KillCount = 10 };
            Assert.That(entry.DamageBonus, Is.EqualTo(0.02f).Within(0.001f));
        }

        [Test]
        public void DamageBonus_5PercentAt50Kills()
        {
            var entry = new BestiaryEntry { KillCount = 50 };
            Assert.That(entry.DamageBonus, Is.EqualTo(0.05f).Within(0.001f));
        }

        [Test]
        public void DamageBonus_10PercentAt100Kills()
        {
            var entry = new BestiaryEntry { KillCount = 100 };
            Assert.That(entry.DamageBonus, Is.EqualTo(0.10f).Within(0.001f));
        }

        [Test]
        public void CompletionPercentage_CalculatesCorrectly()
        {
            RecordKill("slime");
            RecordKill("bat");
            RecordKill("golem");
            int totalTypes = 9;
            float completion = (float)_entries.Count / totalTypes;
            Assert.That(completion, Is.EqualTo(3f / 9f).Within(0.001f));
        }

        [Test]
        public void TotalKills_SumsAcrossEntries()
        {
            for (int i = 0; i < 10; i++) RecordKill("slime");
            for (int i = 0; i < 5; i++) RecordKill("bat");
            int total = 0;
            foreach (var e in _entries.Values) total += e.KillCount;
            Assert.That(total, Is.EqualTo(15));
        }
    }
}

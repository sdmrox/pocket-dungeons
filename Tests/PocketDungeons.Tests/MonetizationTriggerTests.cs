using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class MonetizationTriggerTests
    {
        enum TriggerType { StarterPack, GemPack, Revive, BattlePassPreview, DoubleGold, InstantChest, TripleDailyReward }

        int _starterPackLevel = 5;
        int _battlePassPreviewDay = 3;
        int _maxAdsPerDay = 6;
        Dictionary<TriggerType, int> _showCounts = new();
        bool _starterPackPurchased;
        bool _frustrated;
        bool _inGameplay;
        int _adsToday;

        [SetUp]
        public void SetUp()
        {
            _showCounts.Clear();
            _starterPackPurchased = false;
            _frustrated = false;
            _inGameplay = false;
            _adsToday = 0;
        }

        int GetShowCount(TriggerType t)
        {
            _showCounts.TryGetValue(t, out int c);
            return c;
        }

        void RecordShow(TriggerType t)
        {
            _showCounts.TryGetValue(t, out int c);
            _showCounts[t] = c + 1;
            if (t is TriggerType.DoubleGold or TriggerType.InstantChest or TriggerType.TripleDailyReward)
                _adsToday++;
        }

        bool ShouldShow(TriggerType type, int level, int days, bool bestRun, bool dead, int bossFloor)
        {
            if (_inGameplay && type != TriggerType.Revive) return false;
            if (_frustrated) return false;

            return type switch
            {
                TriggerType.StarterPack => !_starterPackPurchased && level >= _starterPackLevel && GetShowCount(type) == 0,
                TriggerType.GemPack => bestRun && !_frustrated && GetShowCount(type) < 3,
                TriggerType.Revive => dead && bossFloor > 0 && GetShowCount(type) < 1,
                TriggerType.BattlePassPreview => days >= _battlePassPreviewDay && GetShowCount(type) == 0,
                TriggerType.DoubleGold => dead && _adsToday < _maxAdsPerDay,
                TriggerType.InstantChest => _adsToday < _maxAdsPerDay,
                TriggerType.TripleDailyReward => _adsToday < _maxAdsPerDay,
                _ => false
            };
        }

        [Test]
        public void StarterPack_ShowsAtLevel5()
        {
            Assert.That(ShouldShow(TriggerType.StarterPack, 4, 0, false, false, 0), Is.False);
            Assert.That(ShouldShow(TriggerType.StarterPack, 5, 0, false, false, 0), Is.True);
        }

        [Test]
        public void StarterPack_OnlyShowsOnce()
        {
            RecordShow(TriggerType.StarterPack);
            Assert.That(ShouldShow(TriggerType.StarterPack, 5, 0, false, false, 0), Is.False);
        }

        [Test]
        public void StarterPack_HiddenAfterPurchase()
        {
            _starterPackPurchased = true;
            Assert.That(ShouldShow(TriggerType.StarterPack, 10, 0, false, false, 0), Is.False);
        }

        [Test]
        public void Revive_ShowsOnBossFloorDeath()
        {
            Assert.That(ShouldShow(TriggerType.Revive, 5, 5, false, true, 10), Is.True);
        }

        [Test]
        public void Revive_NotShownOnNonBossFloor()
        {
            Assert.That(ShouldShow(TriggerType.Revive, 5, 5, false, true, 0), Is.False);
        }

        [Test]
        public void Revive_NotShownWhenAlive()
        {
            Assert.That(ShouldShow(TriggerType.Revive, 5, 5, false, false, 10), Is.False);
        }

        [Test]
        public void BattlePassPreview_ShowsOnDay3()
        {
            Assert.That(ShouldShow(TriggerType.BattlePassPreview, 1, 2, false, false, 0), Is.False);
            Assert.That(ShouldShow(TriggerType.BattlePassPreview, 1, 3, false, false, 0), Is.True);
        }

        [Test]
        public void GemPack_ShowsAfterBestRun()
        {
            Assert.That(ShouldShow(TriggerType.GemPack, 5, 5, true, false, 0), Is.True);
            Assert.That(ShouldShow(TriggerType.GemPack, 5, 5, false, false, 0), Is.False);
        }

        [Test]
        public void GemPack_MaxThreeShows()
        {
            RecordShow(TriggerType.GemPack);
            RecordShow(TriggerType.GemPack);
            RecordShow(TriggerType.GemPack);
            Assert.That(ShouldShow(TriggerType.GemPack, 5, 5, true, false, 0), Is.False);
        }

        [Test]
        public void NeverShowDuringGameplay_ExceptRevive()
        {
            _inGameplay = true;
            Assert.That(ShouldShow(TriggerType.StarterPack, 5, 5, true, false, 0), Is.False);
            Assert.That(ShouldShow(TriggerType.DoubleGold, 5, 5, false, true, 0), Is.False);
            Assert.That(ShouldShow(TriggerType.Revive, 5, 5, false, true, 10), Is.True);
        }

        [Test]
        public void NeverShowWhenFrustrated()
        {
            _frustrated = true;
            Assert.That(ShouldShow(TriggerType.StarterPack, 5, 5, true, false, 0), Is.False);
            Assert.That(ShouldShow(TriggerType.Revive, 5, 5, false, true, 10), Is.False);
        }

        [Test]
        public void AdLimit_BlocksAfterMaxPerDay()
        {
            for (int i = 0; i < _maxAdsPerDay; i++)
                RecordShow(TriggerType.DoubleGold);
            Assert.That(ShouldShow(TriggerType.InstantChest, 5, 5, false, false, 0), Is.False);
        }

        [Test]
        public void RemainingAds_DecreasesCorrectly()
        {
            RecordShow(TriggerType.DoubleGold);
            RecordShow(TriggerType.InstantChest);
            int remaining = Math.Max(0, _maxAdsPerDay - _adsToday);
            Assert.That(remaining, Is.EqualTo(4));
        }
    }
}

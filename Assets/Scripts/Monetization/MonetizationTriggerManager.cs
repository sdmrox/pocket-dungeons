using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Monetization
{
    public enum TriggerType
    {
        StarterPack,
        GemPackAfterBestRun,
        ReviveOffer,
        BattlePassPreview,
        DoubleGoldAd,
        InstantChestAd,
        TripleDailyRewardAd
    }

    [Serializable]
    public class MonetizationTrigger
    {
        public TriggerType Type;
        public int MinPlayerLevel;
        public int MaxShowCount;
        public float CooldownHours;
        public bool NeverAfterFrustration;
        public bool NeverDuringGameplay;
    }

    public class MonetizationTriggerManager : MonoBehaviour
    {
        public static MonetizationTriggerManager Instance { get; private set; }

        [SerializeField] private int _starterPackLevel = 5;
        [SerializeField] private int _battlePassPreviewDay = 3;
        [SerializeField] private int _reviveCostGems = 50;
        [SerializeField] private int _maxRewardedAdsPerDay = 6;

        private readonly Dictionary<TriggerType, int> _showCounts = new();
        private readonly Dictionary<TriggerType, DateTime> _lastShown = new();
        private bool _starterPackPurchased;
        private bool _isFrustrated;
        private bool _isInGameplay;
        private int _rewardedAdsToday;
        private DateTime _lastAdResetDate;

        public int StarterPackLevel => _starterPackLevel;
        public int ReviveCostGems => _reviveCostGems;
        public bool StarterPackPurchased => _starterPackPurchased;

        public event Action<TriggerType> OnTriggerActivated;

        private void Awake()
        {
            Instance = this;
            _lastAdResetDate = DateTime.UtcNow.Date;
        }

        public bool ShouldShowTrigger(TriggerType type, int playerLevel, int daysPlayed,
            bool isBestRun, bool isDead, int bossFloor)
        {
            if (_isInGameplay && type != TriggerType.ReviveOffer) return false;
            if (_isFrustrated) return false;

            ResetDailyCountersIfNeeded();

            return type switch
            {
                TriggerType.StarterPack =>
                    !_starterPackPurchased &&
                    playerLevel >= _starterPackLevel &&
                    GetShowCount(type) == 0,

                TriggerType.GemPackAfterBestRun =>
                    isBestRun &&
                    !_isFrustrated &&
                    GetShowCount(type) < 3,

                TriggerType.ReviveOffer =>
                    isDead &&
                    bossFloor > 0 &&
                    GetShowCount(type) < 1,

                TriggerType.BattlePassPreview =>
                    daysPlayed >= _battlePassPreviewDay &&
                    GetShowCount(type) == 0,

                TriggerType.DoubleGoldAd =>
                    isDead &&
                    _rewardedAdsToday < _maxRewardedAdsPerDay &&
                    CanShowAd(type, 0.5f),

                TriggerType.InstantChestAd =>
                    _rewardedAdsToday < _maxRewardedAdsPerDay &&
                    CanShowAd(type, 1f),

                TriggerType.TripleDailyRewardAd =>
                    _rewardedAdsToday < _maxRewardedAdsPerDay &&
                    CanShowAd(type, 2f),

                _ => false
            };
        }

        public void RecordTriggerShown(TriggerType type)
        {
            _showCounts.TryGetValue(type, out int count);
            _showCounts[type] = count + 1;
            _lastShown[type] = DateTime.UtcNow;

            if (type == TriggerType.DoubleGoldAd ||
                type == TriggerType.InstantChestAd ||
                type == TriggerType.TripleDailyRewardAd)
            {
                _rewardedAdsToday++;
            }

            OnTriggerActivated?.Invoke(type);
        }

        public void SetStarterPackPurchased() => _starterPackPurchased = true;
        public void SetFrustrated(bool frustrated) => _isFrustrated = frustrated;
        public void SetInGameplay(bool inGameplay) => _isInGameplay = inGameplay;

        public int GetShowCount(TriggerType type)
        {
            return _showCounts.TryGetValue(type, out int count) ? count : 0;
        }

        public int GetRemainingAdsToday() => Mathf.Max(0, _maxRewardedAdsPerDay - _rewardedAdsToday);

        private bool CanShowAd(TriggerType type, float cooldownHours)
        {
            if (!_lastShown.TryGetValue(type, out var lastTime)) return true;
            return (DateTime.UtcNow - lastTime).TotalHours >= cooldownHours;
        }

        private void ResetDailyCountersIfNeeded()
        {
            if (DateTime.UtcNow.Date != _lastAdResetDate)
            {
                _rewardedAdsToday = 0;
                _lastAdResetDate = DateTime.UtcNow.Date;
            }
        }

        public void ResetForTesting()
        {
            _showCounts.Clear();
            _lastShown.Clear();
            _starterPackPurchased = false;
            _isFrustrated = false;
            _isInGameplay = false;
            _rewardedAdsToday = 0;
        }
    }
}

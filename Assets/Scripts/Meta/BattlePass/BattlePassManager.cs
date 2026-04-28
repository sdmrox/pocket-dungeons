using System;
using UnityEngine;

namespace PocketDungeons.Meta.BattlePass
{
    /// <summary>
    /// Manages Battle Pass progression. $4.99/season (6-week cycles).
    /// Free track gives basic rewards; premium unlocks cosmetics and bonus items.
    /// XP from quests, runs, and daily play.
    /// </summary>
    public class BattlePassManager : MonoBehaviour
    {
        public static BattlePassManager Instance { get; private set; }

        [SerializeField] private BattlePassData _currentSeason;

        private int _currentXP;
        private int _currentLevel;
        private bool _isPremium;
        private bool[] _freeRewardsClaimed;
        private bool[] _premiumRewardsClaimed;

        public BattlePassData CurrentSeason => _currentSeason;
        public int CurrentXP => _currentXP;
        public int CurrentLevel => _currentLevel;
        public bool IsPremium => _isPremium;

        public event Action<int, int> OnLevelUp; // newLevel, totalXP
        public event Action<int, bool> OnRewardClaimed; // tier, isPremium

        private void Awake()
        {
            Instance = this;

            if (_currentSeason != null && _currentSeason.Tiers != null)
            {
                _freeRewardsClaimed = new bool[_currentSeason.Tiers.Length];
                _premiumRewardsClaimed = new bool[_currentSeason.Tiers.Length];
            }
        }

        public void AddXP(int amount)
        {
            if (amount <= 0 || _currentSeason == null) return;

            _currentXP += amount;

            while (_currentLevel < _currentSeason.Tiers.Length)
            {
                int xpNeeded = _currentSeason.Tiers[_currentLevel].XPRequired;
                if (_currentXP >= xpNeeded)
                {
                    _currentXP -= xpNeeded;
                    _currentLevel++;
                    OnLevelUp?.Invoke(_currentLevel, _currentXP);
                }
                else
                {
                    break;
                }
            }
        }

        public bool TryClaimFreeReward(int tier)
        {
            if (tier >= _currentLevel || tier < 0) return false;
            if (_freeRewardsClaimed[tier]) return false;

            _freeRewardsClaimed[tier] = true;
            GrantReward(_currentSeason.Tiers[tier].FreeReward,
                       _currentSeason.Tiers[tier].FreeRewardId,
                       _currentSeason.Tiers[tier].FreeRewardAmount);
            OnRewardClaimed?.Invoke(tier, false);
            return true;
        }

        public bool TryClaimPremiumReward(int tier)
        {
            if (!_isPremium || tier >= _currentLevel || tier < 0) return false;
            if (_premiumRewardsClaimed[tier]) return false;

            _premiumRewardsClaimed[tier] = true;
            GrantReward(_currentSeason.Tiers[tier].PremiumReward,
                       _currentSeason.Tiers[tier].PremiumRewardId,
                       _currentSeason.Tiers[tier].PremiumRewardAmount);
            OnRewardClaimed?.Invoke(tier, true);
            return true;
        }

        public void ActivatePremium()
        {
            _isPremium = true;
        }

        public float GetLevelProgress()
        {
            if (_currentSeason == null || _currentLevel >= _currentSeason.Tiers.Length)
                return 1f;

            int xpNeeded = _currentSeason.Tiers[_currentLevel].XPRequired;
            return xpNeeded > 0 ? (float)_currentXP / xpNeeded : 1f;
        }

        private void GrantReward(BattlePassRewardType type, string rewardId, int amount)
        {
            switch (type)
            {
                case BattlePassRewardType.Gold:
                    Town.TownManager.Instance?.AddGold(amount);
                    break;
                case BattlePassRewardType.Gems:
                    // TODO: Add gems to player wallet
                    break;
                case BattlePassRewardType.CosmeticSkin:
                case BattlePassRewardType.PowerUpUnlock:
                case BattlePassRewardType.TitleBadge:
                case BattlePassRewardType.EmoteSlot:
                    // TODO: Unlock in cosmetics/collection system
                    break;
            }
        }
    }
}

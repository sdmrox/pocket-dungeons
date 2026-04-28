using System;
using UnityEngine;

namespace PocketDungeons.LiveOps
{
    /// <summary>
    /// Daily login rewards calendar. 7-day cycle with escalating rewards.
    /// Streak bonus for consecutive days. Resets if missed.
    /// </summary>
    public class DailyLoginManager : MonoBehaviour
    {
        public static DailyLoginManager Instance { get; private set; }

        [Serializable]
        public struct DailyReward
        {
            public int Gold;
            public int Gems;
            public int BattlePassXP;
            public string SpecialRewardId;
        }

        [SerializeField] private DailyReward[] _weeklyRewards = new DailyReward[7];
        [SerializeField] private float _streakMultiplier = 0.1f;

        private int _currentStreak;
        private int _currentDayInCycle;
        private string _lastLoginDate;

        public int CurrentStreak => _currentStreak;
        public int CurrentDayInCycle => _currentDayInCycle;

        public event Action<DailyReward, int> OnDailyRewardClaimed; // reward, streak

        private void Awake()
        {
            Instance = this;
        }

        public bool CheckAndClaimDailyReward()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (_lastLoginDate == today) return false;

            string yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
            if (_lastLoginDate == yesterday)
            {
                _currentStreak++;
            }
            else
            {
                _currentStreak = 1;
            }

            _lastLoginDate = today;
            _currentDayInCycle = (_currentDayInCycle + 1) % _weeklyRewards.Length;

            DailyReward reward = _weeklyRewards[_currentDayInCycle];
            GrantReward(reward);
            OnDailyRewardClaimed?.Invoke(reward, _currentStreak);

            return true;
        }

        private void GrantReward(DailyReward reward)
        {
            float multiplier = 1f + (_currentStreak - 1) * _streakMultiplier;

            int gold = Mathf.RoundToInt(reward.Gold * multiplier);
            int gems = Mathf.RoundToInt(reward.Gems * multiplier);
            int xp = Mathf.RoundToInt(reward.BattlePassXP * multiplier);

            Meta.Town.TownManager.Instance?.AddGold(gold);
            Meta.BattlePass.BattlePassManager.Instance?.AddXP(xp);

            Debug.Log($"[DailyLogin] Day {_currentDayInCycle + 1}, Streak: {_currentStreak}x, " +
                      $"Gold: {gold}, Gems: {gems}, BP XP: {xp}");
        }

        public void LoadState(int streak, int dayInCycle, string lastLogin)
        {
            _currentStreak = streak;
            _currentDayInCycle = dayInCycle;
            _lastLoginDate = lastLogin;
        }
    }
}

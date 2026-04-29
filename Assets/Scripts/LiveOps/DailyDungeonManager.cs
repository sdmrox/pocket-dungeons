using System;
using UnityEngine;

namespace PocketDungeons.LiveOps
{
    public class DailyDungeonManager : MonoBehaviour
    {
        public static DailyDungeonManager Instance { get; private set; }

        [SerializeField] private int _baseSeed = 42;

        private int _todaySeed;
        private bool _hasCompletedToday;
        private int _bestScore;
        private int _bestFloor;
        private DateTime _lastPlayedDate;

        public int TodaySeed => _todaySeed;
        public bool HasCompletedToday => _hasCompletedToday;
        public int BestScore => _bestScore;
        public int BestFloor => _bestFloor;

        public event Action<int, int> OnDailyCompleted; // score, floor
        public event Action OnNewDayStarted;

        private void Awake()
        {
            Instance = this;
            RefreshSeed();
        }

        public void RefreshSeed()
        {
            var today = DateTime.UtcNow.Date;

            if (_lastPlayedDate.Date != today)
            {
                _hasCompletedToday = false;
                _bestScore = 0;
                _bestFloor = 0;
                OnNewDayStarted?.Invoke();
            }

            _todaySeed = GenerateDailySeed(today);
            _lastPlayedDate = today;
        }

        public int GenerateDailySeed(DateTime date)
        {
            return _baseSeed ^ (date.Year * 10000 + date.Month * 100 + date.Day);
        }

        public void SubmitResult(int score, int floorsReached)
        {
            if (score > _bestScore)
            {
                _bestScore = score;
                _bestFloor = floorsReached;
            }

            _hasCompletedToday = true;
            OnDailyCompleted?.Invoke(score, floorsReached);
        }

        public bool CanPlay() => !_hasCompletedToday;

        public TimeSpan GetTimeUntilReset()
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(1);
            return tomorrow - now;
        }

        public void SetBaseSeed(int seed) => _baseSeed = seed;

        public void ResetForTesting()
        {
            _hasCompletedToday = false;
            _bestScore = 0;
            _bestFloor = 0;
        }
    }
}

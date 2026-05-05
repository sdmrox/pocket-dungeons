using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.LiveOps
{
    public enum ChallengeModifier
    {
        DoubleEnemies,
        SpeedRun,
        NoHeal,
        OneHitKill,
        RandomPowerUps,
        GlassCannon,
        TankMode,
        GoldRush
    }

    [Serializable]
    public class WeeklyChallenge
    {
        public string ChallengeId;
        public string DisplayName;
        public string Description;
        public ChallengeModifier Modifier;
        public float ModifierValue;
        public int RewardGold;
        public int RewardGems;
        public int TargetFloor;
        public float TimeLimit;
    }

    public class WeeklyChallengeManager : MonoBehaviour
    {
        public static WeeklyChallengeManager Instance { get; private set; }

        [SerializeField] private WeeklyChallenge[] _challengePool;

        private WeeklyChallenge _activeChallenge;
        private readonly Dictionary<string, int> _bestScores = new();
        private bool _hasCompletedThisWeek;

        public WeeklyChallenge ActiveChallenge => _activeChallenge;
        public bool HasCompletedThisWeek => _hasCompletedThisWeek;

        public event Action<WeeklyChallenge> OnChallengeStarted;
        public event Action<int> OnChallengeCompleted;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(WeeklyChallenge[] pool)
        {
            _challengePool = pool;
        }

        public void SelectWeeklyChallenge()
        {
            if (_challengePool == null || _challengePool.Length == 0) return;

            int weekNumber = GetWeekNumber(DateTime.UtcNow);
            int index = weekNumber % _challengePool.Length;
            _activeChallenge = _challengePool[index];
            _hasCompletedThisWeek = false;
            OnChallengeStarted?.Invoke(_activeChallenge);
        }

        public void SelectChallenge(int index)
        {
            if (_challengePool == null || index < 0 || index >= _challengePool.Length) return;
            _activeChallenge = _challengePool[index];
            _hasCompletedThisWeek = false;
            OnChallengeStarted?.Invoke(_activeChallenge);
        }

        public bool SubmitResult(int floorsReached, float timeElapsed, int score)
        {
            if (_activeChallenge == null) return false;

            bool success = floorsReached >= _activeChallenge.TargetFloor;
            if (_activeChallenge.TimeLimit > 0 && timeElapsed > _activeChallenge.TimeLimit)
                success = false;

            if (success)
            {
                _hasCompletedThisWeek = true;

                if (!_bestScores.TryGetValue(_activeChallenge.ChallengeId, out int best) || score > best)
                    _bestScores[_activeChallenge.ChallengeId] = score;

                OnChallengeCompleted?.Invoke(score);
            }

            return success;
        }

        public float ApplyModifier(ChallengeModifier modifier, float baseValue)
        {
            if (_activeChallenge == null || _activeChallenge.Modifier != modifier) return baseValue;

            return modifier switch
            {
                ChallengeModifier.DoubleEnemies => baseValue * 2f,
                ChallengeModifier.SpeedRun => baseValue * 1.5f,
                ChallengeModifier.GlassCannon => baseValue * 3f,
                ChallengeModifier.TankMode => baseValue * 0.5f,
                ChallengeModifier.GoldRush => baseValue * 2f,
                _ => baseValue * _activeChallenge.ModifierValue
            };
        }

        public int GetBestScore(string challengeId)
        {
            return _bestScores.TryGetValue(challengeId, out int score) ? score : 0;
        }

        public static int GetWeekNumber(DateTime date)
        {
            return (date.DayOfYear - 1) / 7 + 1;
        }

        public void ResetForTesting()
        {
            _activeChallenge = null;
            _hasCompletedThisWeek = false;
            _bestScores.Clear();
        }
    }
}

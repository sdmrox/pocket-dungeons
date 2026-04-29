using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.PostLaunch
{
    /// <summary>
    /// Retention tracking and engagement optimization.
    /// KPI targets: D1 >35%, D7 >15%, D30 >8%.
    /// North Star Metric: Daily Runs Completed per Active Player.
    /// </summary>
    public class RetentionManager : MonoBehaviour
    {
        public static RetentionManager Instance { get; private set; }

        private int _installDayNumber;
        private int _consecutiveLoginDays;
        private int _runsToday;
        private float _sessionStartTime;
        private readonly List<float> _sessionDurations = new();

        public int InstallDayNumber => _installDayNumber;
        public int ConsecutiveLoginDays => _consecutiveLoginDays;
        public int RunsToday => _runsToday;
        public float AverageSessionDuration => CalculateAverageSession();

        public event Action<int> OnMilestoneReached;
        public event Action<RetentionAlert> OnRetentionAlert;

        private void Awake()
        {
            Instance = this;
        }

        public void OnSessionStart()
        {
            _sessionStartTime = Time.realtimeSinceStartup;
            _installDayNumber++;

            // Check retention milestones
            CheckMilestones();
        }

        public void OnSessionEnd()
        {
            float duration = Time.realtimeSinceStartup - _sessionStartTime;
            _sessionDurations.Add(duration);

            Analytics.AnalyticsTracker.Instance?.TrackRunCompleted(
                0, 0, 0, duration, false, "session_end");
        }

        public void OnRunCompleted()
        {
            _runsToday++;
        }

        public void ResetDaily()
        {
            _runsToday = 0;
        }

        private void CheckMilestones()
        {
            int[] milestones = { 1, 3, 7, 14, 30, 60, 90 };
            foreach (int m in milestones)
            {
                if (_installDayNumber == m)
                {
                    OnMilestoneReached?.Invoke(m);
                    break;
                }
            }
        }

        public RetentionMetrics GetMetrics()
        {
            return new RetentionMetrics
            {
                DaysSinceInstall = _installDayNumber,
                ConsecutiveLoginDays = _consecutiveLoginDays,
                RunsToday = _runsToday,
                AverageSessionMinutes = AverageSessionDuration / 60f,
                TotalSessions = _sessionDurations.Count
            };
        }

        private float CalculateAverageSession()
        {
            if (_sessionDurations.Count == 0) return 0f;

            float total = 0f;
            foreach (float d in _sessionDurations)
                total += d;

            return total / _sessionDurations.Count;
        }

        public void LoadState(int installDay, int consecutiveDays, int totalSessions)
        {
            _installDayNumber = installDay;
            _consecutiveLoginDays = consecutiveDays;
        }
    }

    [Serializable]
    public struct RetentionMetrics
    {
        public int DaysSinceInstall;
        public int ConsecutiveLoginDays;
        public int RunsToday;
        public float AverageSessionMinutes;
        public int TotalSessions;
    }

    public enum RetentionAlert
    {
        ChurnRisk,
        EngagementDrop,
        SpendingStop,
        SessionShortening
    }
}

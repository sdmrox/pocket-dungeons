using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Social
{
    /// <summary>
    /// Game Center leaderboard integration.
    /// Leaderboards: Best Score, Deepest Floor, Fastest Boss Kill, Weekly Challenge.
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }

        public const string LeaderboardBestScore = "com.pocketdungeons.score";
        public const string LeaderboardDeepestFloor = "com.pocketdungeons.floor";
        public const string LeaderboardFastestBoss = "com.pocketdungeons.boss_time";
        public const string LeaderboardWeekly = "com.pocketdungeons.weekly";

        [Serializable]
        public struct LeaderboardEntry
        {
            public string PlayerId;
            public string PlayerName;
            public long Score;
            public int Rank;
        }

        private bool _isAuthenticated;
        public bool IsAuthenticated => _isAuthenticated;

        public event Action OnAuthenticated;
        public event Action<string, List<LeaderboardEntry>> OnScoresLoaded;

        private void Awake()
        {
            Instance = this;
        }

        public void Authenticate()
        {
            // TODO: GameKit authentication
            // GKLocalPlayer.local.authenticateHandler
            Debug.Log("[Leaderboard] Authenticating with Game Center...");
            _isAuthenticated = true;
            OnAuthenticated?.Invoke();
        }

        public void SubmitScore(string leaderboardId, long score)
        {
            if (!_isAuthenticated)
            {
                Debug.LogWarning("[Leaderboard] Not authenticated");
                return;
            }

            Debug.Log($"[Leaderboard] Submitting score {score} to {leaderboardId}");
            // TODO: GKLeaderboard.submitScore
        }

        public void LoadScores(string leaderboardId, int count = 25)
        {
            if (!_isAuthenticated) return;

            Debug.Log($"[Leaderboard] Loading top {count} scores for {leaderboardId}");
            // TODO: GKLeaderboard.loadEntries
        }

        public void ShowGameCenterUI()
        {
            Debug.Log("[Leaderboard] Showing Game Center dashboard");
            // TODO: GKAccessPoint.shared.trigger
        }

        public void SubmitRunResults(int score, int floorsCleared, float bossTime)
        {
            SubmitScore(LeaderboardBestScore, score);
            SubmitScore(LeaderboardDeepestFloor, floorsCleared);

            if (bossTime > 0f)
                SubmitScore(LeaderboardFastestBoss, (long)(bossTime * 1000));
        }
    }
}

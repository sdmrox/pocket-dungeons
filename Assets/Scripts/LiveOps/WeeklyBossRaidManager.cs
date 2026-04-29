using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.LiveOps
{
    [Serializable]
    public class RaidParticipant
    {
        public string PlayerId;
        public int DamageDealt;
        public DateTime JoinTime;
    }

    public class WeeklyBossRaidManager : MonoBehaviour
    {
        public static WeeklyBossRaidManager Instance { get; private set; }

        [SerializeField] private int _communityHPPool = 1000000;
        [SerializeField] private int _maxAttemptsPerPlayer = 3;

        private string _currentBossId;
        private int _remainingHP;
        private int _totalDamageDealt;
        private readonly Dictionary<string, RaidParticipant> _participants = new();
        private readonly Dictionary<string, int> _attemptCounts = new();
        private bool _isActive;
        private DateTime _raidEndTime;

        public string CurrentBossId => _currentBossId;
        public int RemainingHP => _remainingHP;
        public int TotalDamageDealt => _totalDamageDealt;
        public bool IsActive => _isActive;
        public int ParticipantCount => _participants.Count;
        public bool IsBossDefeated => _remainingHP <= 0;

        public event Action OnRaidStarted;
        public event Action OnBossDefeated;
        public event Action<string, int> OnDamageContributed;

        private void Awake()
        {
            Instance = this;
        }

        public void StartRaid(string bossId, int communityHP)
        {
            _currentBossId = bossId;
            _communityHPPool = communityHP;
            _remainingHP = communityHP;
            _totalDamageDealt = 0;
            _participants.Clear();
            _attemptCounts.Clear();
            _isActive = true;
            _raidEndTime = DateTime.UtcNow.Date.AddDays(7 - (int)DateTime.UtcNow.DayOfWeek);
            OnRaidStarted?.Invoke();
        }

        public bool ContributeDamage(string playerId, int damage)
        {
            if (!_isActive || _remainingHP <= 0) return false;

            _attemptCounts.TryGetValue(playerId, out int attempts);
            if (attempts >= _maxAttemptsPerPlayer) return false;

            _attemptCounts[playerId] = attempts + 1;

            int actualDamage = Mathf.Min(damage, _remainingHP);
            _remainingHP -= actualDamage;
            _totalDamageDealt += actualDamage;

            if (_participants.TryGetValue(playerId, out var participant))
            {
                participant.DamageDealt += actualDamage;
            }
            else
            {
                _participants[playerId] = new RaidParticipant
                {
                    PlayerId = playerId,
                    DamageDealt = actualDamage,
                    JoinTime = DateTime.UtcNow
                };
            }

            OnDamageContributed?.Invoke(playerId, actualDamage);

            if (_remainingHP <= 0)
            {
                _isActive = false;
                OnBossDefeated?.Invoke();
            }

            return true;
        }

        public int GetPlayerDamage(string playerId)
        {
            return _participants.TryGetValue(playerId, out var p) ? p.DamageDealt : 0;
        }

        public int GetAttemptsRemaining(string playerId)
        {
            _attemptCounts.TryGetValue(playerId, out int attempts);
            return Mathf.Max(0, _maxAttemptsPerPlayer - attempts);
        }

        public float GetBossHealthPercentage()
        {
            return _communityHPPool > 0 ? (float)_remainingHP / _communityHPPool : 0f;
        }

        public TimeSpan GetTimeRemaining()
        {
            return _raidEndTime > DateTime.UtcNow ? _raidEndTime - DateTime.UtcNow : TimeSpan.Zero;
        }

        public List<RaidParticipant> GetLeaderboard()
        {
            var sorted = new List<RaidParticipant>(_participants.Values);
            sorted.Sort((a, b) => b.DamageDealt.CompareTo(a.DamageDealt));
            return sorted;
        }
    }
}

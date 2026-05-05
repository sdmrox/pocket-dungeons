using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Social
{
    public enum ChallengeStatus
    {
        Pending,
        Accepted,
        Completed,
        Expired,
        Declined
    }

    [Serializable]
    public class FriendChallenge
    {
        public string ChallengeId;
        public string SenderId;
        public string SenderName;
        public string ReceiverId;
        public int SenderScore;
        public int SenderFloor;
        public int DungeonSeed;
        public ChallengeStatus Status;
        public DateTime CreatedAt;
        public DateTime ExpiresAt;
        public int ReceiverScore;
        public int ReceiverFloor;
    }

    public class FriendChallengeManager : MonoBehaviour
    {
        public static FriendChallengeManager Instance { get; private set; }

        [SerializeField] private int _challengeExpirationHours = 48;
        [SerializeField] private int _maxActiveChallenges = 10;

        private readonly Dictionary<string, FriendChallenge> _challenges = new();
        private readonly List<FriendChallenge> _incoming = new();
        private readonly List<FriendChallenge> _outgoing = new();

        public int ActiveChallengeCount => _challenges.Count;
        public IReadOnlyList<FriendChallenge> IncomingChallenges => _incoming;
        public IReadOnlyList<FriendChallenge> OutgoingChallenges => _outgoing;

        public event Action<FriendChallenge> OnChallengeReceived;
        public event Action<FriendChallenge> OnChallengeCompleted;
        public event Action<string> OnChallengeSent;

        private void Awake()
        {
            Instance = this;
        }

        public FriendChallenge CreateChallenge(string senderId, string senderName,
            string receiverId, int score, int floor, int seed)
        {
            if (_outgoing.Count >= _maxActiveChallenges) return null;

            var challenge = new FriendChallenge
            {
                ChallengeId = Guid.NewGuid().ToString(),
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = receiverId,
                SenderScore = score,
                SenderFloor = floor,
                DungeonSeed = seed,
                Status = ChallengeStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(_challengeExpirationHours)
            };

            _challenges[challenge.ChallengeId] = challenge;
            _outgoing.Add(challenge);
            OnChallengeSent?.Invoke(challenge.ChallengeId);

            // TODO: Send via Messages (MSMessagesAppViewController)
            return challenge;
        }

        public bool AcceptChallenge(string challengeId)
        {
            if (!_challenges.TryGetValue(challengeId, out var challenge)) return false;
            if (challenge.Status != ChallengeStatus.Pending) return false;
            if (DateTime.UtcNow > challenge.ExpiresAt)
            {
                challenge.Status = ChallengeStatus.Expired;
                return false;
            }

            challenge.Status = ChallengeStatus.Accepted;
            return true;
        }

        public bool SubmitChallengeResult(string challengeId, int score, int floor)
        {
            if (!_challenges.TryGetValue(challengeId, out var challenge)) return false;
            if (challenge.Status != ChallengeStatus.Accepted) return false;

            challenge.ReceiverScore = score;
            challenge.ReceiverFloor = floor;
            challenge.Status = ChallengeStatus.Completed;
            OnChallengeCompleted?.Invoke(challenge);
            return true;
        }

        public bool DeclineChallenge(string challengeId)
        {
            if (!_challenges.TryGetValue(challengeId, out var challenge)) return false;
            if (challenge.Status != ChallengeStatus.Pending) return false;

            challenge.Status = ChallengeStatus.Declined;
            return true;
        }

        public void ReceiveChallenge(FriendChallenge challenge)
        {
            _challenges[challenge.ChallengeId] = challenge;
            _incoming.Add(challenge);
            OnChallengeReceived?.Invoke(challenge);
        }

        public FriendChallenge GetChallenge(string challengeId)
        {
            return _challenges.TryGetValue(challengeId, out var c) ? c : null;
        }

        public bool DidSenderWin(string challengeId)
        {
            if (!_challenges.TryGetValue(challengeId, out var c)) return false;
            if (c.Status != ChallengeStatus.Completed) return false;
            return c.SenderScore > c.ReceiverScore;
        }

        public string GenerateShareText(FriendChallenge challenge)
        {
            return $"I scored {challenge.SenderScore} on Floor {challenge.SenderFloor} in Pocket Dungeons! " +
                   $"Can you beat my score? Challenge ID: {challenge.ChallengeId}";
        }

        public void CleanupExpired()
        {
            var expired = new List<string>();
            foreach (var kvp in _challenges)
            {
                if (DateTime.UtcNow > kvp.Value.ExpiresAt && kvp.Value.Status == ChallengeStatus.Pending)
                {
                    kvp.Value.Status = ChallengeStatus.Expired;
                    expired.Add(kvp.Key);
                }
            }
        }
    }
}

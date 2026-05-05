using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class FriendChallengeTests
    {
        enum ChallengeStatus { Pending, Accepted, Completed, Expired, Declined }

        class Challenge
        {
            public string Id = Guid.NewGuid().ToString();
            public string SenderId;
            public string ReceiverId;
            public int SenderScore;
            public int SenderFloor;
            public int DungeonSeed;
            public ChallengeStatus Status = ChallengeStatus.Pending;
            public DateTime ExpiresAt;
            public int ReceiverScore;
            public int ReceiverFloor;
        }

        Dictionary<string, Challenge> _challenges = new();
        int _maxActive = 10;

        [SetUp]
        public void SetUp()
        {
            _challenges.Clear();
        }

        Challenge Create(string sender, string receiver, int score, int floor, int seed)
        {
            if (_challenges.Count >= _maxActive) return null;
            var c = new Challenge
            {
                SenderId = sender,
                ReceiverId = receiver,
                SenderScore = score,
                SenderFloor = floor,
                DungeonSeed = seed,
                ExpiresAt = DateTime.UtcNow.AddHours(48)
            };
            _challenges[c.Id] = c;
            return c;
        }

        bool Accept(string id)
        {
            if (!_challenges.TryGetValue(id, out var c)) return false;
            if (c.Status != ChallengeStatus.Pending) return false;
            if (DateTime.UtcNow > c.ExpiresAt) { c.Status = ChallengeStatus.Expired; return false; }
            c.Status = ChallengeStatus.Accepted;
            return true;
        }

        bool Submit(string id, int score, int floor)
        {
            if (!_challenges.TryGetValue(id, out var c)) return false;
            if (c.Status != ChallengeStatus.Accepted) return false;
            c.ReceiverScore = score;
            c.ReceiverFloor = floor;
            c.Status = ChallengeStatus.Completed;
            return true;
        }

        bool Decline(string id)
        {
            if (!_challenges.TryGetValue(id, out var c)) return false;
            if (c.Status != ChallengeStatus.Pending) return false;
            c.Status = ChallengeStatus.Declined;
            return true;
        }

        [Test]
        public void Create_AddsChallenge()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Assert.That(c, Is.Not.Null);
            Assert.That(_challenges.Count, Is.EqualTo(1));
            Assert.That(c.Status, Is.EqualTo(ChallengeStatus.Pending));
        }

        [Test]
        public void Create_RejectsOverMaxActive()
        {
            for (int i = 0; i < _maxActive; i++)
                Create("p1", $"p{i + 2}", 100, 5, i);
            var overflow = Create("p1", "p99", 100, 5, 9999);
            Assert.That(overflow, Is.Null);
        }

        [Test]
        public void Accept_ChangesStatus()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Assert.That(Accept(c.Id), Is.True);
            Assert.That(c.Status, Is.EqualTo(ChallengeStatus.Accepted));
        }

        [Test]
        public void Submit_CompletesChallenge()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Accept(c.Id);
            Assert.That(Submit(c.Id, 600, 12), Is.True);
            Assert.That(c.Status, Is.EqualTo(ChallengeStatus.Completed));
            Assert.That(c.ReceiverScore, Is.EqualTo(600));
        }

        [Test]
        public void Submit_FailsIfNotAccepted()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Assert.That(Submit(c.Id, 600, 12), Is.False);
        }

        [Test]
        public void Decline_FailsIfNotPending()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Accept(c.Id);
            Assert.That(Decline(c.Id), Is.False);
        }

        [Test]
        public void SenderWins_WhenHigherScore()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Accept(c.Id);
            Submit(c.Id, 400, 8);
            Assert.That(c.SenderScore > c.ReceiverScore, Is.True);
        }

        [Test]
        public void ReceiverWins_WhenHigherScore()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Accept(c.Id);
            Submit(c.Id, 700, 15);
            Assert.That(c.ReceiverScore > c.SenderScore, Is.True);
        }

        [Test]
        public void ShareText_ContainsScore()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            string text = $"I scored {c.SenderScore} on Floor {c.SenderFloor} in Pocket Dungeons!";
            Assert.That(text, Does.Contain("500"));
            Assert.That(text, Does.Contain("Floor 10"));
        }

        [Test]
        public void Challenge_HasSameSeedForBothPlayers()
        {
            var c = Create("p1", "p2", 500, 10, 12345);
            Assert.That(c.DungeonSeed, Is.EqualTo(12345));
        }
    }
}

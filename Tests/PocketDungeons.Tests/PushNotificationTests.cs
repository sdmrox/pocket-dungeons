using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class PushNotificationTests
    {
        enum NotificationType { EnergyFull, DailyChallenge, StreakReminder, WeeklyReset, BattlePassExpiring }

        class Notification
        {
            public string Id = Guid.NewGuid().ToString();
            public NotificationType Type;
            public string Title;
            public string Body;
            public DateTime ScheduledTime;
            public bool IsRepeating;
        }

        Dictionary<string, Notification> _scheduled = new();
        HashSet<NotificationType> _disabled = new();
        bool _authorized;
        bool _enabled = true;

        [SetUp]
        public void SetUp()
        {
            _scheduled.Clear();
            _disabled.Clear();
            _authorized = true;
            _enabled = true;
        }

        string Schedule(NotificationType type, string title, string body, TimeSpan delay, bool repeat = false)
        {
            if (!_authorized || !_enabled) return null;
            if (_disabled.Contains(type)) return null;

            var n = new Notification
            {
                Type = type,
                Title = title,
                Body = body,
                ScheduledTime = DateTime.UtcNow + delay,
                IsRepeating = repeat
            };
            _scheduled[n.Id] = n;
            return n.Id;
        }

        [Test]
        public void Schedule_ReturnsId()
        {
            var id = Schedule(NotificationType.DailyChallenge, "Test", "Body", TimeSpan.FromHours(1));
            Assert.That(id, Is.Not.Null);
            Assert.That(_scheduled.ContainsKey(id), Is.True);
        }

        [Test]
        public void Schedule_RejectsWhenUnauthorized()
        {
            _authorized = false;
            var id = Schedule(NotificationType.DailyChallenge, "Test", "Body", TimeSpan.FromHours(1));
            Assert.That(id, Is.Null);
        }

        [Test]
        public void Schedule_RejectsWhenDisabled()
        {
            _enabled = false;
            var id = Schedule(NotificationType.DailyChallenge, "Test", "Body", TimeSpan.FromHours(1));
            Assert.That(id, Is.Null);
        }

        [Test]
        public void Schedule_RejectsDisabledType()
        {
            _disabled.Add(NotificationType.StreakReminder);
            var id = Schedule(NotificationType.StreakReminder, "Test", "Body", TimeSpan.FromHours(1));
            Assert.That(id, Is.Null);
        }

        [Test]
        public void Cancel_RemovesNotification()
        {
            var id = Schedule(NotificationType.DailyChallenge, "Test", "Body", TimeSpan.FromHours(1));
            Assert.That(_scheduled.Remove(id), Is.True);
            Assert.That(_scheduled.ContainsKey(id), Is.False);
        }

        [Test]
        public void CancelAll_ClearsSchedule()
        {
            Schedule(NotificationType.DailyChallenge, "A", "B", TimeSpan.FromHours(1));
            Schedule(NotificationType.StreakReminder, "C", "D", TimeSpan.FromHours(2));
            _scheduled.Clear();
            Assert.That(_scheduled.Count, Is.EqualTo(0));
        }

        [Test]
        public void TypeToggle_WorksCorrectly()
        {
            _disabled.Add(NotificationType.EnergyFull);
            Assert.That(_disabled.Contains(NotificationType.EnergyFull), Is.True);
            _disabled.Remove(NotificationType.EnergyFull);
            Assert.That(_disabled.Contains(NotificationType.EnergyFull), Is.False);
        }

        [Test]
        public void RepeatingNotification_FlagIsSet()
        {
            var id = Schedule(NotificationType.DailyChallenge, "Daily", "Go!", TimeSpan.FromHours(24), true);
            Assert.That(_scheduled[id].IsRepeating, Is.True);
        }

        [Test]
        public void ScheduledTime_IsInFuture()
        {
            var id = Schedule(NotificationType.DailyChallenge, "Test", "Body", TimeSpan.FromHours(1));
            Assert.That(_scheduled[id].ScheduledTime, Is.GreaterThan(DateTime.UtcNow));
        }
    }
}

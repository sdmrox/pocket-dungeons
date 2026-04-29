using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.LiveOps
{
    public enum NotificationType
    {
        EnergyFull,
        DailyChallenge,
        StreakReminder,
        WeeklyReset,
        BattlePassExpiring,
        EventStarting,
        FriendChallenge,
        LimitedOffer
    }

    [Serializable]
    public class ScheduledNotification
    {
        public string NotificationId;
        public NotificationType Type;
        public string Title;
        public string Body;
        public DateTime ScheduledTime;
        public bool IsRepeating;
        public TimeSpan RepeatInterval;
    }

    public class PushNotificationManager : MonoBehaviour
    {
        public static PushNotificationManager Instance { get; private set; }

        private bool _isAuthorized;
        private bool _isEnabled = true;
        private readonly Dictionary<string, ScheduledNotification> _scheduled = new();
        private readonly HashSet<NotificationType> _disabledTypes = new();

        public bool IsAuthorized => _isAuthorized;
        public bool IsEnabled => _isEnabled;
        public int ScheduledCount => _scheduled.Count;

        public event Action<bool> OnAuthorizationResult;
        public event Action<string> OnNotificationScheduled;

        private void Awake()
        {
            Instance = this;
        }

        public void RequestAuthorization()
        {
            // TODO: UNUserNotificationCenter.requestAuthorization
            _isAuthorized = true;
            OnAuthorizationResult?.Invoke(true);
        }

        public string ScheduleNotification(NotificationType type, string title, string body,
            TimeSpan delay, bool repeating = false, TimeSpan repeatInterval = default)
        {
            if (!_isAuthorized || !_isEnabled) return null;
            if (_disabledTypes.Contains(type)) return null;

            var notification = new ScheduledNotification
            {
                NotificationId = Guid.NewGuid().ToString(),
                Type = type,
                Title = title,
                Body = body,
                ScheduledTime = DateTime.UtcNow + delay,
                IsRepeating = repeating,
                RepeatInterval = repeatInterval
            };

            _scheduled[notification.NotificationId] = notification;
            OnNotificationScheduled?.Invoke(notification.NotificationId);

            // TODO: UNMutableNotificationContent + UNTimeIntervalNotificationTrigger
            return notification.NotificationId;
        }

        public bool CancelNotification(string notificationId)
        {
            return _scheduled.Remove(notificationId);
        }

        public void CancelAllNotifications()
        {
            _scheduled.Clear();
            // TODO: UNUserNotificationCenter.removeAllPendingNotificationRequests
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
            if (!enabled)
                CancelAllNotifications();
        }

        public void SetTypeEnabled(NotificationType type, bool enabled)
        {
            if (enabled)
                _disabledTypes.Remove(type);
            else
                _disabledTypes.Add(type);
        }

        public bool IsTypeEnabled(NotificationType type) => !_disabledTypes.Contains(type);

        public ScheduledNotification GetNotification(string id)
        {
            return _scheduled.TryGetValue(id, out var n) ? n : null;
        }

        public void ScheduleDefaultNotifications()
        {
            ScheduleNotification(NotificationType.DailyChallenge,
                "New Daily Dungeon!",
                "A new dungeon awaits. Can you top the leaderboard?",
                TimeSpan.FromHours(24), true, TimeSpan.FromHours(24));

            ScheduleNotification(NotificationType.StreakReminder,
                "Don't lose your streak!",
                "Your daily login streak is at risk. Come back for your reward!",
                TimeSpan.FromHours(20));
        }
    }
}

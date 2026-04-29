using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.LiveOps
{
    /// <summary>
    /// Manages seasonal/limited-time events (holiday events, weekly challenges, etc.).
    /// Events are defined remotely and fetched at startup.
    /// Each event has start/end times, special rules, and exclusive rewards.
    /// </summary>
    public class SeasonalEventManager : MonoBehaviour
    {
        public static SeasonalEventManager Instance { get; private set; }

        [Serializable]
        public class SeasonalEvent
        {
            public string EventId;
            public string DisplayName;
            public string Description;
            public string StartDate;
            public string EndDate;
            public EventType Type;
            public string SpecialBiomeId;
            public string[] ExclusiveRewards;
            public Dictionary<string, float> ModifiedParameters;
        }

        public enum EventType
        {
            HolidayEvent,       // Halloween, Christmas, Lunar New Year
            WeeklyChallenge,    // Rotating modifiers
            BossRush,           // Boss-only floors
            GoldRush,           // 2x gold weekend
            HeroSpotlight,      // Bonus XP for specific hero
            CommunityGoal       // Server-wide kill count
        }

        [SerializeField] private TextAsset _defaultEventsJson;

        private readonly List<SeasonalEvent> _activeEvents = new();
        private readonly List<SeasonalEvent> _upcomingEvents = new();

        public IReadOnlyList<SeasonalEvent> ActiveEvents => _activeEvents;
        public IReadOnlyList<SeasonalEvent> UpcomingEvents => _upcomingEvents;

        public event Action<SeasonalEvent> OnEventStarted;
        public event Action<SeasonalEvent> OnEventEnded;

        private void Awake()
        {
            Instance = this;
        }

        public void RefreshEvents()
        {
            // TODO: Fetch from Firebase Remote Config or custom backend
            Debug.Log("[LiveOps] Refreshing seasonal events...");

            DateTime now = DateTime.UtcNow;
            _activeEvents.Clear();
            _upcomingEvents.Clear();

            // Parse local defaults or remote config
        }

        public void CheckEventTransitions()
        {
            DateTime now = DateTime.UtcNow;

            for (int i = _activeEvents.Count - 1; i >= 0; i--)
            {
                if (DateTime.TryParse(_activeEvents[i].EndDate, out DateTime end) && now > end)
                {
                    var ended = _activeEvents[i];
                    _activeEvents.RemoveAt(i);
                    OnEventEnded?.Invoke(ended);
                }
            }

            for (int i = _upcomingEvents.Count - 1; i >= 0; i--)
            {
                if (DateTime.TryParse(_upcomingEvents[i].StartDate, out DateTime start) && now >= start)
                {
                    var started = _upcomingEvents[i];
                    _upcomingEvents.RemoveAt(i);
                    _activeEvents.Add(started);
                    OnEventStarted?.Invoke(started);
                }
            }
        }

        public bool IsEventActive(string eventId)
        {
            foreach (var e in _activeEvents)
                if (e.EventId == eventId) return true;
            return false;
        }

        public float GetModifiedParameter(string paramName, float defaultValue)
        {
            foreach (var e in _activeEvents)
            {
                if (e.ModifiedParameters != null && e.ModifiedParameters.TryGetValue(paramName, out float val))
                    return val;
            }
            return defaultValue;
        }
    }
}

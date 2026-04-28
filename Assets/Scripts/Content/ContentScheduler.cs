using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Content
{
    /// <summary>
    /// Manages post-launch content cadence:
    /// - Weekly: New quest rotations, challenge modifiers
    /// - Bi-weekly: New enemy variants, power-ups
    /// - Monthly: New hero, new biome, Battle Pass season
    /// - Quarterly: Major content update (new game mode, story chapter)
    /// </summary>
    public class ContentScheduler : MonoBehaviour
    {
        public static ContentScheduler Instance { get; private set; }

        [Serializable]
        public class ContentDrop
        {
            public string Id;
            public string Name;
            public ContentType Type;
            public string ReleaseDate;
            public string Version;
            public bool RequiresUpdate;
            public string[] AssetBundles;
        }

        public enum ContentType
        {
            QuestRotation,
            ChallengeModifier,
            EnemyVariant,
            PowerUp,
            Hero,
            Biome,
            BattlePassSeason,
            GameMode,
            StoryChapter
        }

        [SerializeField] private TextAsset _contentCalendarJson;

        private readonly List<ContentDrop> _scheduledDrops = new();
        private readonly List<ContentDrop> _releasedContent = new();

        public IReadOnlyList<ContentDrop> ScheduledDrops => _scheduledDrops;
        public IReadOnlyList<ContentDrop> ReleasedContent => _releasedContent;

        public event Action<ContentDrop> OnNewContentAvailable;

        private void Awake()
        {
            Instance = this;
        }

        public void CheckForNewContent()
        {
            DateTime now = DateTime.UtcNow;

            for (int i = _scheduledDrops.Count - 1; i >= 0; i--)
            {
                if (DateTime.TryParse(_scheduledDrops[i].ReleaseDate, out DateTime release) && now >= release)
                {
                    var drop = _scheduledDrops[i];
                    _scheduledDrops.RemoveAt(i);
                    _releasedContent.Add(drop);
                    OnNewContentAvailable?.Invoke(drop);
                }
            }
        }

        public void LoadContentCalendar(string json)
        {
            // TODO: Parse content calendar from remote config
            Debug.Log("[Content] Loading content calendar...");
        }

        public ContentDrop GetNextDrop(ContentType type)
        {
            DateTime now = DateTime.UtcNow;
            ContentDrop nearest = null;
            DateTime nearestDate = DateTime.MaxValue;

            foreach (var drop in _scheduledDrops)
            {
                if (drop.Type != type) continue;
                if (DateTime.TryParse(drop.ReleaseDate, out DateTime d) && d < nearestDate && d > now)
                {
                    nearest = drop;
                    nearestDate = d;
                }
            }

            return nearest;
        }
    }
}

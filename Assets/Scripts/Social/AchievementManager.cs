using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Social
{
    /// <summary>
    /// Game Center achievements. Tracks progress and reports to GameKit.
    /// Categories: Combat, Exploration, Collection, Mastery.
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [Serializable]
        public struct AchievementDef
        {
            public string Id;
            public string DisplayName;
            public string Description;
            public int TargetValue;
            public bool IsHidden;
        }

        [SerializeField] private AchievementDef[] _achievements;

        private readonly Dictionary<string, float> _progress = new();

        public event Action<string> OnAchievementUnlocked;

        private void Awake()
        {
            Instance = this;
        }

        public void ReportProgress(string achievementId, int currentValue)
        {
            AchievementDef? def = FindAchievement(achievementId);
            if (def == null) return;

            float percent = Mathf.Clamp01((float)currentValue / def.Value.TargetValue) * 100f;

            if (_progress.TryGetValue(achievementId, out float existing) && existing >= percent)
                return;

            _progress[achievementId] = percent;

            Debug.Log($"[Achievement] {achievementId}: {percent:F0}%");

            // TODO: GKAchievement.report

            if (percent >= 100f)
                OnAchievementUnlocked?.Invoke(achievementId);
        }

        public void ReportIncremental(string achievementId, int increment)
        {
            AchievementDef? def = FindAchievement(achievementId);
            if (def == null) return;

            float currentPercent = _progress.TryGetValue(achievementId, out float p) ? p : 0f;
            float incrementPercent = ((float)increment / def.Value.TargetValue) * 100f;
            float newPercent = Mathf.Min(currentPercent + incrementPercent, 100f);

            _progress[achievementId] = newPercent;

            if (newPercent >= 100f && currentPercent < 100f)
                OnAchievementUnlocked?.Invoke(achievementId);
        }

        public float GetProgress(string achievementId)
        {
            return _progress.TryGetValue(achievementId, out float p) ? p : 0f;
        }

        private AchievementDef? FindAchievement(string id)
        {
            foreach (var a in _achievements)
            {
                if (a.Id == id) return a;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Meta.Quests
{
    /// <summary>
    /// Manages daily (3) and weekly (3) quests. Resets on schedule.
    /// Tracks progress and distributes rewards on completion.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest Pools")]
        [SerializeField] private QuestData[] _dailyQuestPool;
        [SerializeField] private QuestData[] _weeklyQuestPool;
        [SerializeField] private int _dailyQuestCount = 3;
        [SerializeField] private int _weeklyQuestCount = 3;

        private readonly List<QuestProgress> _activeQuests = new();

        public IReadOnlyList<QuestProgress> ActiveQuests => _activeQuests;
        public event Action<QuestProgress> OnQuestCompleted;
        public event Action<QuestProgress> OnQuestProgressUpdated;

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateDailyQuests()
        {
            _activeQuests.RemoveAll(q => q.Data.Type == QuestType.Daily);
            AddRandomQuests(_dailyQuestPool, _dailyQuestCount, QuestType.Daily);
        }

        public void GenerateWeeklyQuests()
        {
            _activeQuests.RemoveAll(q => q.Data.Type == QuestType.Weekly);
            AddRandomQuests(_weeklyQuestPool, _weeklyQuestCount, QuestType.Weekly);
        }

        public void ReportProgress(QuestObjective objective, int amount = 1, string parameter = null)
        {
            foreach (var quest in _activeQuests)
            {
                if (quest.IsCompleted) continue;
                if (quest.Data.Objective != objective) continue;

                if (!string.IsNullOrEmpty(quest.Data.TargetParameter) &&
                    quest.Data.TargetParameter != parameter)
                    continue;

                quest.CurrentAmount += amount;
                OnQuestProgressUpdated?.Invoke(quest);

                if (quest.CurrentAmount >= quest.Data.TargetAmount)
                {
                    quest.IsCompleted = true;
                    OnQuestCompleted?.Invoke(quest);
                }
            }
        }

        public void ClaimReward(QuestProgress quest)
        {
            if (!quest.IsCompleted || quest.IsClaimed) return;

            quest.IsClaimed = true;

            Town.TownManager.Instance?.AddGold(quest.Data.GoldReward);
            BattlePass.BattlePassManager.Instance?.AddXP(quest.Data.BattlePassXP);
        }

        private void AddRandomQuests(QuestData[] pool, int count, QuestType type)
        {
            if (pool == null || pool.Length == 0) return;

            var shuffled = new List<QuestData>(pool);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }

            for (int i = 0; i < Mathf.Min(count, shuffled.Count); i++)
            {
                _activeQuests.Add(new QuestProgress { Data = shuffled[i] });
            }
        }
    }

    [Serializable]
    public class QuestProgress
    {
        public QuestData Data;
        public int CurrentAmount;
        public bool IsCompleted;
        public bool IsClaimed;

        public float Progress => (float)CurrentAmount / Data.TargetAmount;
    }
}

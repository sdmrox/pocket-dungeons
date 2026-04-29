using System;
using System.Collections.Generic;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Meta.Progression
{
    public enum UnlockConditionType
    {
        FloorReached,
        EnemiesKilled,
        RunsCompleted,
        BossDefeated,
        GoldEarned,
        AchievementCompleted,
        Default
    }

    [Serializable]
    public class HeroUnlockCondition
    {
        public string HeroId;
        public UnlockConditionType Type;
        public int RequiredValue;
        public string RequiredBossId;
    }

    public class HeroUnlockManager : MonoBehaviour
    {
        public static HeroUnlockManager Instance { get; private set; }

        [SerializeField] private HeroUnlockCondition[] _conditions;

        private readonly HashSet<string> _unlockedHeroes = new();
        private readonly Dictionary<string, HeroUnlockCondition> _conditionMap = new();

        public event Action<string> OnHeroUnlocked;

        private void Awake()
        {
            Instance = this;
            _unlockedHeroes.Add("Warrior");

            if (_conditions != null)
            {
                foreach (var cond in _conditions)
                    _conditionMap[cond.HeroId] = cond;
            }
        }

        public void Initialize(HeroUnlockCondition[] conditions, List<string> alreadyUnlocked)
        {
            _conditionMap.Clear();
            if (conditions != null)
            {
                foreach (var cond in conditions)
                    _conditionMap[cond.HeroId] = cond;
            }

            _unlockedHeroes.Clear();
            _unlockedHeroes.Add("Warrior");
            if (alreadyUnlocked != null)
            {
                foreach (var id in alreadyUnlocked)
                    _unlockedHeroes.Add(id);
            }
        }

        public bool CheckUnlock(string heroId, int floorsReached, int enemiesKilled,
            int runsCompleted, string bossDefeated, int goldEarned)
        {
            if (_unlockedHeroes.Contains(heroId)) return false;
            if (!_conditionMap.TryGetValue(heroId, out var cond)) return false;

            bool met = cond.Type switch
            {
                UnlockConditionType.FloorReached => floorsReached >= cond.RequiredValue,
                UnlockConditionType.EnemiesKilled => enemiesKilled >= cond.RequiredValue,
                UnlockConditionType.RunsCompleted => runsCompleted >= cond.RequiredValue,
                UnlockConditionType.BossDefeated => bossDefeated == cond.RequiredBossId,
                UnlockConditionType.GoldEarned => goldEarned >= cond.RequiredValue,
                UnlockConditionType.Default => true,
                _ => false
            };

            if (met)
            {
                _unlockedHeroes.Add(heroId);
                OnHeroUnlocked?.Invoke(heroId);
            }

            return met;
        }

        public bool IsUnlocked(string heroId) => _unlockedHeroes.Contains(heroId);
        public int UnlockedCount => _unlockedHeroes.Count;
        public IReadOnlyCollection<string> GetUnlockedHeroes() => _unlockedHeroes;

        public HeroUnlockCondition GetCondition(string heroId)
        {
            return _conditionMap.TryGetValue(heroId, out var cond) ? cond : null;
        }

        public float GetProgress(string heroId, int currentValue)
        {
            if (_unlockedHeroes.Contains(heroId)) return 1f;
            if (!_conditionMap.TryGetValue(heroId, out var cond)) return 0f;
            if (cond.RequiredValue <= 0) return 0f;
            return Mathf.Clamp01((float)currentValue / cond.RequiredValue);
        }
    }
}

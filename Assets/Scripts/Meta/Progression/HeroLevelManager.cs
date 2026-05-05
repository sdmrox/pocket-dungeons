using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Meta.Progression
{
    [Serializable]
    public class HeroLevelData
    {
        public string HeroId;
        public int Level = 1;
        public int CurrentXP;
        public int TotalXP;
    }

    public class HeroLevelManager : MonoBehaviour
    {
        public static HeroLevelManager Instance { get; private set; }

        [SerializeField] private int _baseXPRequired = 100;
        [SerializeField] private float _xpScalePerLevel = 1.15f;
        [SerializeField] private int _maxLevel = 50;

        [Header("Stat Growth Per Level")]
        [SerializeField] private float _healthPerLevel = 5f;
        [SerializeField] private float _damagePerLevel = 1f;

        private readonly Dictionary<string, HeroLevelData> _heroLevels = new();

        public int MaxLevel => _maxLevel;
        public event Action<string, int> OnHeroLevelUp;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(int baseXP, float xpScale, int maxLevel)
        {
            _baseXPRequired = baseXP;
            _xpScalePerLevel = xpScale;
            _maxLevel = maxLevel;
        }

        public int AddXP(string heroId, int xpAmount)
        {
            if (xpAmount <= 0) return 0;

            var data = GetOrCreateData(heroId);
            if (data.Level >= _maxLevel) return 0;

            data.CurrentXP += xpAmount;
            data.TotalXP += xpAmount;

            int levelsGained = 0;
            while (data.Level < _maxLevel)
            {
                int required = GetXPRequired(data.Level);
                if (data.CurrentXP < required) break;

                data.CurrentXP -= required;
                data.Level++;
                levelsGained++;
                OnHeroLevelUp?.Invoke(heroId, data.Level);
            }

            if (data.Level >= _maxLevel)
                data.CurrentXP = 0;

            return levelsGained;
        }

        public int GetXPRequired(int level)
        {
            return Mathf.RoundToInt(_baseXPRequired * Mathf.Pow(_xpScalePerLevel, level - 1));
        }

        public int GetLevel(string heroId)
        {
            return _heroLevels.TryGetValue(heroId, out var data) ? data.Level : 1;
        }

        public int GetCurrentXP(string heroId)
        {
            return _heroLevels.TryGetValue(heroId, out var data) ? data.CurrentXP : 0;
        }

        public float GetLevelProgress(string heroId)
        {
            var data = GetOrCreateData(heroId);
            if (data.Level >= _maxLevel) return 1f;
            int required = GetXPRequired(data.Level);
            return required > 0 ? (float)data.CurrentXP / required : 0f;
        }

        public int GetBonusHealth(string heroId)
        {
            int level = GetLevel(heroId);
            return Mathf.RoundToInt(_healthPerLevel * (level - 1));
        }

        public int GetBonusDamage(string heroId)
        {
            int level = GetLevel(heroId);
            return Mathf.RoundToInt(_damagePerLevel * (level - 1));
        }

        public void LoadState(Dictionary<string, HeroLevelData> levels)
        {
            _heroLevels.Clear();
            if (levels != null)
            {
                foreach (var kvp in levels)
                    _heroLevels[kvp.Key] = kvp.Value;
            }
        }

        public Dictionary<string, HeroLevelData> GetAllLevels() => new(_heroLevels);

        private HeroLevelData GetOrCreateData(string heroId)
        {
            if (!_heroLevels.TryGetValue(heroId, out var data))
            {
                data = new HeroLevelData { HeroId = heroId, Level = 1 };
                _heroLevels[heroId] = data;
            }
            return data;
        }
    }
}

using UnityEngine;

namespace PocketDungeons.Gameplay.Balance
{
    /// <summary>
    /// Adaptive difficulty system. Adjusts based on player performance.
    /// Tracks death rate, floor completion time, and damage taken.
    /// Subtly adjusts enemy HP/damage and drop rates to maintain flow state.
    /// </summary>
    public class DifficultyManager : MonoBehaviour
    {
        public static DifficultyManager Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private BalanceConfig _balance;
        [SerializeField] private float _adjustmentRate = 0.05f;
        [SerializeField] private float _minMultiplier = 0.6f;
        [SerializeField] private float _maxMultiplier = 1.4f;

        private float _currentMultiplier = 1f;
        private int _recentDeaths;
        private int _recentFloors;
        private float _avgFloorTime;

        public float CurrentMultiplier => _currentMultiplier;
        public string DifficultyLabel => _currentMultiplier switch
        {
            < 0.8f => "Easy",
            < 1.1f => "Normal",
            < 1.3f => "Hard",
            _ => "Very Hard"
        };

        private void Awake()
        {
            Instance = this;
        }

        public void RecordDeath(int floor, float runTime)
        {
            _recentDeaths++;

            // If dying too frequently on early floors, reduce difficulty
            if (floor <= 5 && _recentDeaths >= 3)
            {
                AdjustDifficulty(-_adjustmentRate * 2);
                _recentDeaths = 0;
            }
            else if (_recentDeaths >= 5)
            {
                AdjustDifficulty(-_adjustmentRate);
                _recentDeaths = 0;
            }
        }

        public void RecordFloorCleared(int floor, float floorTime)
        {
            _recentFloors++;
            _avgFloorTime = (_avgFloorTime * (_recentFloors - 1) + floorTime) / _recentFloors;

            // If clearing floors too fast, increase difficulty
            float targetFloorTime = _balance != null ? _balance.TargetRunDurationSeconds / 10f : 15f;

            if (_avgFloorTime < targetFloorTime * 0.5f && _recentFloors >= 5)
            {
                AdjustDifficulty(_adjustmentRate);
                _recentFloors = 0;
                _avgFloorTime = 0f;
            }
        }

        public int AdjustEnemyHP(int baseHP)
        {
            return Mathf.RoundToInt(baseHP * _currentMultiplier);
        }

        public int AdjustEnemyDamage(int baseDamage)
        {
            return Mathf.RoundToInt(baseDamage * _currentMultiplier);
        }

        public float AdjustDropRate(float baseRate)
        {
            // Inverse: harder difficulty = slightly better drops
            return baseRate * (2f - _currentMultiplier);
        }

        private void AdjustDifficulty(float delta)
        {
            _currentMultiplier = Mathf.Clamp(_currentMultiplier + delta, _minMultiplier, _maxMultiplier);
            Debug.Log($"[Difficulty] Adjusted to {_currentMultiplier:F2} ({DifficultyLabel})");
        }

        public void ResetSession()
        {
            _recentDeaths = 0;
            _recentFloors = 0;
            _avgFloorTime = 0f;
        }
    }
}

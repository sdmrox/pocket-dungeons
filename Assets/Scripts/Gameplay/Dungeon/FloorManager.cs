using System;
using UnityEngine;

namespace PocketDungeons.Gameplay.Dungeon
{
    /// <summary>
    /// Manages floor progression. Generates new dungeon when player reaches the exit.
    /// Tracks current floor, enemies remaining, and triggers floor completion.
    /// </summary>
    public class FloorManager : MonoBehaviour
    {
        public static FloorManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int _startingSeed = -1;
        [SerializeField] private int _floorsPerBoss = 10;

        private BSPDungeonGenerator _generator;
        private DungeonData _currentDungeon;
        private int _currentFloor = 1;
        private int _enemiesRemaining;
        private int _totalEnemiesKilled;
        private float _floorStartTime;
        private bool _floorComplete;

        public int CurrentFloor => _currentFloor;
        public int EnemiesRemaining => _enemiesRemaining;
        public int TotalEnemiesKilled => _totalEnemiesKilled;
        public DungeonData CurrentDungeon => _currentDungeon;
        public bool IsBossFloor => _currentFloor % _floorsPerBoss == 0;

        public event Action<int> OnFloorStarted;
        public event Action<int, float> OnFloorCompleted; // floor, time
        public event Action<int> OnEnemyCountChanged;

        private void Awake()
        {
            Instance = this;
            _generator = new BSPDungeonGenerator();
        }

        public DungeonData StartNewRun()
        {
            _currentFloor = 1;
            _totalEnemiesKilled = 0;
            return GenerateFloor();
        }

        public DungeonData AdvanceFloor()
        {
            _currentFloor++;
            return GenerateFloor();
        }

        private DungeonData GenerateFloor()
        {
            int seed = _startingSeed >= 0 ? _startingSeed + _currentFloor : Random.Range(0, int.MaxValue);
            _currentDungeon = _generator.Generate(seed, _currentFloor);
            _floorComplete = false;
            _floorStartTime = Time.time;
            OnFloorStarted?.Invoke(_currentFloor);
            return _currentDungeon;
        }

        public void RegisterEnemies(int count)
        {
            _enemiesRemaining = count;
            OnEnemyCountChanged?.Invoke(_enemiesRemaining);
        }

        public void OnEnemyKilled()
        {
            _enemiesRemaining--;
            _totalEnemiesKilled++;
            OnEnemyCountChanged?.Invoke(_enemiesRemaining);

            if (_enemiesRemaining <= 0 && !_floorComplete)
            {
                _floorComplete = true;
                float floorTime = Time.time - _floorStartTime;
                OnFloorCompleted?.Invoke(_currentFloor, floorTime);
            }
        }
    }
}

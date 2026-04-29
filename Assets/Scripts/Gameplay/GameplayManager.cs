using UnityEngine;
using PocketDungeons.Core.Services;
using PocketDungeons.Core.StateMachine;
using PocketDungeons.Gameplay.Dungeon;
using PocketDungeons.Gameplay.Player;
using PocketDungeons.Gameplay.Loot;
using PocketDungeons.UI.Views;

namespace PocketDungeons.Gameplay
{
    /// <summary>
    /// Orchestrates a dungeon run: dungeon generation, player spawn,
    /// enemy spawning, floor progression, run tracking, and results.
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _player;
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private DungeonRenderer _dungeonRenderer;
        [SerializeField] private FloorManager _floorManager;
        [SerializeField] private LootManager _lootManager;
        [SerializeField] private HUDView _hud;
        [SerializeField] private ResultsScreenView _resultsScreen;

        [Header("Camera")]
        [SerializeField] private Transform _cameraTarget;

        private RunData _currentRun;
        private float _runStartTime;

        private void Start()
        {
            InitializeRun();
        }

        private void InitializeRun()
        {
            _currentRun = new RunData();
            _runStartTime = Time.time;

            // Subscribe to events
            _playerHealth.OnHealthChanged += OnPlayerHealthChanged;
            _playerHealth.OnDeath += OnPlayerDeath;
            _floorManager.OnFloorStarted += OnFloorStarted;
            _floorManager.OnFloorCompleted += OnFloorCompleted;
            _floorManager.OnEnemyCountChanged += OnEnemyCountChanged;
            _lootManager.OnGoldChanged += OnGoldChanged;
            _lootManager.OnHealthPotionCollected += OnHealthPotionCollected;

            // Results screen callbacks
            if (_resultsScreen != null)
            {
                _resultsScreen.OnPlayAgain = () =>
                {
                    _resultsScreen.Hide();
                    InitializeRun();
                };
                _resultsScreen.OnMainMenu = () =>
                {
                    Services.Get<IGameStateService>()?.ChangeState(GameState.MainMenu);
                };
            }

            // Start floor 1
            _lootManager.Initialize(_player.transform);
            var dungeon = _floorManager.StartNewRun();
            _dungeonRenderer.Render(dungeon);
            SpawnPlayer(dungeon);
        }

        private void SpawnPlayer(DungeonData dungeon)
        {
            _player.transform.position = new Vector3(dungeon.Entrance.x, dungeon.Entrance.y, 0);
            _playerHealth.Initialize(_playerHealth.MaxHealth);
        }

        private void OnFloorStarted(int floor)
        {
            _hud?.UpdateFloor(floor);
        }

        private void OnFloorCompleted(int floor, float time)
        {
            _currentRun.FloorsCleared = floor;

            // Next floor
            var dungeon = _floorManager.AdvanceFloor();
            _dungeonRenderer.Render(dungeon);
            SpawnPlayer(dungeon);
        }

        private void OnPlayerHealthChanged(int current, int max)
        {
            _hud?.UpdateHealth(current, max);
        }

        private void OnGoldChanged(int gold)
        {
            _currentRun.GoldCollected = gold;
            _hud?.UpdateGold(gold);
        }

        private void OnHealthPotionCollected(int healAmount)
        {
            _playerHealth.Heal(healAmount);
            _currentRun.HealthPotionsUsed++;
        }

        private void OnEnemyCountChanged(int count)
        {
            _hud?.UpdateEnemyCount(count);
        }

        private void OnPlayerDeath()
        {
            _currentRun.TotalTimeSeconds = Time.time - _runStartTime;
            _currentRun.EnemiesKilled = _floorManager.TotalEnemiesKilled;
            _resultsScreen?.Show(_currentRun);

            Services.Get<IGameStateService>()?.ChangeState(GameState.Death);
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= OnPlayerHealthChanged;
                _playerHealth.OnDeath -= OnPlayerDeath;
            }

            if (_floorManager != null)
            {
                _floorManager.OnFloorStarted -= OnFloorStarted;
                _floorManager.OnFloorCompleted -= OnFloorCompleted;
                _floorManager.OnEnemyCountChanged -= OnEnemyCountChanged;
            }

            if (_lootManager != null)
            {
                _lootManager.OnGoldChanged -= OnGoldChanged;
                _lootManager.OnHealthPotionCollected -= OnHealthPotionCollected;
            }
        }
    }
}

using UnityEngine;
using PocketDungeons.Core.Services;
using PocketDungeons.Core.StateMachine;

namespace PocketDungeons.Core
{
    /// <summary>
    /// Entry point. Runs in Boot scene, initializes all services,
    /// then transitions to MainMenu.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Services (assign in Inspector)")]
        [SerializeField] private GameStateManager _gameStateManager;

        private void Awake()
        {
            // Ensure this persists across scene loads
            DontDestroyOnLoad(gameObject);

            // Set target frame rate
            Application.targetFrameRate = 60;

            // Register core services
            RegisterServices();
        }

        private void Start()
        {
            // Boot complete → go to Main Menu
            Debug.Log("[Boot] All services registered. Transitioning to MainMenu.");
            Services.Get<IGameStateService>().ChangeState(GameState.MainMenu);
        }

        private void RegisterServices()
        {
            // Game State
            Services.Register<IGameStateService>(_gameStateManager);

            // TODO: Register these when implementations are ready
            // Services.Register<IAudioService>(audioService);
            // Services.Register<IHapticsService>(hapticsService);
            // Services.Register<ISaveService>(saveService);
            // Services.Register<IAnalyticsService>(analyticsService);

            Debug.Log("[Boot] Services registered.");
        }

        private void OnApplicationQuit()
        {
            Services.Clear();
        }
    }
}

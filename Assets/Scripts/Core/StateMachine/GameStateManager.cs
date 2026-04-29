using System;
using UnityEngine;

namespace PocketDungeons.Core.StateMachine
{
    /// <summary>
    /// Manages game state transitions. Singleton accessed via Services.
    /// </summary>
    public class GameStateManager : MonoBehaviour, IGameStateService
    {
        public GameState CurrentState { get; private set; } = GameState.Boot;
        public GameState PreviousState { get; private set; } = GameState.Boot;

        public event Action<GameState, GameState> OnStateChanged;

        private static readonly bool[,] ValidTransitions = InitializeTransitions();

        public void ChangeState(GameState newState)
        {
            if (newState == CurrentState)
                return;

            if (!IsValidTransition(CurrentState, newState))
            {
                Debug.LogWarning($"[GameState] Invalid transition: {CurrentState} → {newState}");
                return;
            }

            PreviousState = CurrentState;
            CurrentState = newState;

            Debug.Log($"[GameState] {PreviousState} → {CurrentState}");
            OnStateChanged?.Invoke(PreviousState, CurrentState);
        }

        public bool IsValidTransition(GameState from, GameState to)
        {
            return ValidTransitions[(int)from, (int)to];
        }

        private static bool[,] InitializeTransitions()
        {
            int count = Enum.GetValues(typeof(GameState)).Length;
            var transitions = new bool[count, count];

            // Boot → MainMenu
            Allow(transitions, GameState.Boot, GameState.MainMenu);

            // MainMenu → Loading, Shop, Settings
            Allow(transitions, GameState.MainMenu, GameState.Loading);
            Allow(transitions, GameState.MainMenu, GameState.Shop);
            Allow(transitions, GameState.MainMenu, GameState.Settings);

            // Loading → Gameplay
            Allow(transitions, GameState.Loading, GameState.Gameplay);

            // Gameplay ↔ Paused, PowerUpSelect, BossIntro
            Allow(transitions, GameState.Gameplay, GameState.Paused);
            Allow(transitions, GameState.Paused, GameState.Gameplay);
            Allow(transitions, GameState.Gameplay, GameState.PowerUpSelect);
            Allow(transitions, GameState.PowerUpSelect, GameState.Gameplay);
            Allow(transitions, GameState.Gameplay, GameState.BossIntro);
            Allow(transitions, GameState.BossIntro, GameState.Gameplay);

            // Gameplay → Death → Results → MainMenu
            Allow(transitions, GameState.Gameplay, GameState.Death);
            Allow(transitions, GameState.Death, GameState.Results);
            Allow(transitions, GameState.Results, GameState.MainMenu);
            Allow(transitions, GameState.Results, GameState.Loading); // Quick restart

            // Paused → MainMenu (quit to menu)
            Allow(transitions, GameState.Paused, GameState.MainMenu);

            // Shop / Settings → MainMenu
            Allow(transitions, GameState.Shop, GameState.MainMenu);
            Allow(transitions, GameState.Settings, GameState.MainMenu);

            return transitions;
        }

        private static void Allow(bool[,] transitions, GameState from, GameState to)
        {
            transitions[(int)from, (int)to] = true;
        }
    }
}

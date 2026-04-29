using System;

namespace PocketDungeons.Core.StateMachine
{
    public interface IGameStateService
    {
        GameState CurrentState { get; }
        GameState PreviousState { get; }
        event Action<GameState, GameState> OnStateChanged;
        void ChangeState(GameState newState);
    }
}

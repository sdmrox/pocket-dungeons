using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Core.Events
{
    /// <summary>
    /// ScriptableObject-based event system for decoupled communication.
    /// Create event assets in Unity: Create > Pocket Dungeons > Events > Game Event
    /// </summary>
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "Pocket Dungeons/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _listeners = new();

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void Register(GameEventListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void Unregister(GameEventListener listener)
        {
            _listeners.Remove(listener);
        }
    }
}

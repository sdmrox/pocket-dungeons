using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Core.Events
{
    /// <summary>
    /// Generic typed ScriptableObject event. Subclass for specific payload types.
    /// Example: IntEvent : GameEvent&lt;int&gt; for damage events.
    /// </summary>
    public abstract class GameEvent<T> : ScriptableObject
    {
        private readonly List<IGameEventListener<T>> _listeners = new();

        public void Raise(T value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised(value);
            }
        }

        public void Register(IGameEventListener<T> listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void Unregister(IGameEventListener<T> listener)
        {
            _listeners.Remove(listener);
        }
    }

    public interface IGameEventListener<T>
    {
        void OnEventRaised(T value);
    }

    /// <summary>
    /// Integer event — use for damage, gold, score, etc.
    /// Create in Unity: Create > Pocket Dungeons > Events > Int Event
    /// </summary>
    [CreateAssetMenu(fileName = "NewIntEvent", menuName = "Pocket Dungeons/Events/Int Event")]
    public class IntEvent : GameEvent<int> { }

    /// <summary>
    /// Float event — use for health percentage, timers, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "NewFloatEvent", menuName = "Pocket Dungeons/Events/Float Event")]
    public class FloatEvent : GameEvent<float> { }
}

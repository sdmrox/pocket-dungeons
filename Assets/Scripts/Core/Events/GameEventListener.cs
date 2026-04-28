using UnityEngine;
using UnityEngine.Events;

namespace PocketDungeons.Core.Events
{
    /// <summary>
    /// Attach to any GameObject to listen for a ScriptableObject GameEvent.
    /// Configure the event reference and response in the Inspector.
    /// </summary>
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent _event;
        [SerializeField] private UnityEvent _response;

        private void OnEnable()
        {
            _event?.Register(this);
        }

        private void OnDisable()
        {
            _event?.Unregister(this);
        }

        public void OnEventRaised()
        {
            _response?.Invoke();
        }
    }
}

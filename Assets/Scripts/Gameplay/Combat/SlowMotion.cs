using System.Collections;
using UnityEngine;

namespace PocketDungeons.Gameplay.Combat
{
    /// <summary>
    /// Slow-motion effect for last-kill-in-room and dramatic moments.
    /// Duration: 0.3s, TimeScale: 0.3 per GDD juice settings.
    /// </summary>
    public class SlowMotion : MonoBehaviour
    {
        public static SlowMotion Instance { get; private set; }

        [SerializeField] private float _defaultDuration = 0.3f;
        [SerializeField] private float _defaultTimeScale = 0.3f;

        private Coroutine _activeRoutine;

        private void Awake()
        {
            Instance = this;
        }

        public void Trigger()
        {
            Trigger(_defaultDuration, _defaultTimeScale);
        }

        public void Trigger(float duration, float timeScale)
        {
            if (_activeRoutine != null)
                StopCoroutine(_activeRoutine);

            _activeRoutine = StartCoroutine(SlowMoRoutine(duration, timeScale));
        }

        private IEnumerator SlowMoRoutine(float duration, float timeScale)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * timeScale;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            _activeRoutine = null;
        }
    }
}

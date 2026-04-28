using System.Collections;
using UnityEngine;

namespace PocketDungeons.Gameplay.Combat
{
    /// <summary>
    /// Hit-stop (freeze frame) effect on damage. 2 frames normal, 4 frames for crits.
    /// At 60 FPS: 2 frames = ~33ms, 4 frames = ~66ms.
    /// </summary>
    public class HitStop : MonoBehaviour
    {
        public static HitStop Instance { get; private set; }

        private bool _isStopped;
        private Coroutine _currentRoutine;

        private void Awake()
        {
            Instance = this;
        }

        public void Stop(bool isCrit)
        {
            int frames = isCrit ? 4 : 2;
            float duration = frames / 60f;

            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);

            _currentRoutine = StartCoroutine(StopRoutine(duration));
        }

        private IEnumerator StopRoutine(float duration)
        {
            _isStopped = true;
            float originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = originalTimeScale;
            _isStopped = false;
            _currentRoutine = null;
        }

        public bool IsStopped => _isStopped;
    }
}

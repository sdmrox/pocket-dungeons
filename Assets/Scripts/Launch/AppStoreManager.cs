using UnityEngine;

namespace PocketDungeons.Launch
{
    /// <summary>
    /// App Store submission management. Handles review guidelines compliance,
    /// rating prompts, and deep links.
    /// </summary>
    public class AppStoreManager : MonoBehaviour
    {
        public static AppStoreManager Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private int _runsBeforeRatingPrompt = 10;
        [SerializeField] private int _minDaysSinceLastPrompt = 30;
        [SerializeField] private string _appStoreId = "";

        private int _runsSinceLastPrompt;
        private bool _hasRated;

        private void Awake()
        {
            Instance = this;
        }

        public void OnRunCompleted()
        {
            _runsSinceLastPrompt++;

            if (ShouldRequestReview())
            {
                RequestReview();
                _runsSinceLastPrompt = 0;
            }
        }

        private bool ShouldRequestReview()
        {
            if (_hasRated) return false;
            if (_runsSinceLastPrompt < _runsBeforeRatingPrompt) return false;

            // TODO: Check minDaysSinceLastPrompt using saved date
            return true;
        }

        private void RequestReview()
        {
            Debug.Log("[AppStore] Requesting review via SKStoreReviewController");
            // TODO: SKStoreReviewController.requestReview() via native plugin

            #if UNITY_IOS && !UNITY_EDITOR
            // UnityEngine.iOS.Device.RequestStoreReview();
            #endif
        }

        public void OpenAppStorePage()
        {
            if (!string.IsNullOrEmpty(_appStoreId))
            {
                Application.OpenURL($"https://apps.apple.com/app/id{_appStoreId}");
            }
        }

        public void HandleDeepLink(string url)
        {
            Debug.Log($"[AppStore] Deep link: {url}");

            // Parse deep link: pocketdungeons://event/halloween
            if (url.Contains("event/"))
            {
                string eventId = url.Substring(url.LastIndexOf("event/") + 6);
                LiveOps.SeasonalEventManager.Instance?.RefreshEvents();
            }
        }
    }
}

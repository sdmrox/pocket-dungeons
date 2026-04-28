using System;
using UnityEngine;

namespace PocketDungeons.Monetization.Ads
{
    /// <summary>
    /// Manages opt-in rewarded ads. No mandatory ads, no interstitials.
    /// Max 6 rewarded ads per day. Ethical F2P: ads never block gameplay.
    /// Placements: revive, double gold, bonus power-up choice.
    /// </summary>
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private int _maxAdsPerDay = 6;
        [SerializeField] private float _cooldownBetweenAds = 60f;

        private int _adsWatchedToday;
        private float _lastAdTime;
        private bool _adReady;
        private string _lastDateKey;

        public int AdsRemainingToday => _maxAdsPerDay - _adsWatchedToday;
        public bool CanShowAd => _adReady && AdsRemainingToday > 0 && Time.time - _lastAdTime > _cooldownBetweenAds;

        public event Action<string> OnAdRewardGranted; // placement
        public event Action<string> OnAdFailed; // placement

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ResetDailyCountIfNeeded();
            LoadRewardedAd();
        }

        public void ShowRewardedAd(string placement)
        {
            if (!CanShowAd)
            {
                OnAdFailed?.Invoke(placement);
                return;
            }

            Debug.Log($"[Ads] Showing rewarded ad for placement: {placement}");
            // TODO: Call ad SDK (AdMob/Unity Ads)
            // On success: OnAdWatched(placement);
        }

        public void OnAdWatched(string placement)
        {
            _adsWatchedToday++;
            _lastAdTime = Time.time;

            GrantReward(placement);
            OnAdRewardGranted?.Invoke(placement);

            Core.Services.Services.Get<Core.Services.IAnalyticsService>()?.LogEvent("ad_watched",
                new System.Collections.Generic.Dictionary<string, object>
                {
                    { "placement", placement },
                    { "ads_today", _adsWatchedToday }
                });

            LoadRewardedAd();
        }

        private void GrantReward(string placement)
        {
            switch (placement)
            {
                case "revive":
                    // Revive player at current floor
                    break;
                case "double_gold":
                    // Double end-of-run gold
                    break;
                case "bonus_powerup":
                    // Extra power-up selection
                    break;
            }
        }

        private void LoadRewardedAd()
        {
            // TODO: Pre-load next rewarded ad
            _adReady = true;
        }

        private void ResetDailyCountIfNeeded()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (_lastDateKey != today)
            {
                _adsWatchedToday = 0;
                _lastDateKey = today;
            }
        }
    }
}

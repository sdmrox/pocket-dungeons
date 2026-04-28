using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.PostLaunch
{
    /// <summary>
    /// Revenue optimization system. Monitors monetization KPIs and adjusts offers.
    /// Implements smart pricing, offer timing, and segment-based targeting.
    /// Year 1 target: $209K net revenue.
    /// </summary>
    public class RevenueOptimizer : MonoBehaviour
    {
        public static RevenueOptimizer Instance { get; private set; }

        [Serializable]
        public class PlayerSegment
        {
            public string SegmentId;
            public string Name;
            public float MinSpend;
            public float MaxSpend;
            public int MinRuns;
            public int MinDaysActive;
        }

        [Serializable]
        public class SpecialOffer
        {
            public string OfferId;
            public string ProductId;
            public string TriggerEvent;
            public int TriggerValue;
            public float DiscountPercent;
            public int DurationHours;
            public string[] TargetSegments;
        }

        [SerializeField] private PlayerSegment[] _segments;
        [SerializeField] private SpecialOffer[] _offers;

        private string _currentSegment = "free_player";
        private float _totalSpend;
        private int _totalRuns;
        private int _daysActive;

        public string CurrentSegment => _currentSegment;
        public event Action<SpecialOffer> OnOfferTriggered;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdatePlayerProfile(float totalSpend, int totalRuns, int daysActive)
        {
            _totalSpend = totalSpend;
            _totalRuns = totalRuns;
            _daysActive = daysActive;

            ClassifyPlayer();
        }

        public void CheckOfferTriggers(string eventName, int value)
        {
            foreach (var offer in _offers)
            {
                if (offer.TriggerEvent != eventName) continue;
                if (value < offer.TriggerValue) continue;

                bool segmentMatch = false;
                foreach (var seg in offer.TargetSegments)
                {
                    if (seg == _currentSegment || seg == "all")
                    {
                        segmentMatch = true;
                        break;
                    }
                }

                if (segmentMatch)
                    OnOfferTriggered?.Invoke(offer);
            }
        }

        private void ClassifyPlayer()
        {
            foreach (var seg in _segments)
            {
                if (_totalSpend >= seg.MinSpend && _totalSpend <= seg.MaxSpend &&
                    _totalRuns >= seg.MinRuns && _daysActive >= seg.MinDaysActive)
                {
                    _currentSegment = seg.SegmentId;
                    return;
                }
            }

            _currentSegment = "free_player";
        }

        public MonetizationReport GetReport()
        {
            return new MonetizationReport
            {
                TotalSpend = _totalSpend,
                PlayerSegment = _currentSegment,
                TotalRuns = _totalRuns,
                DaysActive = _daysActive,
                ARPU = _daysActive > 0 ? _totalSpend / _daysActive : 0f,
                RunsPerDay = _daysActive > 0 ? (float)_totalRuns / _daysActive : 0f
            };
        }
    }

    [Serializable]
    public struct MonetizationReport
    {
        public float TotalSpend;
        public string PlayerSegment;
        public int TotalRuns;
        public int DaysActive;
        public float ARPU;
        public float RunsPerDay;
    }
}

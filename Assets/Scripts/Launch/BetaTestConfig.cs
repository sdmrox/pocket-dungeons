using UnityEngine;

namespace PocketDungeons.Launch
{
    /// <summary>
    /// Beta testing configuration. Controls TestFlight phases.
    /// Phase 1: Internal (5 devs) — Weeks 23-24
    /// Phase 2: Closed Beta (50 testers) — Week 24
    /// Phase 3: Open Beta (500 testers) — Weeks 25-26
    /// </summary>
    [CreateAssetMenu(fileName = "BetaTestConfig", menuName = "Pocket Dungeons/Config/Beta Test")]
    public class BetaTestConfig : ScriptableObject
    {
        [Header("TestFlight Phases")]
        public BetaPhase CurrentPhase = BetaPhase.Internal;

        [Header("Feedback")]
        public bool EnableFeedbackButton = true;
        public bool EnableCrashReporting = true;
        public bool EnableVerboseLogging = true;
        public bool ShowDebugOverlay;

        [Header("Feature Flags")]
        public bool EnableBattlePass;
        public bool EnableIAP;
        public bool EnableAds;
        public bool EnableCloudSave;
        public bool EnableLeaderboards;

        [Header("Test Parameters")]
        public float SpeedMultiplier = 1f;
        public bool UnlockAllHeroes;
        public int StartingGold = 10000;
        public bool ShowAllFloors;
    }

    public enum BetaPhase
    {
        Internal,
        ClosedBeta,
        OpenBeta,
        Production
    }
}

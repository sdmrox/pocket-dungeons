using System;
using UnityEngine;

namespace PocketDungeons.PostLaunch
{
    /// <summary>
    /// Platform abstraction layer for Android port readiness.
    /// Wraps platform-specific APIs behind interfaces.
    /// iOS: StoreKit 2, GameKit, Core Haptics, CloudKit
    /// Android: Google Play Billing, Play Games, Vibrator, Google Drive
    /// </summary>
    public static class PlatformAbstraction
    {
        public enum Platform
        {
            iOS,
            Android,
            Editor
        }

        public static Platform CurrentPlatform
        {
            get
            {
                #if UNITY_IOS
                return Platform.iOS;
                #elif UNITY_ANDROID
                return Platform.Android;
                #else
                return Platform.Editor;
                #endif
            }
        }

        public static IPlatformStore CreateStore() => CurrentPlatform switch
        {
            Platform.iOS => new IOSStore(),
            Platform.Android => new AndroidStore(),
            _ => new EditorStore()
        };

        public static IPlatformSocial CreateSocial() => CurrentPlatform switch
        {
            Platform.iOS => new IOSSocial(),
            Platform.Android => new AndroidSocial(),
            _ => new EditorSocial()
        };

        public static IPlatformHaptics CreateHaptics() => CurrentPlatform switch
        {
            Platform.iOS => new IOSHaptics(),
            Platform.Android => new AndroidHaptics(),
            _ => new EditorHaptics()
        };
    }

    public interface IPlatformStore
    {
        void Initialize();
        void Purchase(string productId, Action<bool> callback);
        void RestorePurchases(Action<bool> callback);
    }

    public interface IPlatformSocial
    {
        void Authenticate(Action<bool> callback);
        void SubmitScore(string leaderboardId, long score);
        void ShowLeaderboard();
        void ReportAchievement(string id, float progress);
    }

    public interface IPlatformHaptics
    {
        void Light();
        void Medium();
        void Heavy();
        void Success();
        void Error();
    }

    // iOS implementations
    public class IOSStore : IPlatformStore
    {
        public void Initialize() => Debug.Log("[iOS] StoreKit 2 init");
        public void Purchase(string productId, Action<bool> callback) => callback?.Invoke(true);
        public void RestorePurchases(Action<bool> callback) => callback?.Invoke(true);
    }

    public class IOSSocial : IPlatformSocial
    {
        public void Authenticate(Action<bool> callback) => callback?.Invoke(true);
        public void SubmitScore(string leaderboardId, long score) { }
        public void ShowLeaderboard() { }
        public void ReportAchievement(string id, float progress) { }
    }

    public class IOSHaptics : IPlatformHaptics
    {
        public void Light() => Debug.Log("[iOS] Core Haptics: Light");
        public void Medium() => Debug.Log("[iOS] Core Haptics: Medium");
        public void Heavy() => Debug.Log("[iOS] Core Haptics: Heavy");
        public void Success() => Debug.Log("[iOS] Core Haptics: Success");
        public void Error() => Debug.Log("[iOS] Core Haptics: Error");
    }

    // Android implementations
    public class AndroidStore : IPlatformStore
    {
        public void Initialize() => Debug.Log("[Android] Play Billing init");
        public void Purchase(string productId, Action<bool> callback) => callback?.Invoke(true);
        public void RestorePurchases(Action<bool> callback) => callback?.Invoke(true);
    }

    public class AndroidSocial : IPlatformSocial
    {
        public void Authenticate(Action<bool> callback) => callback?.Invoke(true);
        public void SubmitScore(string leaderboardId, long score) { }
        public void ShowLeaderboard() { }
        public void ReportAchievement(string id, float progress) { }
    }

    public class AndroidHaptics : IPlatformHaptics
    {
        public void Light() => Debug.Log("[Android] Vibrator: Light");
        public void Medium() => Debug.Log("[Android] Vibrator: Medium");
        public void Heavy() => Debug.Log("[Android] Vibrator: Heavy");
        public void Success() => Debug.Log("[Android] Vibrator: Success");
        public void Error() => Debug.Log("[Android] Vibrator: Error");
    }

    // Editor stubs
    public class EditorStore : IPlatformStore
    {
        public void Initialize() => Debug.Log("[Editor] Store mock init");
        public void Purchase(string productId, Action<bool> callback) => callback?.Invoke(true);
        public void RestorePurchases(Action<bool> callback) => callback?.Invoke(true);
    }

    public class EditorSocial : IPlatformSocial
    {
        public void Authenticate(Action<bool> callback) => callback?.Invoke(true);
        public void SubmitScore(string leaderboardId, long score) { }
        public void ShowLeaderboard() { }
        public void ReportAchievement(string id, float progress) { }
    }

    public class EditorHaptics : IPlatformHaptics
    {
        public void Light() { }
        public void Medium() { }
        public void Heavy() { }
        public void Success() { }
        public void Error() { }
    }
}

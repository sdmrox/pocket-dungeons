namespace PocketDungeons.Core.Services
{
    public enum HapticType
    {
        Light,      // Loot pickup, menu tap
        Medium,     // Hit enemy, dodge
        Heavy,      // Crit, boss hit
        Success,    // Level up, achievement
        Warning,    // Low health
        Error       // Death
    }

    public interface IHapticsService
    {
        void Play(HapticType type);
        void SetEnabled(bool enabled);
        bool IsEnabled { get; }
    }
}

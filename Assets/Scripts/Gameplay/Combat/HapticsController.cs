using UnityEngine;
using PocketDungeons.Core.Services;

namespace PocketDungeons.Gameplay.Combat
{
    /// <summary>
    /// Maps game events to haptic feedback patterns.
    /// Intensity table per GDD:
    ///   Light: loot pickup, menu tap
    ///   Medium: hit enemy, dodge
    ///   Heavy: crit, boss hit, death
    ///   Success: level up, achievement
    /// </summary>
    public class HapticsController : MonoBehaviour
    {
        public static HapticsController Instance { get; private set; }

        private IHapticsService _hapticsService;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Services.TryGet(out _hapticsService);
        }

        public void OnEnemyHit(bool isCrit)
        {
            _hapticsService?.Play(isCrit ? HapticType.Heavy : HapticType.Medium);
        }

        public void OnPlayerDodge()
        {
            _hapticsService?.Play(HapticType.Medium);
        }

        public void OnLootPickup()
        {
            _hapticsService?.Play(HapticType.Light);
        }

        public void OnPlayerHit()
        {
            _hapticsService?.Play(HapticType.Heavy);
        }

        public void OnPlayerDeath()
        {
            _hapticsService?.Play(HapticType.Error);
        }

        public void OnLevelUp()
        {
            _hapticsService?.Play(HapticType.Success);
        }

        public void OnBossHit()
        {
            _hapticsService?.Play(HapticType.Heavy);
        }

        public void OnMenuTap()
        {
            _hapticsService?.Play(HapticType.Light);
        }

        public void OnLowHealth()
        {
            _hapticsService?.Play(HapticType.Warning);
        }
    }
}

using System;
using UnityEngine;

namespace PocketDungeons.Gameplay.Audio
{
    public enum MusicLayer
    {
        Ambient,    // Always playing, quiet
        Exploration, // Active when exploring
        Combat,     // Fades in during combat
        BossFight,  // Overrides during boss
        Victory,    // Brief stinger on floor clear
        Death       // Death jingle
    }

    public enum CombatIntensity
    {
        None,       // No enemies nearby
        Low,        // 1-2 enemies
        Medium,     // 3-5 enemies
        High,       // 6+ enemies or boss
        Boss        // Boss fight
    }

    [Serializable]
    public class MusicLayerConfig
    {
        public MusicLayer Layer;
        public AudioClip Clip;
        [Range(0f, 1f)] public float MaxVolume = 1f;
        public float FadeInDuration = 1f;
        public float FadeOutDuration = 0.5f;
    }

    public class AdaptiveMusicSystem : MonoBehaviour
    {
        public static AdaptiveMusicSystem Instance { get; private set; }

        [SerializeField] private MusicLayerConfig[] _layers;
        [SerializeField] private AudioSource[] _layerSources;

        [Header("Intensity Thresholds")]
        [SerializeField] private int _lowEnemyThreshold = 1;
        [SerializeField] private int _mediumEnemyThreshold = 3;
        [SerializeField] private int _highEnemyThreshold = 6;

        private CombatIntensity _currentIntensity = CombatIntensity.None;
        private readonly float[] _layerTargetVolumes = new float[6];
        private readonly float[] _layerCurrentVolumes = new float[6];

        public CombatIntensity CurrentIntensity => _currentIntensity;
        public event Action<CombatIntensity> OnIntensityChanged;

        private void Awake()
        {
            Instance = this;
        }

        public void SetEnemyCount(int activeEnemies, bool isBossActive)
        {
            var newIntensity = CalculateIntensity(activeEnemies, isBossActive);
            if (newIntensity == _currentIntensity) return;

            _currentIntensity = newIntensity;
            UpdateLayerTargets();
            OnIntensityChanged?.Invoke(_currentIntensity);
        }

        public CombatIntensity CalculateIntensity(int enemies, bool isBoss)
        {
            if (isBoss) return CombatIntensity.Boss;
            if (enemies >= _highEnemyThreshold) return CombatIntensity.High;
            if (enemies >= _mediumEnemyThreshold) return CombatIntensity.Medium;
            if (enemies >= _lowEnemyThreshold) return CombatIntensity.Low;
            return CombatIntensity.None;
        }

        public void UpdateLayerTargets()
        {
            for (int i = 0; i < _layerTargetVolumes.Length; i++)
                _layerTargetVolumes[i] = 0f;

            switch (_currentIntensity)
            {
                case CombatIntensity.None:
                    _layerTargetVolumes[(int)MusicLayer.Ambient] = 1f;
                    _layerTargetVolumes[(int)MusicLayer.Exploration] = 0.6f;
                    break;
                case CombatIntensity.Low:
                    _layerTargetVolumes[(int)MusicLayer.Ambient] = 0.5f;
                    _layerTargetVolumes[(int)MusicLayer.Combat] = 0.4f;
                    break;
                case CombatIntensity.Medium:
                    _layerTargetVolumes[(int)MusicLayer.Ambient] = 0.3f;
                    _layerTargetVolumes[(int)MusicLayer.Combat] = 0.7f;
                    break;
                case CombatIntensity.High:
                    _layerTargetVolumes[(int)MusicLayer.Combat] = 1f;
                    break;
                case CombatIntensity.Boss:
                    _layerTargetVolumes[(int)MusicLayer.BossFight] = 1f;
                    break;
            }
        }

        public void UpdateFade(float deltaTime)
        {
            if (_layers == null) return;

            for (int i = 0; i < _layers.Length && i < _layerSources.Length; i++)
            {
                float target = _layerTargetVolumes[i] * _layers[i].MaxVolume;
                float speed = _layerCurrentVolumes[i] < target
                    ? 1f / _layers[i].FadeInDuration
                    : 1f / _layers[i].FadeOutDuration;

                _layerCurrentVolumes[i] = Mathf.MoveTowards(
                    _layerCurrentVolumes[i], target, speed * deltaTime);

                if (_layerSources[i] != null)
                    _layerSources[i].volume = _layerCurrentVolumes[i];
            }
        }

        public void PlayStinger(MusicLayer stinger)
        {
            if (stinger == MusicLayer.Victory || stinger == MusicLayer.Death)
            {
                _layerTargetVolumes[(int)stinger] = 1f;
            }
        }

        public float GetLayerVolume(int index)
        {
            return index >= 0 && index < _layerCurrentVolumes.Length
                ? _layerCurrentVolumes[index] : 0f;
        }

        public float GetLayerTarget(int index)
        {
            return index >= 0 && index < _layerTargetVolumes.Length
                ? _layerTargetVolumes[index] : 0f;
        }

        public void Reset()
        {
            _currentIntensity = CombatIntensity.None;
            for (int i = 0; i < _layerTargetVolumes.Length; i++)
            {
                _layerTargetVolumes[i] = 0f;
                _layerCurrentVolumes[i] = 0f;
            }
        }
    }
}

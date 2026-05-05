using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Gameplay.Audio
{
    public enum SFXType
    {
        PlayerAttack,
        PlayerDodge,
        PlayerHit,
        PlayerDeath,
        EnemyHit,
        EnemyDeath,
        BossRoar,
        BossPhaseChange,
        LootPickup,
        GoldPickup,
        PotionPickup,
        LevelUp,
        PowerUpSelect,
        ButtonClick,
        MenuOpen,
        MenuClose,
        DoorOpen,
        ChestOpen,
        TrapTrigger,
        AbilityFire,
        CriticalHit,
        ShieldBlock,
        Explosion,
        HealEffect
    }

    [Serializable]
    public class SFXEntry
    {
        public SFXType Type;
        public AudioClip[] Clips;
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(0.8f, 1.2f)] public float PitchMin = 0.95f;
        [Range(0.8f, 1.2f)] public float PitchMax = 1.05f;
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private SFXEntry[] _sfxEntries;

        [Header("Settings")]
        [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 0.7f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;

        private readonly Dictionary<SFXType, SFXEntry> _sfxMap = new();
        private bool _isMuted;

        public float MasterVolume { get => _masterVolume; set => SetMasterVolume(value); }
        public float MusicVolume { get => _musicVolume; set => SetMusicVolume(value); }
        public float SFXVolume { get => _sfxVolume; set => SetSFXVolume(value); }
        public bool IsMuted => _isMuted;

        public event Action<float> OnMasterVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;

        private void Awake()
        {
            Instance = this;

            if (_sfxEntries != null)
            {
                foreach (var entry in _sfxEntries)
                    _sfxMap[entry.Type] = entry;
            }
        }

        public void PlaySFX(SFXType type)
        {
            if (_isMuted || _sfxSource == null) return;
            if (!_sfxMap.TryGetValue(type, out var entry)) return;
            if (entry.Clips == null || entry.Clips.Length == 0) return;

            var clip = entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];
            if (clip == null) return;

            _sfxSource.pitch = UnityEngine.Random.Range(entry.PitchMin, entry.PitchMax);
            _sfxSource.PlayOneShot(clip, entry.Volume * _sfxVolume * _masterVolume);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource == null || clip == null) return;

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = _musicVolume * _masterVolume;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource?.Stop();
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
            OnMasterVolumeChanged?.Invoke(_masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
            OnMusicVolumeChanged?.Invoke(_musicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }

        public void ToggleMute()
        {
            _isMuted = !_isMuted;
            if (_musicSource != null)
                _musicSource.mute = _isMuted;
        }

        public void SetMuted(bool muted)
        {
            _isMuted = muted;
            if (_musicSource != null)
                _musicSource.mute = _isMuted;
        }

        public bool HasSFX(SFXType type) => _sfxMap.ContainsKey(type);
        public int GetSFXCount() => _sfxMap.Count;

        private void UpdateMusicVolume()
        {
            if (_musicSource != null)
                _musicSource.volume = _musicVolume * _masterVolume;
        }
    }
}

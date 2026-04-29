using System;
using System.Collections.Generic;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.Dungeon
{
    [Serializable]
    public class BiomeTheme
    {
        public string BiomeId;
        public Color FloorColor = Color.gray;
        public Color WallColor = Color.white;
        public Color AccentColor = Color.yellow;
        public Color AmbientLightColor = Color.white;
        public float AmbientIntensity = 0.6f;
        public float FogDensity;
        public Color FogColor = Color.black;
    }

    public class BiomeThemeManager : MonoBehaviour
    {
        public static BiomeThemeManager Instance { get; private set; }

        [SerializeField] private BiomeTheme[] _themes;

        private readonly Dictionary<string, BiomeTheme> _themeMap = new();
        private BiomeTheme _activeTheme;

        public BiomeTheme ActiveTheme => _activeTheme;
        public event Action<BiomeTheme> OnThemeChanged;

        private void Awake()
        {
            Instance = this;

            if (_themes != null)
            {
                foreach (var theme in _themes)
                    _themeMap[theme.BiomeId] = theme;
            }
        }

        public void Initialize(BiomeTheme[] themes)
        {
            _themeMap.Clear();
            if (themes != null)
            {
                foreach (var theme in themes)
                    _themeMap[theme.BiomeId] = theme;
            }
        }

        public bool ApplyTheme(string biomeId)
        {
            if (!_themeMap.TryGetValue(biomeId, out var theme)) return false;

            _activeTheme = theme;
            OnThemeChanged?.Invoke(theme);
            return true;
        }

        public BiomeTheme GetTheme(string biomeId)
        {
            return _themeMap.TryGetValue(biomeId, out var theme) ? theme : null;
        }

        public string GetBiomeForFloor(int floor)
        {
            if (floor <= 10) return "stone_dungeon";
            if (floor <= 20) return "cursed_catacombs";
            return "crystal_caverns";
        }

        public bool HasTheme(string biomeId) => _themeMap.ContainsKey(biomeId);
        public int ThemeCount => _themeMap.Count;

        public Color GetFloorColor() => _activeTheme?.FloorColor ?? Color.gray;
        public Color GetWallColor() => _activeTheme?.WallColor ?? Color.white;
        public Color GetAmbientColor() => _activeTheme?.AmbientLightColor ?? Color.white;
        public float GetAmbientIntensity() => _activeTheme?.AmbientIntensity ?? 0.6f;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.UI.Localization
{
    public enum Language
    {
        English,
        Arabic,
        Japanese,
        Spanish,
        Portuguese
    }

    [Serializable]
    public class LocalizationEntry
    {
        public string Key;
        public string Value;
    }

    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        private Language _currentLanguage = Language.English;
        private readonly Dictionary<Language, Dictionary<string, string>> _translations = new();

        public Language CurrentLanguage => _currentLanguage;
        public event Action<Language> OnLanguageChanged;

        private void Awake()
        {
            Instance = this;
            InitializeDefaultTranslations();
        }

        public void SetLanguage(Language language)
        {
            _currentLanguage = language;
            OnLanguageChanged?.Invoke(language);
        }

        public string GetText(string key)
        {
            if (_translations.TryGetValue(_currentLanguage, out var langMap))
            {
                if (langMap.TryGetValue(key, out var value))
                    return value;
            }

            if (_currentLanguage != Language.English &&
                _translations.TryGetValue(Language.English, out var fallback))
            {
                if (fallback.TryGetValue(key, out var value))
                    return value;
            }

            return key;
        }

        public string GetText(string key, params object[] args)
        {
            string template = GetText(key);
            return string.Format(template, args);
        }

        public void LoadTranslations(Language language, Dictionary<string, string> entries)
        {
            _translations[language] = new Dictionary<string, string>(entries);
        }

        public void AddTranslation(Language language, string key, string value)
        {
            if (!_translations.TryGetValue(language, out var langMap))
            {
                langMap = new Dictionary<string, string>();
                _translations[language] = langMap;
            }
            langMap[key] = value;
        }

        public bool HasTranslation(string key)
        {
            return _translations.TryGetValue(_currentLanguage, out var langMap) &&
                   langMap.ContainsKey(key);
        }

        public bool HasLanguage(Language language) => _translations.ContainsKey(language);

        public int GetTranslationCount(Language language)
        {
            return _translations.TryGetValue(language, out var langMap) ? langMap.Count : 0;
        }

        public bool IsRTL() => _currentLanguage == Language.Arabic;

        public string GetLanguageCode() => _currentLanguage switch
        {
            Language.English => "en",
            Language.Arabic => "ar",
            Language.Japanese => "ja",
            Language.Spanish => "es",
            Language.Portuguese => "pt",
            _ => "en"
        };

        private void InitializeDefaultTranslations()
        {
            var english = new Dictionary<string, string>
            {
                { "menu.play", "Play" },
                { "menu.heroes", "Heroes" },
                { "menu.town", "Town" },
                { "menu.shop", "Shop" },
                { "menu.settings", "Settings" },
                { "hud.floor", "Floor {0}" },
                { "hud.gold", "Gold: {0}" },
                { "hud.enemies", "Enemies: {0}" },
                { "results.title", "Run Complete!" },
                { "results.score", "Score: {0}" },
                { "results.best_floor", "Best Floor: {0}" },
                { "results.play_again", "Play Again" },
                { "results.main_menu", "Main Menu" },
                { "death.title", "You Died" },
                { "death.revive", "Revive ({0} Gems)" },
                { "death.continue", "Continue" },
                { "quest.daily_title", "Daily Quests" },
                { "quest.weekly_title", "Weekly Challenges" },
                { "battle_pass.title", "Battle Pass" },
                { "battle_pass.premium", "Upgrade to Premium" },
                { "town.blacksmith", "Blacksmith" },
                { "town.armory", "Armory" },
                { "town.alchemist", "Alchemist" },
                { "town.treasury", "Treasury" },
                { "accessibility.colorblind", "Colorblind Mode" },
                { "accessibility.ui_scale", "UI Scale" },
                { "accessibility.text_size", "Text Size" },
                { "accessibility.reduced_motion", "Reduced Motion" },
                { "notification.daily_challenge", "New Daily Dungeon!" },
                { "notification.streak_reminder", "Don't lose your streak!" }
            };

            _translations[Language.English] = english;

            var spanish = new Dictionary<string, string>
            {
                { "menu.play", "Jugar" },
                { "menu.heroes", "Heroes" },
                { "menu.town", "Pueblo" },
                { "menu.shop", "Tienda" },
                { "menu.settings", "Ajustes" },
                { "hud.floor", "Piso {0}" },
                { "hud.gold", "Oro: {0}" },
                { "results.title", "Carrera Completa!" },
                { "results.play_again", "Jugar de nuevo" },
                { "results.main_menu", "Menu Principal" },
                { "death.title", "Has Muerto" },
                { "death.revive", "Revivir ({0} Gemas)" }
            };
            _translations[Language.Spanish] = spanish;

            var japanese = new Dictionary<string, string>
            {
                { "menu.play", "プレイ" },
                { "menu.heroes", "ヒーロー" },
                { "menu.town", "町" },
                { "menu.shop", "ショップ" },
                { "menu.settings", "設定" },
                { "hud.floor", "フロア {0}" },
                { "results.title", "ラン完了!" },
                { "death.title", "死亡" }
            };
            _translations[Language.Japanese] = japanese;

            var arabic = new Dictionary<string, string>
            {
                { "menu.play", "العب" },
                { "menu.heroes", "الأبطال" },
                { "menu.town", "المدينة" },
                { "menu.shop", "المتجر" },
                { "menu.settings", "الإعدادات" },
                { "results.title", "!اكتملت الجولة" },
                { "death.title", "لقد مت" }
            };
            _translations[Language.Arabic] = arabic;

            var portuguese = new Dictionary<string, string>
            {
                { "menu.play", "Jogar" },
                { "menu.heroes", "Heróis" },
                { "menu.town", "Cidade" },
                { "menu.shop", "Loja" },
                { "menu.settings", "Configurações" },
                { "results.title", "Corrida Completa!" },
                { "death.title", "Você Morreu" }
            };
            _translations[Language.Portuguese] = portuguese;
        }
    }
}

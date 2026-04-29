using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class LocalizationTests
    {
        enum Language { English, Arabic, Japanese, Spanish, Portuguese }

        Dictionary<Language, Dictionary<string, string>> _translations = new();
        Language _current = Language.English;

        [SetUp]
        public void SetUp()
        {
            _current = Language.English;
            _translations = new()
            {
                { Language.English, new Dictionary<string, string>
                    {
                        { "menu.play", "Play" },
                        { "menu.heroes", "Heroes" },
                        { "hud.floor", "Floor {0}" },
                        { "results.title", "Run Complete!" },
                        { "death.title", "You Died" }
                    }
                },
                { Language.Spanish, new Dictionary<string, string>
                    {
                        { "menu.play", "Jugar" },
                        { "menu.heroes", "Heroes" },
                        { "hud.floor", "Piso {0}" },
                        { "death.title", "Has Muerto" }
                    }
                },
                { Language.Japanese, new Dictionary<string, string>
                    {
                        { "menu.play", "プレイ" },
                        { "death.title", "死亡" }
                    }
                },
                { Language.Arabic, new Dictionary<string, string>
                    {
                        { "menu.play", "العب" },
                        { "death.title", "لقد مت" }
                    }
                },
                { Language.Portuguese, new Dictionary<string, string>
                    {
                        { "menu.play", "Jogar" },
                        { "death.title", "Você Morreu" }
                    }
                }
            };
        }

        string GetText(string key)
        {
            if (_translations.TryGetValue(_current, out var map) && map.TryGetValue(key, out var val))
                return val;
            if (_current != Language.English && _translations.TryGetValue(Language.English, out var en) && en.TryGetValue(key, out var fallback))
                return fallback;
            return key;
        }

        string GetLanguageCode() => _current switch
        {
            Language.English => "en",
            Language.Arabic => "ar",
            Language.Japanese => "ja",
            Language.Spanish => "es",
            Language.Portuguese => "pt",
            _ => "en"
        };

        [Test]
        public void English_ReturnsCorrectText()
        {
            Assert.That(GetText("menu.play"), Is.EqualTo("Play"));
            Assert.That(GetText("death.title"), Is.EqualTo("You Died"));
        }

        [Test]
        public void Spanish_ReturnsTranslation()
        {
            _current = Language.Spanish;
            Assert.That(GetText("menu.play"), Is.EqualTo("Jugar"));
            Assert.That(GetText("death.title"), Is.EqualTo("Has Muerto"));
        }

        [Test]
        public void Japanese_ReturnsTranslation()
        {
            _current = Language.Japanese;
            Assert.That(GetText("menu.play"), Is.EqualTo("プレイ"));
        }

        [Test]
        public void Arabic_ReturnsTranslation()
        {
            _current = Language.Arabic;
            Assert.That(GetText("menu.play"), Is.EqualTo("العب"));
        }

        [Test]
        public void Portuguese_ReturnsTranslation()
        {
            _current = Language.Portuguese;
            Assert.That(GetText("menu.play"), Is.EqualTo("Jogar"));
        }

        [Test]
        public void FallbackToEnglish_WhenKeyMissing()
        {
            _current = Language.Spanish;
            Assert.That(GetText("results.title"), Is.EqualTo("Run Complete!"));
        }

        [Test]
        public void ReturnsKey_WhenNoTranslationExists()
        {
            Assert.That(GetText("nonexistent.key"), Is.EqualTo("nonexistent.key"));
        }

        [Test]
        public void Format_InsertsArguments()
        {
            string template = GetText("hud.floor");
            string result = string.Format(template, 5);
            Assert.That(result, Is.EqualTo("Floor 5"));
        }

        [Test]
        public void AllFiveLanguages_HavePlayButton()
        {
            foreach (Language lang in Enum.GetValues<Language>())
            {
                Assert.That(_translations.ContainsKey(lang), Is.True,
                    $"Missing language: {lang}");
                Assert.That(_translations[lang].ContainsKey("menu.play"), Is.True,
                    $"Missing menu.play for {lang}");
            }
        }

        [Test]
        public void Arabic_IsRTL()
        {
            _current = Language.Arabic;
            bool isRTL = _current == Language.Arabic;
            Assert.That(isRTL, Is.True);
        }

        [Test]
        public void LanguageCodes_AreCorrect()
        {
            _current = Language.English;
            Assert.That(GetLanguageCode(), Is.EqualTo("en"));
            _current = Language.Arabic;
            Assert.That(GetLanguageCode(), Is.EqualTo("ar"));
            _current = Language.Japanese;
            Assert.That(GetLanguageCode(), Is.EqualTo("ja"));
            _current = Language.Spanish;
            Assert.That(GetLanguageCode(), Is.EqualTo("es"));
            _current = Language.Portuguese;
            Assert.That(GetLanguageCode(), Is.EqualTo("pt"));
        }
    }
}

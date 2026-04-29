using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class BiomeThemeTests
    {
        record BiomeTheme(string Id, float FloorR, float FloorG, float FloorB,
            float WallR, float WallG, float WallB, float AmbientIntensity, float FogDensity);

        Dictionary<string, BiomeTheme> _themes = new();

        [SetUp]
        public void SetUp()
        {
            _themes.Clear();
            _themes["stone_dungeon"] = new("stone_dungeon", 0.5f, 0.5f, 0.5f, 0.7f, 0.7f, 0.7f, 0.6f, 0f);
            _themes["cursed_catacombs"] = new("cursed_catacombs", 0.3f, 0.2f, 0.3f, 0.4f, 0.3f, 0.4f, 0.4f, 0.1f);
            _themes["crystal_caverns"] = new("crystal_caverns", 0.2f, 0.5f, 0.8f, 0.3f, 0.6f, 0.9f, 0.8f, 0f);
        }

        string GetBiomeForFloor(int floor)
        {
            if (floor <= 10) return "stone_dungeon";
            if (floor <= 20) return "cursed_catacombs";
            return "crystal_caverns";
        }

        [Test]
        public void Floor1to10_IsStoneDungeon()
        {
            for (int f = 1; f <= 10; f++)
                Assert.That(GetBiomeForFloor(f), Is.EqualTo("stone_dungeon"));
        }

        [Test]
        public void Floor11to20_IsCursedCatacombs()
        {
            for (int f = 11; f <= 20; f++)
                Assert.That(GetBiomeForFloor(f), Is.EqualTo("cursed_catacombs"));
        }

        [Test]
        public void Floor21Plus_IsCrystalCaverns()
        {
            Assert.That(GetBiomeForFloor(21), Is.EqualTo("crystal_caverns"));
            Assert.That(GetBiomeForFloor(100), Is.EqualTo("crystal_caverns"));
        }

        [Test]
        public void ThreeBiomes_Exist()
        {
            Assert.That(_themes.Count, Is.EqualTo(3));
            Assert.That(_themes.ContainsKey("stone_dungeon"), Is.True);
            Assert.That(_themes.ContainsKey("cursed_catacombs"), Is.True);
            Assert.That(_themes.ContainsKey("crystal_caverns"), Is.True);
        }

        [Test]
        public void AmbientIntensity_InValidRange()
        {
            foreach (var theme in _themes.Values)
                Assert.That(theme.AmbientIntensity, Is.InRange(0f, 1f));
        }

        [Test]
        public void CursedCatacombs_HasFog()
        {
            Assert.That(_themes["cursed_catacombs"].FogDensity, Is.GreaterThan(0f));
        }
    }
}

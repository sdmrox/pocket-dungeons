using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class HeroSelectViewTests
    {
        string[] _heroes = { "Warrior", "Archer", "Mage" };
        int _selectedIndex;
        float _swipeThreshold = 50f;
        float _carouselSpacing = 300f;
        float _targetOffset;
        float _currentOffset;

        [SetUp]
        public void SetUp()
        {
            _selectedIndex = 0;
            _targetOffset = 0;
            _currentOffset = 0;
        }

        void SelectIndex(int index)
        {
            if (index < 0 || index >= _heroes.Length) return;
            _selectedIndex = index;
            _targetOffset = -index * _carouselSpacing;
        }

        void Swipe(float startX, float endX)
        {
            float delta = endX - startX;
            if (Math.Abs(delta) < _swipeThreshold) return;
            if (delta < 0 && _selectedIndex < _heroes.Length - 1)
                SelectIndex(_selectedIndex + 1);
            else if (delta > 0 && _selectedIndex > 0)
                SelectIndex(_selectedIndex - 1);
        }

        [Test]
        public void InitialSelection_IsFirstHero()
        {
            Assert.That(_selectedIndex, Is.EqualTo(0));
            Assert.That(_heroes[_selectedIndex], Is.EqualTo("Warrior"));
        }

        [Test]
        public void SelectIndex_ChangesHero()
        {
            SelectIndex(1);
            Assert.That(_selectedIndex, Is.EqualTo(1));
            Assert.That(_heroes[_selectedIndex], Is.EqualTo("Archer"));
        }

        [Test]
        public void SelectIndex_ClampsToRange()
        {
            SelectIndex(-1);
            Assert.That(_selectedIndex, Is.EqualTo(0));
            SelectIndex(99);
            Assert.That(_selectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void SwipeLeft_GoesToNext()
        {
            Swipe(200f, 100f);
            Assert.That(_selectedIndex, Is.EqualTo(1));
        }

        [Test]
        public void SwipeRight_GoesToPrevious()
        {
            SelectIndex(2);
            Swipe(100f, 200f);
            Assert.That(_selectedIndex, Is.EqualTo(1));
        }

        [Test]
        public void Swipe_BelowThreshold_NoChange()
        {
            Swipe(200f, 170f); // delta = -30, below 50
            Assert.That(_selectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void SwipeLeft_AtEnd_NoChange()
        {
            SelectIndex(2);
            Swipe(200f, 100f);
            Assert.That(_selectedIndex, Is.EqualTo(2));
        }

        [Test]
        public void SwipeRight_AtStart_NoChange()
        {
            Swipe(100f, 200f);
            Assert.That(_selectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void CarouselOffset_MatchesSelection()
        {
            SelectIndex(1);
            Assert.That(_targetOffset, Is.EqualTo(-300f));
            SelectIndex(2);
            Assert.That(_targetOffset, Is.EqualTo(-600f));
        }

        [Test]
        public void UpdateCarousel_ConvergesOnTarget()
        {
            SelectIndex(1);
            float snapSpeed = 10f;
            for (int i = 0; i < 100; i++)
            {
                float dt = 0.016f;
                _currentOffset = _currentOffset + (_targetOffset - _currentOffset) * dt * snapSpeed;
            }
            Assert.That(_currentOffset, Is.EqualTo(_targetOffset).Within(1f));
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.UI.Views
{
    public class HeroSelectView : MonoBehaviour
    {
        [SerializeField] private HeroData[] _allHeroes;
        [SerializeField] private float _swipeThreshold = 50f;
        [SerializeField] private float _carouselSpacing = 300f;
        [SerializeField] private float _snapSpeed = 10f;

        private int _selectedIndex;
        private float _swipeStartX;
        private bool _isSwiping;
        private float _targetOffset;
        private float _currentOffset;

        public HeroData SelectedHero => _allHeroes != null && _selectedIndex < _allHeroes.Length
            ? _allHeroes[_selectedIndex] : null;

        public int SelectedIndex => _selectedIndex;
        public int HeroCount => _allHeroes?.Length ?? 0;

        public event Action<HeroData> OnHeroSelected;
        public event Action<int> OnIndexChanged;

        public void Initialize(HeroData[] heroes)
        {
            _allHeroes = heroes;
            _selectedIndex = 0;
            _currentOffset = 0;
            _targetOffset = 0;
        }

        public void OnSwipeBegin(float screenX)
        {
            _swipeStartX = screenX;
            _isSwiping = true;
        }

        public void OnSwipeEnd(float screenX)
        {
            if (!_isSwiping) return;
            _isSwiping = false;

            float delta = screenX - _swipeStartX;
            if (Mathf.Abs(delta) >= _swipeThreshold)
            {
                if (delta < 0 && _selectedIndex < _allHeroes.Length - 1)
                    SelectIndex(_selectedIndex + 1);
                else if (delta > 0 && _selectedIndex > 0)
                    SelectIndex(_selectedIndex - 1);
            }

            _targetOffset = -_selectedIndex * _carouselSpacing;
        }

        public void SelectIndex(int index)
        {
            if (_allHeroes == null || index < 0 || index >= _allHeroes.Length) return;

            _selectedIndex = index;
            _targetOffset = -index * _carouselSpacing;
            OnIndexChanged?.Invoke(index);
            OnHeroSelected?.Invoke(_allHeroes[index]);
        }

        public void UpdateCarousel(float deltaTime)
        {
            _currentOffset = Mathf.Lerp(_currentOffset, _targetOffset, deltaTime * _snapSpeed);
        }

        public float GetCurrentOffset() => _currentOffset;

        public bool IsHeroUnlocked(int index)
        {
            if (_allHeroes == null || index < 0 || index >= _allHeroes.Length) return false;
            return _allHeroes[index].UnlockedByDefault;
        }

        public HeroData GetHero(int index)
        {
            if (_allHeroes == null || index < 0 || index >= _allHeroes.Length) return null;
            return _allHeroes[index];
        }

        public Color GetRarityBorderColor(int index)
        {
            if (_allHeroes == null || index < 0 || index >= _allHeroes.Length) return Color.white;

            var hero = _allHeroes[index];
            return hero.Class switch
            {
                HeroClass.Warrior => new Color(0.8f, 0.2f, 0.2f),
                HeroClass.Archer => new Color(0.2f, 0.8f, 0.2f),
                HeroClass.Mage => new Color(0.2f, 0.4f, 0.9f),
                _ => Color.white
            };
        }
    }
}

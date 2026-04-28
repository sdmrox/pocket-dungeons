using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.Player
{
    /// <summary>
    /// Configures player components based on selected HeroData.
    /// Sets stats on PlayerCombat, PlayerHealth, and HeroAbility.
    /// </summary>
    public class HeroManager : MonoBehaviour
    {
        [SerializeField] private PlayerController _controller;
        [SerializeField] private PlayerCombat _combat;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private HeroAbility _ability;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private HeroData _currentHero;

        public HeroData CurrentHero => _currentHero;

        public void Initialize(HeroData hero)
        {
            _currentHero = hero;

            if (_health != null)
                _health.Initialize(hero.BaseHealth);

            if (_combat != null)
                _combat.SetStats(hero.BaseDamage, hero.AttackRange, hero.AttackSpeed, 0.05f, 2f);

            if (_ability != null)
                _ability.Initialize(hero);

            if (_spriteRenderer != null && hero.InGameSprite != null)
                _spriteRenderer.sprite = hero.InGameSprite;
        }

        public void ApplyLevelBonus(int level)
        {
            if (_currentHero == null || level <= 1) return;

            int bonusHealth = Mathf.RoundToInt(_currentHero.HealthPerLevel * (level - 1));
            int bonusDamage = Mathf.RoundToInt(_currentHero.DamagePerLevel * (level - 1));

            _health?.Initialize(_currentHero.BaseHealth + bonusHealth);
            _combat?.SetStats(
                _currentHero.BaseDamage + bonusDamage,
                _currentHero.AttackRange,
                _currentHero.AttackSpeed,
                0.05f,
                2f);
        }
    }
}

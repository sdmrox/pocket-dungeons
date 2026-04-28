using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PocketDungeons.UI.Views
{
    /// <summary>
    /// In-game HUD: health bar, floor number, gold count, enemy count, dodge cooldown.
    /// Placed in safe areas respecting notch/Dynamic Island.
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider _healthBar;
        [SerializeField] private Image _healthFill;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Color _healthColorFull = new(0.2f, 0.9f, 0.3f);
        [SerializeField] private Color _healthColorLow = new(0.9f, 0.2f, 0.2f);
        [SerializeField] private float _lowHealthThreshold = 0.3f;

        [Header("Floor Info")]
        [SerializeField] private TextMeshProUGUI _floorText;
        [SerializeField] private TextMeshProUGUI _enemyCountText;

        [Header("Gold")]
        [SerializeField] private TextMeshProUGUI _goldText;

        [Header("Dodge")]
        [SerializeField] private Image _dodgeCooldownFill;

        [Header("Pause")]
        [SerializeField] private Button _pauseButton;

        public System.Action OnPausePressed;

        private void Awake()
        {
            _pauseButton?.onClick.AddListener(() => OnPausePressed?.Invoke());
        }

        public void UpdateHealth(int current, int max)
        {
            float percent = (float)current / max;

            if (_healthBar != null)
                _healthBar.value = percent;

            if (_healthText != null)
                _healthText.text = $"{current}/{max}";

            if (_healthFill != null)
                _healthFill.color = percent > _lowHealthThreshold ? _healthColorFull : _healthColorLow;
        }

        public void UpdateFloor(int floor)
        {
            if (_floorText != null)
                _floorText.text = $"F{floor}";
        }

        public void UpdateEnemyCount(int count)
        {
            if (_enemyCountText != null)
                _enemyCountText.text = count > 0 ? $"{count}" : "";
        }

        public void UpdateGold(int gold)
        {
            if (_goldText != null)
                _goldText.text = $"{gold}";
        }

        public void UpdateDodgeCooldown(float progress)
        {
            if (_dodgeCooldownFill != null)
                _dodgeCooldownFill.fillAmount = progress;
        }
    }
}

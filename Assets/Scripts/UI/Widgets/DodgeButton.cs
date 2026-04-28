using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PocketDungeons.UI.Widgets
{
    /// <summary>
    /// Dodge button in bottom-right thumb zone. Shows cooldown fill overlay.
    /// Supports swipe direction for aimed dodge.
    /// </summary>
    public class DodgeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("References")]
        [SerializeField] private Image _cooldownOverlay;
        [SerializeField] private Image _buttonIcon;
        [SerializeField] private Color _readyColor = Color.white;
        [SerializeField] private Color _cooldownColor = new(0.5f, 0.5f, 0.5f, 0.7f);

        private Vector2 _pressPosition;
        private bool _pressed;

        public System.Action<Vector2> OnDodgePressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressPosition = eventData.position;
            _pressed = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Track drag for swipe direction
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_pressed) return;
            _pressed = false;

            Vector2 swipeDir = eventData.position - _pressPosition;
            if (swipeDir.magnitude < 10f)
                swipeDir = Vector2.zero; // Tap = dodge in movement direction

            OnDodgePressed?.Invoke(swipeDir.normalized);
        }

        public void UpdateCooldown(float progress)
        {
            if (_cooldownOverlay != null)
                _cooldownOverlay.fillAmount = 1f - progress;

            if (_buttonIcon != null)
                _buttonIcon.color = progress >= 1f ? _readyColor : _cooldownColor;
        }
    }
}

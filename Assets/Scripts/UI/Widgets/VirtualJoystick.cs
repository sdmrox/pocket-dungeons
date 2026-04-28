using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PocketDungeons.UI.Widgets
{
    /// <summary>
    /// Floating virtual joystick for the bottom-left thumb zone.
    /// Appears on touch, follows finger within radius, returns to center on release.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private Canvas _canvas;

        [Header("Settings")]
        [SerializeField] private float _handleRange = 50f;
        [SerializeField] private float _deadZone = 0.1f;
        [SerializeField] private bool _floating = true;

        private Vector2 _input;
        private Vector2 _center;
        private Camera _cam;

        public Vector2 Input => _input;

        private void Start()
        {
            _cam = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;
            _center = _background.anchoredPosition;

            if (_floating)
            {
                _background.gameObject.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_floating)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    eventData.position,
                    _cam,
                    out Vector2 localPoint);

                _background.anchoredPosition = localPoint;
                _background.gameObject.SetActive(true);
            }

            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background,
                eventData.position,
                _cam,
                out Vector2 localPoint);

            Vector2 normalized = localPoint / (_background.sizeDelta * 0.5f);
            _input = normalized.magnitude > 1f ? normalized.normalized : normalized;

            if (_input.magnitude < _deadZone)
                _input = Vector2.zero;

            _handle.anchoredPosition = _input * _handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _input = Vector2.zero;
            _handle.anchoredPosition = Vector2.zero;

            if (_floating)
            {
                _background.gameObject.SetActive(false);
                _background.anchoredPosition = _center;
            }
        }
    }
}

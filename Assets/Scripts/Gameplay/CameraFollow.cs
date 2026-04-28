using UnityEngine;

namespace PocketDungeons.Gameplay
{
    /// <summary>
    /// Smooth camera follow with dungeon bounds clamping.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothSpeed = 8f;
        [SerializeField] private Vector3 _offset = new(0, 0, -10);

        [Header("Bounds")]
        [SerializeField] private bool _useBounds;
        [SerializeField] private float _minX, _maxX, _minY, _maxY;

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;

            if (_useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, _minX, _maxX);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, _minY, _maxY);
            }

            transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        }

        public void SetBounds(float minX, float maxX, float minY, float maxY)
        {
            _useBounds = true;
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SnapToTarget()
        {
            if (_target == null) return;
            transform.position = _target.position + _offset;
        }
    }
}

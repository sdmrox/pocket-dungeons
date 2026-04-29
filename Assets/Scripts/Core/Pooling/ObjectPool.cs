using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Core.Pooling
{
    /// <summary>
    /// Generic object pool for frequently spawned/despawned objects.
    /// Use for: enemies, projectiles, VFX, damage numbers, loot drops.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialSize = 10;
        [SerializeField] private int _maxSize = 50;
        [SerializeField] private bool _expandable = true;

        private readonly Queue<GameObject> _available = new();
        private readonly HashSet<GameObject> _inUse = new();
        private Transform _container;

        public int ActiveCount => _inUse.Count;
        public int AvailableCount => _available.Count;
        public int TotalCount => ActiveCount + AvailableCount;

        private void Awake()
        {
            _container = new GameObject($"Pool_{_prefab.name}").transform;
            _container.SetParent(transform);
            Prewarm(_initialSize);
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count && TotalCount < _maxSize; i++)
            {
                var obj = CreateInstance();
                obj.SetActive(false);
                _available.Enqueue(obj);
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj;

            if (_available.Count > 0)
            {
                obj = _available.Dequeue();
            }
            else if (_expandable && TotalCount < _maxSize)
            {
                obj = CreateInstance();
            }
            else
            {
                Debug.LogWarning($"[Pool] {_prefab.name} exhausted (max: {_maxSize})");
                return null;
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            _inUse.Add(obj);
            return obj;
        }

        public void Return(GameObject obj)
        {
            if (!_inUse.Remove(obj))
            {
                Debug.LogWarning($"[Pool] Tried to return object not from this pool: {obj.name}");
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(_container);
            _available.Enqueue(obj);
        }

        public void ReturnAll()
        {
            var active = new List<GameObject>(_inUse);
            foreach (var obj in active)
            {
                Return(obj);
            }
        }

        private GameObject CreateInstance()
        {
            var obj = Instantiate(_prefab, _container);
            obj.name = $"{_prefab.name}_{TotalCount}";
            return obj;
        }
    }
}

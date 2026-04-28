using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Core.Pooling
{
    /// <summary>
    /// Central pool manager. Register pools by tag for global access.
    /// Usage: PoolManager.Instance.Get("Enemy_Slime", position, rotation)
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [System.Serializable]
        public struct PoolConfig
        {
            public string Tag;
            public ObjectPool Pool;
        }

        [SerializeField] private PoolConfig[] _pools;

        private readonly Dictionary<string, ObjectPool> _poolMap = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            foreach (var config in _pools)
            {
                if (!_poolMap.ContainsKey(config.Tag))
                {
                    _poolMap.Add(config.Tag, config.Pool);
                }
            }
        }

        public GameObject Get(string tag, Vector3 position, Quaternion rotation)
        {
            if (_poolMap.TryGetValue(tag, out var pool))
            {
                return pool.Get(position, rotation);
            }

            Debug.LogError($"[PoolManager] No pool registered with tag: {tag}");
            return null;
        }

        public void Return(string tag, GameObject obj)
        {
            if (_poolMap.TryGetValue(tag, out var pool))
            {
                pool.Return(obj);
            }
            else
            {
                Debug.LogError($"[PoolManager] No pool registered with tag: {tag}");
            }
        }

        public void ReturnAll(string tag)
        {
            if (_poolMap.TryGetValue(tag, out var pool))
            {
                pool.ReturnAll();
            }
        }

        public void ReturnAll()
        {
            foreach (var pool in _poolMap.Values)
            {
                pool.ReturnAll();
            }
        }
    }
}

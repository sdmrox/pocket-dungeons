using UnityEngine;

namespace PocketDungeons.Gameplay.Combat
{
    /// <summary>
    /// Spawns particle VFX at positions. Central access point for all combat VFX.
    /// Uses object pool when available, falls back to Instantiate+Destroy.
    /// </summary>
    public class VFXSpawner : MonoBehaviour
    {
        public static VFXSpawner Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private GameObject _hitVFX;
        [SerializeField] private GameObject _critHitVFX;
        [SerializeField] private GameObject _deathVFX;
        [SerializeField] private GameObject _healVFX;
        [SerializeField] private GameObject _dodgeTrailVFX;
        [SerializeField] private GameObject _levelUpVFX;
        [SerializeField] private GameObject _lootPickupVFX;
        [SerializeField] private GameObject _explosionVFX;
        [SerializeField] private GameObject _shieldBlockVFX;

        [Header("Lifetime")]
        [SerializeField] private float _defaultLifetime = 1f;

        private void Awake()
        {
            Instance = this;
        }

        public void SpawnHit(Vector3 position, bool isCrit)
        {
            Spawn(isCrit ? _critHitVFX : _hitVFX, position);
        }

        public void SpawnDeath(Vector3 position)
        {
            Spawn(_deathVFX, position);
        }

        public void SpawnHeal(Vector3 position)
        {
            Spawn(_healVFX, position);
        }

        public void SpawnDodgeTrail(Vector3 position)
        {
            Spawn(_dodgeTrailVFX, position);
        }

        public void SpawnLevelUp(Vector3 position)
        {
            Spawn(_levelUpVFX, position);
        }

        public void SpawnLootPickup(Vector3 position)
        {
            Spawn(_lootPickupVFX, position);
        }

        public void SpawnExplosion(Vector3 position)
        {
            Spawn(_explosionVFX, position);
        }

        public void SpawnShieldBlock(Vector3 position)
        {
            Spawn(_shieldBlockVFX, position);
        }

        private void Spawn(GameObject prefab, Vector3 position)
        {
            if (prefab == null) return;
            var go = Instantiate(prefab, position, Quaternion.identity);
            Destroy(go, _defaultLifetime);
        }
    }
}

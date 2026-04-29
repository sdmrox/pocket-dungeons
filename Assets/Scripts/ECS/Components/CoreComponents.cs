using Unity.Entities;
using Unity.Mathematics;

namespace PocketDungeons.ECS.Components
{
    // --- Transform / Movement ---
    public struct PositionComponent : IComponentData
    {
        public float2 Value;
    }

    public struct VelocityComponent : IComponentData
    {
        public float2 Value;
    }

    public struct SpeedComponent : IComponentData
    {
        public float Value;
    }

    // --- Health / Combat ---
    public struct HealthComponent : IComponentData
    {
        public int Current;
        public int Max;
    }

    public struct DamageComponent : IComponentData
    {
        public int Value;
        public DamageType Type;
        public Entity Source;
    }

    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison
    }

    // --- Tags ---
    public struct PlayerTag : IComponentData { }
    public struct EnemyTag : IComponentData { }
    public struct ProjectileTag : IComponentData { }
    public struct LootTag : IComponentData { }
    public struct DeadTag : IComponentData { }

    // --- Loot ---
    public struct LootDropComponent : IComponentData
    {
        public int LootTableId;
    }

    // --- Lifetime ---
    public struct LifetimeComponent : IComponentData
    {
        public float RemainingSeconds;
    }

    // --- Invincibility (dodge i-frames) ---
    public struct InvincibleComponent : IComponentData
    {
        public float RemainingSeconds;
    }
}

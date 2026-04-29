using Unity.Burst;
using Unity.Entities;
using PocketDungeons.ECS.Components;

namespace PocketDungeons.ECS.Systems
{
    /// <summary>
    /// Destroys entities when their lifetime expires (projectiles, VFX, etc).
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct LifetimeSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (lifetime, entity) in
                SystemAPI.Query<RefRW<LifetimeComponent>>()
                    .WithEntityAccess())
            {
                lifetime.ValueRW.RemainingSeconds -= dt;

                if (lifetime.ValueRO.RemainingSeconds <= 0f)
                {
                    ecb.DestroyEntity(entity);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}

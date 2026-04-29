using Unity.Burst;
using Unity.Entities;
using PocketDungeons.ECS.Components;

namespace PocketDungeons.ECS.Systems
{
    /// <summary>
    /// Processes damage events: applies damage to health, marks dead entities.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MovementSystem))]
    public partial struct DamageSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (health, damage, entity) in
                SystemAPI.Query<RefRW<HealthComponent>, RefRO<DamageComponent>>()
                    .WithNone<DeadTag>()
                    .WithEntityAccess())
            {
                health.ValueRW.Current -= damage.ValueRO.Value;

                // Remove the damage component after processing
                ecb.RemoveComponent<DamageComponent>(entity);

                if (health.ValueRO.Current <= 0)
                {
                    health.ValueRW.Current = 0;
                    ecb.AddComponent<DeadTag>(entity);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}

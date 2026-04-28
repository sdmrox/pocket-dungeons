using Unity.Burst;
using Unity.Entities;
using PocketDungeons.ECS.Components;

namespace PocketDungeons.ECS.Systems
{
    /// <summary>
    /// Moves all entities with Position + Velocity each frame.
    /// Burst-compiled for maximum performance.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct MovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (pos, vel) in SystemAPI.Query<RefRW<PositionComponent>, RefRO<VelocityComponent>>())
            {
                pos.ValueRW.Value += vel.ValueRO.Value * dt;
            }
        }
    }
}

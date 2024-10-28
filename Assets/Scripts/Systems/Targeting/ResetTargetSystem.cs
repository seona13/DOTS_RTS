using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            if (target.ValueRW.targetEntity != Entity.Null)
            {
                if (SystemAPI.Exists(target.ValueRO.targetEntity) == false || SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity) == false)
                {
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }
        }
        foreach (RefRW<TargetOverride> targetOverride in SystemAPI.Query<RefRW<TargetOverride>>())
        {
            if (targetOverride.ValueRW.targetEntity != Entity.Null)
            {
                if (SystemAPI.Exists(targetOverride.ValueRO.targetEntity) == false || SystemAPI.HasComponent<LocalTransform>(targetOverride.ValueRO.targetEntity) == false)
                {
                    targetOverride.ValueRW.targetEntity = Entity.Null;
                }
            }
        }
    }
}

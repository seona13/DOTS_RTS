using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct LoseTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<Target> target,
            RefRO<LoseTarget> loseTarget,
            RefRO<TargetOverride> targetOverride)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<Target>,
                RefRO<LoseTarget>,
                RefRO<TargetOverride>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue; // We don't already have a target; nothing to do here.
            }

            if (targetOverride.ValueRO.targetEntity != Entity.Null)
            {
                continue; // We have a target override; don't lose it.
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float targetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);
            if (targetDistance > loseTarget.ValueRO.loseTargetDistance)
            {
                // Target is too far, reset it
                target.ValueRW.targetEntity = Entity.Null;
            }
        }
    }
}
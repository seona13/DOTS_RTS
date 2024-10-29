using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using static UnityEngine.GraphicsBuffer;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    ComponentLookup<LocalTransform> localTransformComponentLookup;
    EntityStorageInfoLookup entityStorageInfoLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformComponentLookup = state.GetComponentLookup<LocalTransform>(true);
        entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        localTransformComponentLookup.Update(ref state);
        entityStorageInfoLookup.Update(ref state);

        ResetTargetJob resetTargetJob = new ResetTargetJob
        {
            localTransformComponentLookup = localTransformComponentLookup,
            entityStorageInfoLookup = entityStorageInfoLookup,
        };
        resetTargetJob.ScheduleParallel();

        ResetTargetOverrideJob resetTargetOverrideJob = new ResetTargetOverrideJob
        {
            localTransformComponentLookup = localTransformComponentLookup,
            entityStorageInfoLookup = entityStorageInfoLookup,
        };
        resetTargetOverrideJob.ScheduleParallel();
    }
}


[BurstCompile]
public partial struct ResetTargetJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

    public void Execute(ref Target target)
    {
        if (target.targetEntity != Entity.Null)
        {
            if (entityStorageInfoLookup.Exists(target.targetEntity) == false || localTransformComponentLookup.HasComponent(target.targetEntity) == false)
            {
                target.targetEntity = Entity.Null;
            }
        }
    }
}

[BurstCompile]
public partial struct ResetTargetOverrideJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

    public void Execute(ref TargetOverride targetOverride)
    {
        if (targetOverride.targetEntity != Entity.Null)
        {
            if (entityStorageInfoLookup.Exists(targetOverride.targetEntity) == false || localTransformComponentLookup.HasComponent(targetOverride.targetEntity) == false)
            {
                targetOverride.targetEntity = Entity.Null;
            }
        }
    }
}
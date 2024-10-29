using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    ComponentLookup<LocalTransform> localTransformComponentLookup;
    ComponentLookup<Health> healthComponentLookup;
    ComponentLookup<PostTransformMatrix> postTransformMatrixComponentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformComponentLookup = state.GetComponentLookup<LocalTransform>();
        healthComponentLookup = state.GetComponentLookup<Health>(true);
        postTransformMatrixComponentLookup = state.GetComponentLookup<PostTransformMatrix>(false);
    }


    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;
        if (Camera.main != null) {
            cameraForward = Camera.main.transform.forward; 
        }

        localTransformComponentLookup.Update(ref state);
        healthComponentLookup.Update(ref state);
        postTransformMatrixComponentLookup.Update(ref state);

        HealthBarJob healthBarJob = new HealthBarJob
        {
            cameraForward = cameraForward,
            localTransformComponentLookup = localTransformComponentLookup,
            healthComponentLookup = healthComponentLookup,
            postTransformMatrixComponentLookup = postTransformMatrixComponentLookup
        };
        healthBarJob.ScheduleParallel();
    }
}


[BurstCompile]
public partial struct HealthBarJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [ReadOnly] public ComponentLookup<Health> healthComponentLookup;
    [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> postTransformMatrixComponentLookup;

    public float3 cameraForward;

    public void Execute(in HealthBar healthBar, Entity entity)
    {
        RefRW<LocalTransform> localTransform = localTransformComponentLookup.GetRefRW(entity);
        if (localTransform.ValueRO.Scale == 1f)
        {
            // Health bar is visible, so make it face the camera
            LocalTransform parentLocalTransform = localTransformComponentLookup[healthBar.healthEntity];
            localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
        }

        Health health = healthComponentLookup[healthBar.healthEntity];

        if (health.onHealthChanged == false)
        {
            return; // Health has not changed, so don't update bar
        }

        float healthNormalised = (float)health.healthAmount / health.healthAmountMax;

        if (healthNormalised == 1f)
        {
            localTransform.ValueRW.Scale = 0f;
        }
        else
        {
            localTransform.ValueRW.Scale = 1f;
        }

        RefRW<PostTransformMatrix> barVisualPostTransformMatrix = postTransformMatrixComponentLookup.GetRefRW(healthBar.barVisualEntity);
        barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalised, 1, 1);
    }
}
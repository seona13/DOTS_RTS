using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;
        if (Camera.main != null) {
            cameraForward = Camera.main.transform.forward; 
        }

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<HealthBar> healthBar) 
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<HealthBar>>())
        {
            if (localTransform.ValueRO.Scale == 1f)
            {
                // Health bar is visible, so make it face the camera
                LocalTransform parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);
                localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }

            Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

            if (health.onHealthChanged == false)
            {
                continue; // Health has not changed, so don't update bar
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

            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalised, 1, 1);
        }
    }
}

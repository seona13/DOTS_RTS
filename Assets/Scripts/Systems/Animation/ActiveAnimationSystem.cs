using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

partial struct ActiveAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<ActiveAnimation> activeAnimation,
            RefRW<MaterialMeshInfo> materialMeshInfo)
            in SystemAPI.Query<
                RefRW<ActiveAnimation>,
                RefRW<MaterialMeshInfo>>())
        {
            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
            if (activeAnimation.ValueRW.frameTimer > activeAnimation.ValueRW.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= activeAnimation.ValueRW.frameTimerMax;
                activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % activeAnimation.ValueRW.frameMax;

                switch (activeAnimation.ValueRW.frame)
                {
                    default:
                    case 0:
                        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame0;
                        break;
                    case 1:
                        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame1;
                        break;
                }
            }
        }
    }
}

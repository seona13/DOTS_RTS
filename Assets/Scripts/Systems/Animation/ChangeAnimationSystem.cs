using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

[UpdateBefore(typeof(ActiveAnimationSystem))]
partial struct ChangeAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolder>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();

        ChangeAnimationJob changeAnimationJob = new ChangeAnimationJob
        {
            animationDataBlobArrayBlobAssetReference = animationDataHolder.animationDataBlobArrayBlobAssetReference,
        };
        changeAnimationJob.ScheduleParallel();
    }
}


[BurstCompile]
public partial struct ChangeAnimationJob : IJobEntity
{
    public BlobAssetReference<BlobArray<AnimationData>> animationDataBlobArrayBlobAssetReference;

    public void Execute(ref ActiveAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
    {
        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
        {
            return; // Don't interrupt the shoot animation
        }
        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.zombieAttack)
        {
            return; // Don't interrupt the zombie attack animation
        }

        if (activeAnimation.activeAnimationType != activeAnimation.nextAnimationType)
        {
            activeAnimation.frame = 0;
            activeAnimation.frameTimer = 0f;
            activeAnimation.activeAnimationType = activeAnimation.nextAnimationType;

            ref AnimationData animationData = ref animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.activeAnimationType];

            materialMeshInfo.Mesh = animationData.intMeshIdBlobArray[0];
        }
    }
}
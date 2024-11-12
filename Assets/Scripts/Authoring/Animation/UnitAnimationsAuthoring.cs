using Unity.Entities;
using UnityEngine;

public struct UnitAnimations : IComponentData
{
    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;
}


public class UnitAnimationsAuthoring : MonoBehaviour
{
    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;

    public class Baker : Baker<UnitAnimationsAuthoring>
    {
        public override void Bake(UnitAnimationsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitAnimations
            {
                idleAnimationType = authoring.idleAnimationType,
                walkAnimationType = authoring.walkAnimationType,
            });
        }
    }
}

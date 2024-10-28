using Unity.Entities;
using UnityEngine;

public struct TargetOverride : IComponentData
{
    public Entity targetEntity;
}


public class TargetOverrideAuthoring : MonoBehaviour
{
    public class Baker : Baker<TargetOverrideAuthoring>
    {
        public override void Bake(TargetOverrideAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TargetOverride());
        }
    }
}

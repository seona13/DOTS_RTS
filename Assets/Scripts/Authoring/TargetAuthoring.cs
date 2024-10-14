using Unity.Entities;
using UnityEngine;


public struct Target : IComponentData
{
    public Entity targetEntity;
}


public class TargetAuthoring : MonoBehaviour
{
    public class Baker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Target());
        }
    }
}

using Unity.Entities;
using UnityEngine;

public struct Zombie : IComponentData
{

}


public class ZombieAuthoring : MonoBehaviour
{
    public class Baker : Baker<ZombieAuthoring>
    {
        public override void Bake(ZombieAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Zombie());
        }
    }
}

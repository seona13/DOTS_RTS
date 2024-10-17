using Unity.Entities;
using UnityEngine;

public struct EntitiesReferences :IComponentData
{
    public Entity bulletPrefabEntity;
    public Entity zombiePrefabEntity;
}


public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject bulletPrefabGameObject;
    public GameObject zombiePrefabGameObject;

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                zombiePrefabEntity = GetEntity(authoring.zombiePrefabGameObject, TransformUsageFlags.Dynamic),

            });
        }
    }
}

using Unity.Entities;
using UnityEngine;

public struct EntitiesReferences :IComponentData
{
    public Entity bulletPrefabEntity;
    public Entity zombiePrefabEntity;
    public Entity shootLightPrefabEntity;
    public Entity scoutPrefabEntity;
    public Entity soldierPrefabEntity;
}


public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject bulletPrefabGameObject;
    public GameObject zombiePrefabGameObject;
    public GameObject shootLightPrefabGameObject;
    public GameObject scoutPrefabGameObject;
    public GameObject soldierPrefabGameObject;

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                zombiePrefabEntity = GetEntity(authoring.zombiePrefabGameObject, TransformUsageFlags.Dynamic),
                shootLightPrefabEntity = GetEntity(authoring.shootLightPrefabGameObject, TransformUsageFlags.Dynamic),
                scoutPrefabEntity = GetEntity(authoring.scoutPrefabGameObject, TransformUsageFlags.Dynamic),
                soldierPrefabEntity = GetEntity(authoring.soldierPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }
}

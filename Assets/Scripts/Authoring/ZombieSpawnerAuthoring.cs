using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct ZombieSpawner : IComponentData
{
    public float timer;
    public float timerMax;
}


public class ZombieSpawnerAuthoring : MonoBehaviour
{
    public float timerMax;

    public class Baker : Baker<ZombieSpawnerAuthoring>
    {
        public override void Bake(ZombieSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawner
            {
                timerMax = authoring.timerMax,
            });
        }
    }
}

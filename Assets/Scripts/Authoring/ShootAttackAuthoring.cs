using Unity.Entities;
using UnityEngine;

public struct ShootAttack : IComponentData
{
    public float timer;
    public float timerMax;
}


public class ShootAttackAuthoring : MonoBehaviour
{
    public float timerMax;

    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack
            {
                timerMax = authoring.timerMax,
            });
        }
    }
}

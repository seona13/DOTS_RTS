using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue; // We don't currently have a target, so stop.
            }

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0f)
            {
                continue; // Timer not elapsed, so stop.
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }
}

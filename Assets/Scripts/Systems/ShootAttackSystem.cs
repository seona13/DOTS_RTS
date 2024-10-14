using Unity.Burst;
using Unity.Entities;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target)
            in SystemAPI.Query<
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

            RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
            int damageAmount = 1;
            targetHealth.ValueRW.healthAmount -= damageAmount;
        }
    }
}

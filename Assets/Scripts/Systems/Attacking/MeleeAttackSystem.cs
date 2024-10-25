using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<RaycastHit> raycastHitList = new NativeList<RaycastHit>(Allocator.Temp);

        foreach ((
            RefRO<LocalTransform> localTranform,
            RefRW<MeleeAttack> meleeAttack,
            RefRO<Target> target,
            RefRW<UnitMover> unitMover)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<MeleeAttack>,
                RefRO<Target>,
                RefRW<UnitMover>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue; // We don't have a target, so do nothing.
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float meleeAttackDistanceSq = 2f;
            bool isCloseEnoughToAttack = math.distancesq(localTranform.ValueRO.Position, targetLocalTransform.Position) < meleeAttackDistanceSq;
            
            bool isTouchingTarget = false;
            if (isCloseEnoughToAttack == false)
            {
                float3 dirToTarget = targetLocalTransform.Position - localTranform.ValueRO.Position;
                dirToTarget = math.normalize(dirToTarget);
                float distanceBuffer = 0.4f;

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = localTranform.ValueRO.Position,
                    End = localTranform.ValueRO.Position + dirToTarget * (meleeAttack.ValueRO.colliderSize + distanceBuffer),
                    Filter = CollisionFilter.Default
                };
                raycastHitList.Clear();
                if (collisionWorld.CastRay(raycastInput, ref raycastHitList))
                {
                    foreach (RaycastHit raycastHit in raycastHitList)
                    {
                        if (raycastHit.Entity == target.ValueRO.targetEntity)
                        {
                            // Raycast hit target, close enough to attack
                            isTouchingTarget = true;
                            break;
                        }
                    }
                }
            }
            
            if (isCloseEnoughToAttack == false && isTouchingTarget == false)
            {
                // Target is too far, so move closer
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
            }
            else
            {
                // Target is close enough, so stop moving and attack
                unitMover.ValueRW.targetPosition = localTranform.ValueRO.Position;

                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (meleeAttack.ValueRO.timer > 0)
                {
                    continue; // Not yet time to attack
                }
                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;

                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;
            }
        }
    }
}

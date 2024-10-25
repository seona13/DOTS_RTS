using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<FindTarget> findTarget,
            RefRW<Target> target)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<FindTarget>,
                RefRW<Target>>())
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (findTarget.ValueRO.timer > 0f)
            {
                // Timer not elapsed
                continue;
            }
            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            distanceHitList.Clear();
            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u, // all layers
                CollidesWith = 1u << GameAssets.UNITS_LAYER, // only interact with the units layer
                GroupIndex = 0,
            };

            Entity closestTargetEntity = Entity.Null;
            float closestTargetDistance = float.MaxValue;
            float currentTargetDistanceOffset = 0f;

            if (target.ValueRO.targetEntity != Entity.Null)
            {
                closestTargetEntity = target.ValueRO.targetEntity;
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                closestTargetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);
                currentTargetDistanceOffset = 2f;
            }

            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref distanceHitList, collisionFilter))
            {
                foreach (DistanceHit distanceHit in distanceHitList)
                {
                    if (SystemAPI.Exists(distanceHit.Entity) == false || SystemAPI.HasComponent<Unit>(distanceHit.Entity) == false)
                    {
                        continue; // Target entity is already dead
                    }

                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                    if (targetUnit.faction == findTarget.ValueRO.targetFaction)
                    {
                        // Valid target
                        if (closestTargetEntity == Entity.Null)
                        {
                            closestTargetEntity = distanceHit.Entity;
                            closestTargetDistance = distanceHit.Distance;
                        }
                        else
                        {
                            if (distanceHit.Distance + currentTargetDistanceOffset < closestTargetDistance)
                            {
                                closestTargetEntity = distanceHit.Entity;
                                closestTargetDistance = distanceHit.Distance;
                            }
                        }
                    }
                }
            }
            if (closestTargetEntity != Entity.Null)
            {
                target.ValueRW.targetEntity = closestTargetEntity;
            }
        }
    }
}

using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public struct ActiveAnimation : IComponentData
{
    public int frame;
    public int frameMax;
    public float frameTimer;
    public float frameTimerMax;

    public BatchMeshID frame0;
    public BatchMeshID frame1;
}

public class ActiveAnimationAuthoring : MonoBehaviour
{
    public AnimationDataSO soldierIdle;

    public class Baker : Baker<ActiveAnimationAuthoring>
    {
        public override void Bake(ActiveAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            AddComponent(entity, new ActiveAnimation
            {
                frame0 = entitiesGraphicsSystem.RegisterMesh(authoring.soldierIdle.meshArray[0]),
                frame1 = entitiesGraphicsSystem.RegisterMesh(authoring.soldierIdle.meshArray[1]),
                frameMax = authoring.soldierIdle.meshArray.Length,
                frameTimerMax = authoring.soldierIdle.frameTimerMax,
            });
        }
    }
}

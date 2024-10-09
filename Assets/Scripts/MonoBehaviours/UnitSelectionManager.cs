using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    private Vector2 selectionStartMousePosition;


    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Start unit selection
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0)) // End unit selection
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            // Deselected all currently-selected units
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            }

            // Test to see if multiple or single selection
            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSelectionSizeMin = 40f;
            bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;

            if (isMultipleSelection)
            {
                // Check which units are inside the selection rectangle and select them
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                for (int i = 0; i < localTransformArray.Length; i++)
                {
                    LocalTransform unitLocalTransform = localTransformArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        // Unit is inside the selection area
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                    }
                }
            } else
            {
                // Single selection
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                int unitsLayer = 6;
                RaycastInput raycastInput = new RaycastInput
                {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter
                    {
                        CollidesWith = 1u << unitsLayer, // only interact with the units layer
                        BelongsTo = ~0u, // all layers
                        GroupIndex = 0
                    }
                };
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
                {
                    if (entityManager.HasComponent<Unit>(raycastHit.Entity))
                    {
                        // Hit a unit
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                    }
                }
            }

            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonDown(1)) // Issue move command
        {
            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            for (int i = 0; i < unitMoverArray.Length; i++)
            {
                UnitMover unitMover = unitMoverArray[i];
                unitMover.targetPosition = mouseWorldPosition;
                unitMoverArray[i] = unitMover;
            }
            entityQuery.CopyFromComponentDataArray(unitMoverArray);
        }
    }


    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(
            Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y)
            );
        Vector2 upperRightCorner = new Vector2(
            Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y)
            );

        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y
            );
    }
}

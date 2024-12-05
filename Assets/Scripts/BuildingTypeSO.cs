using UnityEngine;

[CreateAssetMenu()]
public class BuildingTypeSO : ScriptableObject
{
    public enum BuildingType
    {
        None,
        ZombieSpawner,
        Tower,
        Barracks,
    }

    public BuildingType buildingType;
}

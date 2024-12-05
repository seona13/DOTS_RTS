using Unity.Entities;
using UnityEngine;

[CreateAssetMenu()]
public class UnitTypeSO : ScriptableObject
{
    public enum UnitType
    {
        None,
        Soldier,
        Scout,
        Zombie,
    }


    public UnitType unitType;
    public float progrssMax;


    public Entity GetPrefabEntity(EntitiesReferences entitiesReferences)
    {
        switch (unitType)
        {
            default:
            case UnitType.None:
            case UnitType.Soldier:  return entitiesReferences.soldierPrefabEntity;
            case UnitType.Scout:    return entitiesReferences.scoutPrefabEntity;
            case UnitType.Zombie:   return entitiesReferences.zombiePrefabEntity;
        }
    }
}

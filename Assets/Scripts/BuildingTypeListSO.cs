using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BuildingTypeListSO : ScriptableObject
{
    public List<BuildingTypeSO> buildingTypeSOList;


    public BuildingTypeSO GetBuildingTypeSO(BuildingTypeSO.BuildingType buildingType)
    {
        foreach (BuildingTypeSO buildingTypeSO in buildingTypeSOList)
        {
            if (buildingTypeSO.buildingType == buildingType)
            {
                return buildingTypeSO;
            }
        }
        Debug.LogError("Could not find BuildingTypeSO for Building Type " +  buildingType);
        return null;
    }
}

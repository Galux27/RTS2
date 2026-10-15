using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SettlementBuildingMoniter 
{
    public SettlementBuildingMoniter(SettlementBuildingData data)
    {
        Data = data;
        BuildingQuantities = new Dictionary<string, BuildingPlacementData>();
        Vector2Int sizeOfBuilding = new Vector2Int();
        for(int x = 0; x < data.BuildingsForSettlement.Count; x++)
        {
            BuildingQuantities.Add(data.BuildingsForSettlement[x].Building.BuildingName, new BuildingPlacementData());
            sizeOfBuilding.x = data.BuildingsForSettlement[x].Building.MaxWidth;
            sizeOfBuilding.y = data.BuildingsForSettlement[x].Building.MaxHeight;
            if (sizeOfBuilding.x > LargestBuildingSize.x)
            {
                LargestBuildingSize.x = sizeOfBuilding.x;
            }
            if(sizeOfBuilding.y> LargestBuildingSize.y)
            {
                LargestBuildingSize.y = sizeOfBuilding.y;
            }
        }
    }
    public SettlementBuildingData Data;
    Dictionary<string, BuildingPlacementData> BuildingQuantities;
    public Vector2Int  LargestBuildingSize;
    public List<SettlementBuilding> ValidData;
    public void SetValidBuildings(Vector2Int size)
    {
        ValidData=new List<SettlementBuilding>();
        for(int x = 0; x < Data.BuildingsForSettlement.Count; x++)
        {
            if (Data.BuildingsForSettlement[x].Building.MinWidth <=size.x && Data.BuildingsForSettlement[x].Building.MinHeight <= size.x)
            {
                ValidData.Add(Data.BuildingsForSettlement[x]);
            }
        }
    }
    List<SettlementBuilding> Candidates = new List<SettlementBuilding>();
    public SettlementBuilding GetBuilding(Vector2Int position,Vector2Int size)
    {
        Candidates.Clear();
        for(int x = 0; x < ValidData.Count; x++)
        {
            if (CouldPlaceBuilding(ValidData[x], position,size))
            {
                Candidates.Add(ValidData[x]);
            }
        }
        if (Candidates.Count == 0)
        {
            return null;
        }
        int index = Random.Range(0, Candidates.Count);
        OnPlaceBuilding(Candidates[index],position);
        return Candidates[index];
    }


    bool CouldPlaceBuilding(SettlementBuilding building, Vector2Int position,Vector2Int size)
    {
        Vector2Int avalible = size - position;
        if( size.x>=building.Building.MinWidth && size.y >= building.Building.MinHeight)
        {
            bool hasTooMany = false;
            switch (building.QuantityType)
            {
                case SettlementBuildingQuantityType.Unlimited:
                    break;
                case SettlementBuildingQuantityType.Limited:
                    hasTooMany = GetNumberOfBuilding(building)< building.Max;
                    break;
                default:
                    break;
            }
            if (hasTooMany)
            {
                return false;
            }
            bool farEnoughAway = true;
            switch (building.PlacementType)
            {
                case SettlementBuildingPlacementType.None:
                    break;
                case SettlementBuildingPlacementType.NotNearSame:
                    farEnoughAway = IsFarEnoughAwayFromOtherBuildingsOfType(building, position);
                    break;
                default:
                    break;
            }
            return farEnoughAway;

        }
        return false;
    }

    int GetNumberOfBuilding(SettlementBuilding building) 
    {
        return BuildingQuantities[building.Building.name].Positions.Count;
    }

    bool IsFarEnoughAwayFromOtherBuildingsOfType(SettlementBuilding building,Vector2Int pos)
    {
        List<Vector2> buildings = BuildingQuantities[building.Building.BuildingName].Positions;
        for(int x = 0; x < buildings.Count; x++)
        {
            if (Vector2.Distance(pos, buildings[x]) < building.MinDist)
            {
                return false;
            }
        }
        return true;
    }

    void OnPlaceBuilding(SettlementBuilding building,Vector2Int position)
    {
        BuildingQuantities[building.Building.name].Positions.Add(position);
    }


    

}

public class BuildingPlacementData
{
    public BuildingPlacementData()
    {
        Positions = new List<Vector2>();
    }

    public List<Vector2> Positions;

}

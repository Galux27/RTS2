using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettlementBuildingData", menuName = "ScriptableObjects/Settlement Building Data", order = 1)]
public class SettlementBuildingData : ScriptableObject 
{
    public string SettlementType;
    public List<SettlementBuilding> BuildingsForSettlement;
}

[System.Serializable]
public class SettlementBuilding
{
    public BuildingTemplate Building;
    public SettlementBuildingQuantityType QuantityType;
    public SettlementBuildingPlacementType PlacementType;
    public int Min, Max;
    public int MinDist, MaxDist;
}

public enum SettlementBuildingQuantityType {
 Unlimited,
 Limited
}
public enum SettlementBuildingPlacementType
{
    None,
    NotNearSame
}

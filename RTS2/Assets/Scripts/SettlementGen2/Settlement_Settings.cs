using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class Settlement_Settings
{



    public int StartingHighwayCount;
    public float DistBetweenHighways,DistBetweenAvenues,DistBetweenRoads,MinAvenueLengthForRoad;
    public bool GenerateHighwayStarts;
    public Vector2 Center, Size;
    public float HighwayDirOffsetScale,RiverBendScale;
    public int MaxAvenuePasses,MaxRoadPasses;
    public float HighwayLength, AvenueLength,RoadLength;

    public int RiverPoints, RiverWidth,RiverSectionLength;

    public List<Vector2> ManualHighwayPoints=new List<Vector2>(),
        ManualAvenuePoints = new List<Vector2>(),
        ManualRoadPoints = new List<Vector2>(),
        ManualRiverPoints = new List<Vector2>();
}

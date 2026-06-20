using UnityEngine;

[System.Serializable]
public class Settlement_Settings
{
    public int StartingHighwayCount;
    public float DistBetweenHighways,DistBetweenAvenues,DistBetweenRoads,MinAvenueLengthForRoad;
    public Vector2 Center, Size;
    public float HighwayDirOffsetScale;
    public int MaxAvenuePasses,MaxRoadPasses;
    public float HighwayLength, AvenueLength,RoadLength;
}

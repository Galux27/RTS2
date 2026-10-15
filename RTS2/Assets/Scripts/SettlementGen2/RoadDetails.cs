using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "RoadDetails", menuName = "ScriptableObjects/Road Details", order = 1)]
public class RoadDetails : ScriptableObject
{
    public RoadType RoadType;    
    public int RoadWidth;
    public string TileID;
    public bool HasEdge;
    public string EdgeTileID;
    public int EdgeWidth;

    public List<RoadProps> Props;
    public void CopyOverData(ref RoadData data)
    {
        data.Width = RoadWidth;
        data.HasEdge = HasEdge;
        data.EdgeWidth=EdgeWidth;
        data.EdgeTile = EdgeTileID;
        data.RoadTile = TileID;
    }
}

[System.Serializable]
public class RoadProps
{
    public string ObjectKey;
    public int DistanceBetweenInstances;
}



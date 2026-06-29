using UnityEngine;

[CreateAssetMenu(fileName = "RoadDetails", menuName = "ScriptableObjects/Road Details", order = 1)]
public class RoadDetails : ScriptableObject
{
    public RoadType RoadType;    
    public int RoadWidth;
    public string TileID;
    public bool HasEdge;
    public string EdgeTileID;
    public int EdgeWidth;
}



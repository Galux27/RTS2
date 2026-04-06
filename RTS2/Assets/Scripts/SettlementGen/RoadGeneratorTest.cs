using UnityEngine;

public class RoadGeneratorTest : MonoBehaviour
{
    public bool Generate = false;
    public string RoadTile, EdgeTile;
    public GameObject StartMarker, EndMarker;
    public int width = 5;

    void Update()
    {
        if (Generate)
        {
            RoadData data = new RoadData();
            data.StartPos=new Vector2Int((int)StartMarker.transform.position.x,(int)StartMarker.transform.position.y);
            data.EndPos = new Vector2Int((int)EndMarker.transform.position.x, (int)EndMarker.transform.position.y);
            data.HasEdge = true;
            data.EdgeTile = EdgeTile;
            data.Width = width; 
            data.RoadTile=RoadTile;
            RoadGenerator.GenerateRoad(data);
            Generate = false;
        }
    }
}

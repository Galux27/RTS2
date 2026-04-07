using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class RoadGeneratorTest : MonoBehaviour
{
    public bool Generate = false;
    public string RoadTile, EdgeTile;
    public GameObject StartMarker, EndMarker;
    public int width = 5;
    public List<RoadData> testRoads=new List<RoadData>();
    void Update()
    {
        if (Generate)
        {
            Generate = false;

            RoadData data = new RoadData(new Vector2Int((int)StartMarker.transform.position.x, (int)StartMarker.transform.position.y)
                , new Vector2Int((int)EndMarker.transform.position.x, (int)EndMarker.transform.position.y),width, RoadType.MajorRoad);        
            data.HasEdge = true;
            data.EdgeTile = EdgeTile;
            data.RoadTile=RoadTile;
            RoadGenerator.GenerateRoad(data,ref testRoads);
            testRoads.Add(data);
        }

        for(int x=0;x<RoadGenerator.AllRoads.Count;x++)
        {
            Debug.DrawLine(RoadGenerator.AllRoads[x].DebugStart(), RoadGenerator.AllRoads[x].DebugEnd(), Color.blue);
            Debug.DrawLine(RoadGenerator.AllRoads[x].DebugStart(), RoadGenerator.AllRoads[x].DebugStart()+ RoadGenerator.AllRoads[x].DebugPerp(), Color.red);

        }
    }
}

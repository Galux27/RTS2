using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPathTesting : MonoBehaviour
{

    public GameObject Marker1, Marker2;
    public bool FindPath=false,isPathNull=false;
    List<PathfindingNode> path;
    public int PathLength = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Vector2 batchCoords,TestCoords;
    void DrawNodesAroundPosition(Vector3 center)
    {
        Vector3 p = center;
       WorldTile node = null;
        PathfindingNode pathNode = null;
        batchCoords = new Vector2(WorldChunkManager.NewCalculateBatchCoords(TestCoords.x), WorldChunkManager.NewCalculateBatchCoords( TestCoords.y));
        for(int x=-5;x<=5;x++)
        {
            for(int y=-5;y<=5;y++)
            {
                p = center + new Vector3(x, y, 0);
              
                node = Pathfinding.GetTileFromPosition(p);
                pathNode = Pathfinding.GetNodeFromPosition(p);
                if(node != null)
                {
                    
                    for (int i = 0; i < pathNode.neighbours.Count; i++)
                    {
                        if (node.TileTraversable())
                        {
                            if (pathNode.IsPassable == false)
                            {
                                Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].worldPos, Color.green);

                            }
                            else
                            {
                                Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].worldPos, Color.blue);

                            }
                        }
                        else
                        {
                            if (node.Elevation.IsPassible())
                            {
                                Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].worldPos, Color.red);

                            }
                            else
                            {
                                Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].worldPos, Color.magenta);

                            }
                        }
                    }
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {

        DrawNodesAroundPosition(Marker1.transform.position);


        DrawNodesAroundPosition(Marker2.transform.position);

        if (FindPath)
        {
           path=  Pathfinding.FindPath(Marker1.transform.position, Marker2.transform.position);
            FindPath = false;
        }
        if (path != null && path.Count>0)
        {
            for(int x = 0; x < path.Count-1; x++)
            {
                Debug.DrawLine(path[x].worldPos, path[x+1].worldPos,Color.magenta);
            }
            isPathNull = false;
            PathLength = path.Count;
        }
        else
        {
            isPathNull = true;
        }
    }
}

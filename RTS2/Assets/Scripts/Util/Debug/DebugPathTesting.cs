using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPathTesting : MonoBehaviour
{

    public GameObject Marker1, Marker2;
    public bool FindPath=false,isPathNull=false;
    List<PathfindingNode> path;
    public List<int> PathGroupPath;
    public bool IsNull = false;
    public int PathLength = 0;
    TileRaycast tr;
    public int size = 5;
    PathfindingNode startNode, endNode;
    public int s, e;
    public List<int> sN, eN;

    public float TimeTakenForPath = 0, TimeTakenForGroups = 0;

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
        for(int x= -size; x <= size; x++)
        {
            for(int y= -size; y <= size; y++)
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
                                Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].Node.worldPos, Color.green);
                               // Debug.DrawLine(pathNode.worldPos, pathNode.worldPos + new Vector3(0, 0, -1*(pathNode.neighbours.Count + 1)), Color.magenta);
                            }
                            else
                            {
                                if (pathNode.neighbours[i].IsAccessable)
                                {
                                    Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].Node.worldPos, Color.blue);

                                }
                                else
                                {
                                    Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].Node.worldPos, Color.red);


                                }
                               // Debug.DrawLine(pathNode.worldPos, pathNode.worldPos + new Vector3(0, 0, -1 * (pathNode.neighbours.Count + 1)), Color.magenta);

                            }
                        }
                        else
                        {
                            if (node.Elevation.IsPassible())
                            {
                                Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].Node.worldPos, Color.red);
                                //Debug.DrawLine(pathNode.worldPos, pathNode.worldPos + new Vector3(0, 0, -1 * (pathNode.neighbours.Count + 1)), Color.magenta);

                            }
                            else
                            {
                                Debug.DrawLine(pathNode.worldPos, pathNode.neighbours[i].Node.worldPos, Color.magenta);
                               // Debug.DrawLine(pathNode.worldPos, pathNode.worldPos + new Vector3(0, 0, -1 * (pathNode.neighbours.Count + 1)), Color.magenta);

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
            EasyStopwatch.StartStopwatch();
           path=  Pathfinding.FindPath(Marker1.transform.position, Marker2.transform.position);
            EasyStopwatch.StopStopwatch();
            TimeTakenForPath=EasyStopwatch.GetStopwatchElapsedTime();
            FindPath = false;
            tr = new TileRaycast(Marker1.transform.position, Marker2.transform.position);
            tr.PerformRaycast();
            startNode = Pathfinding.GetNodeFromPosition(Marker1.transform.position);
            endNode = Pathfinding.GetNodeFromPosition(Marker2.transform.position);
            s = startNode.PathNodeGroupID;
            e = endNode.PathNodeGroupID;



            sN = NodeIDPathing.PathNodeIDs[s].NeighbouringIDs;
            eN = NodeIDPathing.PathNodeIDs[e].NeighbouringIDs;
            EasyStopwatch.StartStopwatch();

            PathGroupPath = NodeIDPathing.GetPath(startNode, endNode);
            EasyStopwatch.StopStopwatch();
            TimeTakenForGroups = EasyStopwatch.GetStopwatchElapsedTime();
            IsNull = PathGroupPath == null;
        }
        if (tr != null)
        {
            tr.DrawPath();
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

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

    // Update is called once per frame
    void Update()
    {

        PathfindingNode node =  Pathfinding.GetNodeFromPosition(Marker1.transform.position);
        for(int x = 0; x < node.neighbours.Count; x++)
        {
            Debug.DrawLine(node.worldPos, node.neighbours[x].worldPos,Color.blue);
        }

        node = Pathfinding.GetNodeFromPosition(Marker2.transform.position);
        for (int x = 0; x < node.neighbours.Count; x++)
        {
            Debug.DrawLine(node.worldPos, node.neighbours[x].worldPos, Color.red);
        }

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

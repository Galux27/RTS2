using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower
{



    public PathFollower(Unit FollowingPath)
    {
        followingPath = FollowingPath;
    }

    public PathFollower() 
    { 
    
    }

    Unit followingPath;


    public bool debugDrawPath = true;
    const float MinDistToPoint = .015f;
    List<PathfindingNode> pathfindingNodes;
    int currentIndex = 0;
    bool isPathDone = false;
    public bool HasPath()
    {
        return pathfindingNodes != null &&pathfindingNodes.Count > 0;
    }

    public void GetPath(Vector3 myPos,Vector3 targetPos)
    {
        pathfindingNodes = Pathfinding.FindPath(myPos, targetPos,followingPath);

    }

    Vector3 GetCurrentNode()
    {
        return pathfindingNodes[currentIndex].worldPos;
       
    }

    public Vector3 GetDirToNode(Vector3 curPos)
    {
        return (GetCurrentNode()-curPos).normalized;
    }

    public void OnUpdate(Vector3 curPos)
    {
        if(!isPathDone && HasPath())
        {
            if (Vector3.Distance(curPos, GetCurrentNode()) < MinDistToPoint)
            {
                currentIndex++;
              
                if(currentIndex >= pathfindingNodes.Count-1) {
                    isPathDone = true;
                    currentIndex = pathfindingNodes.Count - 1;
                    Cleanup();
                }
            }
        }

        if (debugDrawPath)
        {
            if (pathfindingNodes != null)
            {
                for (int x = 0; x < pathfindingNodes.Count - 1; x++)
                {
                    Debug.DrawLine(pathfindingNodes[x].worldPos, pathfindingNodes[x + 1].worldPos, Color.magenta);
                }
            }
        }
    }

    void Cleanup()
    {
        pathfindingNodes.Clear();
    }
}

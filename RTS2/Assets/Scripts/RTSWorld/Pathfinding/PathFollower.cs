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
    const float MinDistToPoint = .15f;
    List<PathfindingNode> pathfindingNodes;
    int currentIndex = 0;
    bool isPathDone = false;
    public bool HasPath()
    {
        return pathfindingNodes != null &&pathfindingNodes.Count > 0;
    }


    public void GetPath(Vector3 myPos, PathfindingNode targetPos)
    {
        EasyStopwatch.StartStopwatch();
        pathfindingNodes = Pathfinding.FindPath(myPos, targetPos, followingPath);
        EasyStopwatch.StopStopwatch();
        int count = 0;

        if (pathfindingNodes != null)
        {
            count = pathfindingNodes.Count;
        }
        Debug.Log("Trying to get path between " + myPos + " to " + targetPos + " length " + count+" took "+ EasyStopwatch.GetStopwatchElapsedTime());

    }

    public void GetPath(Vector3 myPos,Vector3 targetPos)
    {
        EasyStopwatch.StartStopwatch();
        pathfindingNodes = Pathfinding.FindPath(myPos, targetPos,followingPath);
        EasyStopwatch.StopStopwatch();
        int count = 0;

        if (pathfindingNodes != null)
        {
            count = pathfindingNodes.Count;
        }
        Debug.Log("Trying to get path between " + myPos + " to " + targetPos + " length " + count);

    }

    Vector3 GetCurrentNode()
    {
        return pathfindingNodes[currentIndex].worldPos;
       
    }

    public Vector3 GetLastNode()
    {
        return pathfindingNodes[pathfindingNodes.Count-1].worldPos;
    }


    public Vector3 GetDirToNode(Vector3 curPos)
    {
        return (GetCurrentNode()-curPos).normalized;
    }

    public void DoorCheck()
    {

        DoorSegment ds = WorldController.Instance.WallManager.IsThereADoorAtCoords(
            pathfindingNodes[currentIndex].X, pathfindingNodes[currentIndex].Y);
        if (ds!=null)
        {
            if (ds.UnitCanUseDoor(followingPath))
            {
                if (ds.NeedToOpenDoor())
                {
                    ds.OpenDoor();
                }
            }
        }
    }


    public void OnUpdate(Vector3 curPos)
    {
        if(!isPathDone && HasPath())
        {
            DoorCheck();
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

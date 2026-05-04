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
    public const float MinDistToPoint = .15f,NonPathMinDistToPoint=1.5f;
    List<PathfindingNode> pathfindingNodes;
    int currentIndex = 0;
    bool isPathDone = false;
    Unit toFollow;
    public bool HasPath()
    {
        return pathfindingNodes != null &&pathfindingNodes.Count > 0&& currentIndex <= pathfindingNodes.Count-1;
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
       // Debug.Log("Trying to get path between " + myPos + " to " + targetPos + " length " + count+" took "+ EasyStopwatch.GetStopwatchElapsedTime());
        toFollow = null;
        currentIndex = GetIndexToStartAt(myPos);
        isPathDone = false;


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
      //  Debug.Log("Trying to get path between " + myPos + " to " + targetPos + " length " + count);
        toFollow = null;
        currentIndex = GetIndexToStartAt(myPos);
        isPathDone = false;

    }

    public void SetTargetToFollow(Unit toFollow)
    {
        this.toFollow = toFollow;
    }

    public void GetPath(Vector3 myPos, Unit toPathTo)
    {
        EasyStopwatch.StartStopwatch();
        pathfindingNodes = Pathfinding.FindPath(myPos, toPathTo.transform.position, followingPath);
        EasyStopwatch.StopStopwatch();
        int count = 0;

        if (pathfindingNodes != null)
        {
            count = pathfindingNodes.Count;
        }
       // Debug.Log("Trying to get path between " + myPos + " to " + toPathTo.transform.position + " length " + count);
        toFollow = toPathTo;
        currentIndex = GetIndexToStartAt(myPos) ;
        isPathDone = false;
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

    int GetIndexToStartAt(Vector3 curPos)
    {
         if (pathfindingNodes == null || pathfindingNodes.Count == 0)
        {
            return 0;
        }
        int retVal = 1;
        float closest = 999999f, dist2 = 0f ;
       
        for(int x = 0; x < pathfindingNodes.Count; x++)
        {
            dist2 = Vector3.Distance(pathfindingNodes[x].worldPos, curPos);
            if (dist2 < closest)
            {
                retVal = x;
                closest = dist2;
            }
        }

        return retVal;
    }

    void CheckToUpdatePathWithTargetMovement(Vector3 curPos)
    {

        if (pathfindingNodes==null|| pathfindingNodes.Count==0
            || Vector3.Distance(toFollow.transform.position, pathfindingNodes[pathfindingNodes.Count - 1].worldPos) > 5f)
        {
            GetPath(curPos, toFollow);
        }
    }
    //add summit to find out whats the nearest index in a new path and start at that
    public void OnUpdate(Vector3 curPos)
    {
        if (toFollow != null)
        {
            CheckToUpdatePathWithTargetMovement(curPos);
        }
        if (!isPathDone && HasPath())
        {
           
            DoorCheck();
            if (Vector3.Distance(curPos, GetCurrentNode()) < MinDistToPoint)
            {
                if (currentIndex >= pathfindingNodes.Count - 1)
                {
                    isPathDone = true;
                    currentIndex = pathfindingNodes.Count - 1;
                    Cleanup();
                }
                else
                {
                    if (followingPath != null && pathfindingNodes != null && pathfindingNodes[currentIndex] != null)
                    {
                        followingPath.SetLastNode(pathfindingNodes[currentIndex]);
                    }
                }
               
                    currentIndex++;
              
              
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

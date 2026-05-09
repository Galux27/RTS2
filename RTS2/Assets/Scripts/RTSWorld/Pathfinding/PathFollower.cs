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

    public bool IsWaitingOnPath()
    {
        return gettingPath != null;
    }

    public void ResetFollower()
    {
        if (pathfindingNodes != null)
        {
            pathfindingNodes.Clear();
        }
            currentIndex = 0;
    }
    PathfindingMultiThreadedAction gettingPath = null;
    void ClearLastPathRequest()
    {
        if (gettingPath != null)
        {
            MultiThreadedManager.Instance.RemovePathRequest(gettingPath);
        }
    }


    void GetPathToPosition()
    {
        this.pathfindingNodes = Pathfinding.FindPath(startPos, endPos, followingPath); 
        gettingPath=null;
    }

    Vector3 startPos;
    PathfindingNode targetNode;
    Vector3 endPos;
    public void GetPathToNode()
    {
        this.pathfindingNodes = Pathfinding.FindPath(startPos, targetNode, followingPath);
        gettingPath = null;

    }

    public void GetPath(Vector3 myPos, PathfindingNode targetPos)
    {
        ClearLastPathRequest();
        this.startPos = myPos;
        this.targetNode = targetPos;
        gettingPath = MultiThreadedManager.Instance.AddPathfindingAction(() => GetPathToNode(),MultiThreadedManager.Instance.IsUnitHighPriority(followingPath));
        currentIndex = 0;
        isPathDone = false;


    }

    public void GetPath(Vector3 myPos,Vector3 targetPos)
    {
        ClearLastPathRequest();

        this.startPos = myPos;
        this.endPos = targetPos;
        gettingPath=MultiThreadedManager.Instance.AddPathfindingAction(() => GetPathToPosition(), MultiThreadedManager.Instance.IsUnitHighPriority(followingPath));
        currentIndex = 0;
        isPathDone = false;

    }

    public void SetTargetToFollow(Unit toFollow)
    {
        this.toFollow = toFollow;
    }

    public void GetPath(Vector3 myPos, Unit toPathTo)
    {
        ClearLastPathRequest();

        this.startPos = myPos;
        this.endPos = toPathTo.Position();
        toFollow = toPathTo;
        gettingPath = MultiThreadedManager.Instance.AddPathfindingAction(() => GetPathToPosition(), MultiThreadedManager.Instance.IsUnitHighPriority(followingPath));
        currentIndex = 0;
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
            if (!IsWaitingOnPath())
            {
                GetPath(curPos, toFollow);
            }
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

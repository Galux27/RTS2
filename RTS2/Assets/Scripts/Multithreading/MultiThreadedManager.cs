using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MultiThreadedManager : MonoBehaviour
{
    static MultiThreadedManager instance;
    public static MultiThreadedManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<MultiThreadedManager>();
            }
            return instance;
        }
    }

    List<MultiThreadedAction> actions = new List<MultiThreadedAction>();
    List<PathfindingMultiThreadedAction> pathRequests = new List<PathfindingMultiThreadedAction>();
    public int PathCount = 0;
    public void AddAction(Action toPerform,Action OnComplete,bool autoComplete=true)
    {
        MultiThreadedAction action = new MultiThreadedAction(toPerform,OnComplete,autoComplete);
        actions.Add(action);
        action.StartAction();
    }

    public bool IsUnitHighPriority(Unit toMove)
    {
        if (toMove.MyFaction.MyFactionID == FactionController.USER_FACTION || Vector3.Distance(toMove.Position(), CameraController.Instance.transform.position) < 55)
        {
            return true;
        }
        return false;
    }

  
    public PathfindingMultiThreadedAction AddPathfindingAction(Action toPerform,bool highPriority=false)
    {
        if (highPriority)
        {
            pathRequests.Insert(0,new PathfindingMultiThreadedAction(toPerform, null, true));
            return pathRequests[0];
        }
        else
        {
            pathRequests.Add(new PathfindingMultiThreadedAction(toPerform, null, true));
            return pathRequests[pathRequests.Count - 1];
        }
      
    }


    private void OnApplicationQuit()
    {
        StopAllActions();
    }

    private void OnDestroy()
    {
        StopAllActions();
    }

    private void Update()
    {

        UpdateMultiThreadedManager();
    }

    int index = 0;
    const int MAX_UPDATES = 500;
    void UpdateMultiThreadedManager()
    {
        int count = 0;

        while (index < actions.Count&&count<MAX_UPDATES)
        {
            actions[index].CheckForCompletion();
            index++;
            count++;
        }
        if (index >= actions.Count - 1)
        {
            index = 0;
        }
        if (pathRequests.Count > 0)
        {
            pathRequests[0].CheckForCompletion();
            PathCount = pathRequests.Count;
        }
    }

    public void OnActionComplete(MultiThreadedAction complete)
    {
        complete.OnComplete?.Invoke();
        complete.StopThread();      
        actions.Remove(complete);
    }

    public void RemovePathRequest(PathfindingMultiThreadedAction toRemove)
    {
        actions.Remove(toRemove);
    }

    public void OnActionComplete(PathfindingMultiThreadedAction complete)
    {
        complete.OnComplete?.Invoke();
        complete.StopThread();
        pathRequests.RemoveAt(0);
    }

    public void StopAllActions()
    {
        for(int x=0;x<actions.Count; x++)
        {
            try
            {
                actions[x].StopThread();
            }
            catch
            {

            }
        }
        actions.Clear();

        for (int x = 0; x < pathRequests.Count; x++)
        {
            try
            {
                if (pathRequests[x].Started())
                {
                    pathRequests[x].StopThread();
                }
                }
            catch
            {

            }
        }
        pathRequests.Clear();
    }
}

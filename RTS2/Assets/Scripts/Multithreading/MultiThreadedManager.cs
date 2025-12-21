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

    public void AddAction(Action toPerform,Action OnComplete,bool autoComplete=true)
    {
        MultiThreadedAction action = new MultiThreadedAction(toPerform,OnComplete,autoComplete);
        actions.Add(action);
        action.StartAction();
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
        int index = 0;
        while(index < actions.Count)
        {
            actions[index].CheckForCompletion();
        }
       
    }

    public void OnActionComplete(MultiThreadedAction complete)
    {
        complete.OnComplete?.Invoke();
        complete.StopThread();      
        actions.Remove(complete);
    }


    void StopAllActions()
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
    }
}

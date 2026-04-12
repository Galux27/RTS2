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
    }

    public void OnActionComplete(MultiThreadedAction complete)
    {
        complete.OnComplete?.Invoke();
        complete.StopThread();      
        actions.Remove(complete);
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
    }
}

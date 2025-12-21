using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class MultiThreadedAction
{
    public Action ToPerform,OnComplete;
    public bool IsComplete = false;
    public MultiThreadedAction(Action action,Action onComplete)
    {
        ToPerform = action;
        OnComplete = onComplete;
    }
    Thread MyThread;
    public void StartAction()
    {
        MyThread = new Thread(PerformAction);
        MyThread.Start();
    }

    void PerformAction()
    {
        
            ToPerform.Invoke();
        
        IsComplete = true;
    }

    public void CheckForCompletion()
    {
        if (IsComplete)
        {

            MultiThreadedManager.Instance.OnActionComplete(this);
        }
    }


    public void StopThread()
    {
        MyThread.Abort();
        
    }
}

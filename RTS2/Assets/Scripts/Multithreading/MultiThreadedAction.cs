using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using System.Runtime.CompilerServices;

public class MultiThreadedAction
{
    public Action ToPerform,OnComplete;
    public bool IsComplete = false,AutoComplete=true;
    public MultiThreadedAction(Action action,Action onComplete,bool autoComplete=false)
    {
        ToPerform = action;
        OnComplete = onComplete;
        AutoComplete=autoComplete;
    }
    Thread MyThread;

    public bool Started()
    {
        return MyThread != null;
    }

    public void StartAction()
    {
        MyThread = new Thread(PerformAction);
        MyThread.Priority = System.Threading.ThreadPriority.Highest;
        MyThread.IsBackground = true;
        MyThread.Start();
    }

    void PerformAction()
    {      
        if (AutoComplete)
        {
            ToPerform.Invoke();
            IsComplete = true;
        }
        else
        {
            while (!IsComplete)
            {
                ToPerform.Invoke();
            }
        }
    }

    public virtual void CheckForCompletion()
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

public class PathfindingMultiThreadedAction : MultiThreadedAction
{
    public PathfindingMultiThreadedAction(Action action, Action onComplete, bool autoComplete = false):base(action, onComplete, autoComplete)
    {
      
    }

    public override void CheckForCompletion()
    {
        if (!Started())
        {
            StartAction();
        }
        if (IsComplete)
        {
            MultiThreadedManager.Instance.OnActionComplete(this);
        }
    }
}

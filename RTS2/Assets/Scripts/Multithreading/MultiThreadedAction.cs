using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
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

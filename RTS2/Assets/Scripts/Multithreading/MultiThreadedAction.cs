using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class MultiThreadedAction
{
    public Action ToPerform,OnComplete;
    public bool IsComplete = false,AutoComplete=true,Killed=false;
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


    public void KillAction()
    {
        if (MyThread != null)
        {
            MyThread.Abort();
            MyThread = null;
        }
        
        IsComplete = true; 
        MyThread = null;
        Killed = true;
    }
   public virtual void PerformAction()
    {      
        if (AutoComplete)
        {
            try
            {
                ToPerform.Invoke();
            }catch(System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
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
    PathFollower Updating;
    public PathfindingMultiThreadedAction(Action action, Action onComplete,PathFollower updating, bool autoComplete = false):base(action, onComplete, autoComplete)
    {
         this.Updating= updating;
    }
    public override void PerformAction()
    {
        if (AutoComplete)
        {
            try
            {
                ToPerform.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                Updating.FailedPath = true;
            }
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
    public override void CheckForCompletion()
    {
        if (!Started())
        {
            AutoComplete = true;
            StartAction();
        }
        if (IsComplete)
        {
            MultiThreadedManager.Instance.OnActionComplete(this);
        }
    }
}

public class DataWritingAction : MultiThreadedAction
{
    public DataWritingAction(Action action, Action onComplete, bool autoComplete = false):base(action,onComplete,autoComplete)
    {
        
    }

    public override void CheckForCompletion()
    {
        if (IsComplete)
        {
            MultiThreadedManager.Instance.OnDataWriteComplete(this);
        }
    }
}

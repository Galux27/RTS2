using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer
{
    float timeVal = 0f, timeLimit = 0f;
    ProgressBarUI ProgressBarUI;
    public Timer(float TimeLimit, float initVal = 0f)
    {
        timeLimit = TimeLimit;
        timeVal = initVal;
    }

    public float TimeLimit { get { return timeLimit; } }
    public float GetCurrentTime { get { return timeVal; } }

    public void ProgressTime(float updateRate)
    {
        Debug.Log("Updating timer by " + updateRate);
        timeVal += updateRate;
        if (ProgressBarUI != null)
        {
            ProgressBarUI.UpdateCurrent(timeVal);
            if (IsTimerFinished())
            {
                ProgressBarUI.ReturnProgressBar();
            }
        }
    }

    public bool IsTimerFinished()
    {
        return timeVal >= timeLimit;
    }



    public void CreateProgressBarFromTimer(Vector3 pos)
    {
        ProgressBarUI = ProgressBarUI.CreateProgressBar();
        ProgressBarUI.InitProgressBar(TimeLimit, timeVal, pos);
    }
}

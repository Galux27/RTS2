using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehaviour_DeconstructObject : BehaviourBase
{
    ObjectInfo toDeconstruct;
    PathFollower follower;

    public void InitBehaviour(Unit toPerform, ObjectInfo obj)
    {
        base.InitBehaviour(toPerform);
        toDeconstruct = obj;
        follower = new PathFollower(toPerform);
        TargetPosition = toDeconstruct.Position();
        follower.GetPath(toPerform.transform.position, TargetPosition);
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour(performing, (ObjectInfo)IDManager.GetObjectByUID(Type.GetType((string)data[DataKeys.MiscString]),  (ulong)data[DataKeys.TargetUID]));
        maxTime = (float)data[DataKeys.MaxProgress];
        startTime = (float)data[DataKeys.CurrentProgress];
    }

    Vector3 TargetPosition;
    bool isDeconstructed = false;


    public override bool CanPerformBehaviour()
    {
        return unitToMove != null && isDeconstructed==false;
    }

    public override bool IsBehaviourComplete()
    {
        return isDeconstructed;
    }

    Vector3 DirectionToTarget()
    {
        if (follower.HasPath())
        {
            return follower.GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return Vector3.zero;//return (TargetPosition - unitToMove.transform.position).normalized;
        }
    }
    float startTime = 0f;
    float maxTime = -1f;


    ProgressBarUI progressBarUI;
    public override void PerformBehaviour()
    {
        if (isDeconstructed)
        {
            return;
        }
        float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
        if (dist > 1f)
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {
            if (progressBarUI == null)
            {
                progressBarUI = ProgressBarUI.CreateProgressBar();
                progressBarUI.InitProgressBar(MaxProgress(), startTime, toDeconstruct.Position());
            }

            progressBarUI.UpdateCurrent(DeltaTimeWrapper.GameplayDelta+progressBarUI.CurrentValue);
            Debug.Log("Destroy: progress " + progressBarUI.CurrentValue + "/" + progressBarUI.MaxValue+" is done "+ progressBarUI.IsDone());
            if (progressBarUI.IsDone())
            {
                progressBarUI.ReturnProgressBar();
                progressBarUI = null;
                DestroyObject();
                isDeconstructed = true;

            }
        }
    }

    float MaxProgress()
    {
        if(maxTime < 0f)
        {
            maxTime= toDeconstruct.MaxHealth() / 10f;
        }
        return maxTime;
    }

    float Progress()
    {
        if (progressBarUI == null)
        {
            return 0f;
        }
        return progressBarUI.CurrentValue;
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.TargetUID, toDeconstruct.MyUID().Value);
        data.AddDataToSerialize(DataKeys.CurrentProgress, Progress());
        data.AddDataToSerialize(DataKeys.MaxProgress,MaxProgress());
        data.AddDataToSerialize(DataKeys.MiscString, toDeconstruct.GetType());
        return data;
    }

    void DestroyObject()
    {
        if(toDeconstruct as EnvironmentObjectInstance != null)
        {
            EnvironmentObjectInstance obj = toDeconstruct as EnvironmentObjectInstance;
            obj.AdjustHealth(-9999999f);
        }else if(toDeconstruct as WallSegment != null)
        {
            WallSegment wall = toDeconstruct as WallSegment;
            wall.AdjustHealth(-9999999f);
        }
    }

}


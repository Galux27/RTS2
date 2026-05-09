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
        follower = toPerform.GetFollower();
        follower.ResetFollower();
        TargetPosition = toDeconstruct.Position();
        follower.GetPath(toPerform.transform.position, TargetPosition);
    }
    public void InitBehaviour(Unit toPerform, ObjectInfo obj, PathfindingNode targetNode)
    {
        base.InitBehaviour(toPerform);
        toDeconstruct = obj;
        follower = toPerform.GetFollower();
        follower.ResetFollower();
        TargetPosition = toDeconstruct.Position();
        follower.GetPath(toPerform.transform.position, targetNode);
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
    bool IsAtTarget()
    {
        if (follower.HasPath())
        {
            float dist = Vector3.Distance(unitToMove.transform.position, follower.GetLastNode());
            return dist < PathFollower.MinDistToPoint;
        }
        else
        {
            float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
            return dist < PathFollower.NonPathMinDistToPoint;
        }

    }

    ProgressBarUI progressBarUI;
    public override void PerformBehaviour()
    {
        if (isDeconstructed)
        {
            return;
        }
        if (!IsAtTarget())
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
            Debug.Log("Deconstruct: " + toDeconstruct.Health() + "/" + toDeconstruct.MaxHealth());
            toDeconstruct.AdjustHealth(DeltaTimeWrapper.GameplayDelta * -10f);

            progressBarUI.UpdateCurrent(toDeconstruct.Health());
            // Debug.Log("Destroy: progress " + progressBarUI.CurrentValue + "/" + progressBarUI.MaxValue+" is done "+ progressBarUI.IsDone());

            if (IsDeconstructed())
            {
                progressBarUI.ReturnProgressBar();
                progressBarUI = null;
                isDeconstructed = true;
                DestroyObject();

            }
            
        }
    }

    public bool IsDeconstructed()
    {
        if (toDeconstruct == null)
        {
            return true;
        }
        if (toDeconstruct as EnvironmentObjectInstance != null)
        {
            EnvironmentObjectInstance obj = toDeconstruct as EnvironmentObjectInstance;
            return obj == null || obj.Health()<= 0;
        }
        else if (toDeconstruct as WallSegment != null)
        {
            WallSegment wall = toDeconstruct as WallSegment;
            return wall.HasWall==false || wall.Health()<=0;
        }
        return false;
    }

    float MaxProgress()
    {
        if(maxTime < 0f)
        {
            maxTime= toDeconstruct.MaxHealth();
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (progressBarUI != null)
        {
            progressBarUI.ReturnProgressBar();
            progressBarUI = null;
            isDeconstructed = true;
        }
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehaviour_ChangeUnitType : BehaviourBase
{
    ConstructableObjectInstance toConstruct;
    PathFollower follower;
    string UnitTypeToBecome;
    bool convertedUnit = false;
    public void InitBehaviour(Unit toPerform, ConstructableObjectInstance obj,string unitType)
    {
        base.InitBehaviour(toPerform);
        toConstruct = obj;
        follower = new PathFollower(toPerform);
        TargetPosition = toConstruct.GetPosition();
        follower.GetPath(toPerform.transform.position, TargetPosition);
        UnitTypeToBecome = unitType;
    }


    Vector3 TargetPosition;

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.CurrentProgress, Progress());
        data.AddDataToSerialize(DataKeys.MaxProgress, MaxProgress());
        data.AddDataToSerialize(DataKeys.UnitType, UnitTypeToBecome);
        data.AddDataToSerialize(DataKeys.TargetUID, toConstruct.GetMyUID());
        return data;
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour(performing, (ConstructableObjectInstance)IDManager.GetObjectByUID(typeof(ConstructableObjectInstance),
            (ulong)data[DataKeys.TargetUID]), (string)data[DataKeys.UnitType]);
        maxTime = (float)data[DataKeys.MaxProgress];
        startTime = (float)data[DataKeys.CurrentProgress];
    }

    public override bool CanPerformBehaviour()
    {
        return unitToMove != null && toConstruct!=null;
    }

    public override bool IsBehaviourComplete()
    {
        return convertedUnit;
    }

    Vector3 DirectionToTarget()
    {
        if (follower.HasPath())
        {
            return follower.GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return Vector3.zero; //return (TargetPosition - unitToMove.transform.position).normalized;
        }
    }

    float Progress()
    {
        if (Timer == null)
        {
            return 0f;
        }
        return Timer.GetCurrentTime;
    }

    float MaxProgress()
    {
        if (maxTime < 0f)
        {
            maxTime= UnitTypesController.Instance.Units[UnitTypeToBecome].TrainingTime; 
        }
        return maxTime;
    }
    float maxTime = -1f;
    float startTime = 0f;
    Timer Timer;
    public override void PerformBehaviour()
    {
        float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
        if (dist > PathFollower.MinDistToPoint)
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {
            if (Timer == null)
            {
                Timer = new Timer(MaxProgress(), startTime);
                Timer.CreateProgressBarFromTimer(unitToMove.transform.position);
            }
            Timer.ProgressTime(DeltaTimeWrapper.GameplayDelta);
            if (Timer.IsTimerFinished())
            {
                if (UnitCapacityManager.GetMaxCapacityForUnitType(UnitTypeToBecome) > UnitMoniter.Instance.GetUserUnitCount(UnitTypeToBecome))
                {
                    UnitTrainingHelpers.TurnUnitIntoOtherUnit(unitToMove, UnitTypeToBecome);
                }
                convertedUnit = true;
            }
        }

    }

}
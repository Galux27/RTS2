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
        return data;
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
            return (TargetPosition - unitToMove.transform.position).normalized;
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
        return UnitTypesController.Instance.Units[UnitTypeToBecome].TrainingTime;
    }

    Timer Timer;
    public override void PerformBehaviour()
    {
        float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
        if (dist > 1f)
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {
            if (Timer == null)
            {
                Timer = new Timer(MaxProgress(), 0f);
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
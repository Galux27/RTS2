using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HumanAttackUnit_Behaviour : BehaviourBase
{
    Unit objectToFollow;
    float  DistRefreshTimer = TargetRefreshTimerLength;
    ObjectHealth healthOfUnitAttacking;
    PathFollower follower;
    Vector3 targetPosition;
    const float DistanceTargetRefresh = 2f, TargetRefreshTimerLength = 1f;

    public void InitBehaviour(Unit objectToFollow, Unit me)
    {
        InitBehaviour(me);
        this.objectToFollow = objectToFollow;
        healthOfUnitAttacking = objectToFollow.GetComponent<ObjectHealth>();
        follower = new PathFollower(me);
        targetPosition = objectToFollow.transform.position;
        follower.GetPath(me.transform.position, targetPosition);
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize dataToSerialize = new DataToSerialize();
        dataToSerialize.AddDataToSerialize(DataKeys.TargetUID, objectToFollow.GetMyUID().Value);
        return dataToSerialize;
    }


    public override bool CanPerformBehaviour()
    {
        return IsBehaviourComplete()==false;
    }

    public override bool IsBehaviourComplete()
    {
        return healthOfUnitAttacking.CurrentHealth <= 0;
    }

    Vector3 DirectionToTarget()
    {
        if (follower.HasPath() && InRangeToAttack()==false)
        {
            return follower.GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return (objectToFollow.transform.position - unitToMove.transform.position).normalized;
        }
    }

    bool InRangeToAttack()
    {
        if (unitToMove.MyAttackController.CanRangedAttack(objectToFollow.gameObject) || unitToMove.MyAttackController.CanMeleeAttack(objectToFollow.gameObject))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdatePath()
    {
        DistRefreshTimer -= DeltaTimeWrapper.GameplayDelta;
        if (DistRefreshTimer <= 0)
        {
            if(Vector3.Distance(targetPosition, objectToFollow.transform.position) > DistanceTargetRefresh)
            {
                targetPosition = objectToFollow.transform.position;
                follower.GetPath(unitToMove.transform.position, targetPosition);
            }
            DistRefreshTimer = TargetRefreshTimerLength;
        }
    }

    public override void PerformBehaviour()
    {
        UpdatePath();

        if (unitToMove.MyAttackController.CanRangedAttack(objectToFollow.gameObject) == false)
        {
         
            if (unitToMove.MyAttackController.CanMeleeAttack(objectToFollow.gameObject) == false && unitToMove.MyAttackController.HasRanged==false)
            {

                unitToMove.MoveUnit(DirectionToTarget());
            }
            else if (unitToMove.MyAttackController.CanMeleeAttack(objectToFollow.gameObject) == false)
            {
                if (unitToMove.GetOrderVal(OrderConstants.ORDER_PURSUE_ENEMIES))
                {
                    unitToMove.MoveUnit(DirectionToTarget());
                }
            }
        }
     
        unitToMove.MyAttackController.AttemptAttack(objectToFollow);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackTarget_Behaviour : BehaviourBase
{
    Unit objectToFollow;
    ObjectHealth healthOfUnitAttacking;
    public void InitBehaviour(Unit objectToFollow, Unit me)
    {
        InitBehaviour(me);
        this.objectToFollow = objectToFollow;
        healthOfUnitAttacking=objectToFollow.GetComponent<ObjectHealth>();
    }


    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return objectToFollow==null|| healthOfUnitAttacking.CurrentHealth <= 0;
    }

   

    Vector3 DirectionToTarget()
    {
        if (objectToFollow != null)
        {
            return (objectToFollow.transform.position - unitToMove.transform.position).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

  

    public override void PerformBehaviour()
    {
        if (objectToFollow != null)
        {
            if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, DirectionToTarget(),unitToMove))
            {
                unitToMove.MoveUnit(DirectionToTarget());
                unitToMove.MyAttackController.AttemptAttack(objectToFollow);
            }
        }
    }
}


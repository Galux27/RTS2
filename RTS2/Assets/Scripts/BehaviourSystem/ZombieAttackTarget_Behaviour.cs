using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackTarget_Behaviour : BehaviourBase
{
    Unit objectToFollow;
    float MinDistFrom = .25f;
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
        return healthOfUnitAttacking.CurrentHealth <= 0;
    }

   

    Vector3 DirectionToTarget()
    {

        return (objectToFollow.transform.position - unitToMove.transform.position).normalized;
    }

    public override void PerformBehaviour()
    {
        unitToMove.MoveUnit(DirectionToTarget());
        unitToMove.MyAttackController.AttemptAttack(objectToFollow);
    }
}


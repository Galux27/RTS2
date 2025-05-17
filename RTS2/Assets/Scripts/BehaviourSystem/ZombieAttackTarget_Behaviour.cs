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

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour((Unit)IDManager.GetObjectByUID(typeof(Unit), (ulong)data[DataKeys.TargetUID]), performing);
    }


    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return objectToFollow==null|| healthOfUnitAttacking.CurrentHealth <= 0||Vector3.Distance(objectToFollow.transform.position,unitToMove.transform.position)>25f;
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize behaviourSpecificData = new DataToSerialize();
        behaviourSpecificData.AddDataToSerialize(DataKeys.TargetUID, objectToFollow.GetMyUID().Value);

        return behaviourSpecificData;
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

    bool AreWeInRangeToAttack()
    {
        return Vector3.Distance(unitToMove.transform.position, objectToFollow.transform.position) < 1f ;
    }

    public override void PerformBehaviour()
    {
        if (objectToFollow != null)
        {
            if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, DirectionToTarget(),unitToMove))
            {
                if (!AreWeInRangeToAttack())
                {
                    unitToMove.MoveUnit(DirectionToTarget());
                }
                unitToMove.MyAttackController.AttemptAttack(objectToFollow);
            }
        }
    }

    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }
}


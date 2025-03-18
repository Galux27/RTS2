using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackObject_Behaviour : BehaviourBase
{
   
    ObjectInfo targetObject;
    public void InitBehaviour(ObjectInfo objectToATtack, Unit me)
    {
        InitBehaviour(me);
        targetObject=objectToATtack;
        Debug.Log("Object being attackled " + targetObject.Name() + " at " + targetObject.Position());
    }


    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return targetObject.Health() <= 0;
    }

   

    Vector3 DirectionToTarget()
    {

            return (targetObject.Position() - unitToMove.transform.position).normalized;

    }

  

    public override void PerformBehaviour()
    {
      
            if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, DirectionToTarget(),unitToMove))
            {
                unitToMove.MoveUnit(DirectionToTarget());
                unitToMove.MyAttackController.AttemptAttack(targetObject);
            }
       
    }
}


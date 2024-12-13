using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HumanBehaviour_ConstructObject : BehaviourBase
{
    ConstructableObjectInstance toConstruct;
    PathFollower follower;

    public void InitBehaviour(Unit toPerform,ConstructableObjectInstance obj)
    {
        base.InitBehaviour(toPerform);
        toConstruct = obj;
        follower = new PathFollower();
        TargetPosition = new Vector3(toConstruct.PosX, toConstruct.PosY);
        follower.GetPath(toPerform.transform.position, TargetPosition);
    }


    Vector3 TargetPosition;
   


    public override bool CanPerformBehaviour()
    {
        return  unitToMove != null && toConstruct.IsBuilt()==false;
    }

    public override bool IsBehaviourComplete()
    {
        return toConstruct.IsBuilt();
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

    public override void PerformBehaviour()
    {
        float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
        Debug.Log("Dist to building " + dist);
        if (dist > 1f)
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {
            Debug.Log("Is Built " + toConstruct.IsBuilt());
            if (toConstruct.IsBuilt() == false)
            {
                toConstruct.ConstructObject();
            }
            
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo_Behaviour : BehaviourBase
{
    public Vector3 TargetPosition;
    PathFollower follower;
    bool usePath = true;
    public void InitBehaviour(Unit toPerform, Vector3 targetPos, bool UsePath = true)
    {
        base.InitBehaviour(toPerform);
        TargetPosition = targetPos;
        usePath= UsePath;
        if (usePath)
        {
            follower = new PathFollower(toPerform);
            follower.GetPath(toPerform.transform.position, targetPos);
        }
    }

    public override bool CanPerformBehaviour()
    {
        return Vector3.Distance(unitToMove.transform.position, TargetPosition) >1f && unitToMove!=null;
    }

    public override bool IsBehaviourComplete()
    {
        return Vector3.Distance(unitToMove.transform.position, TargetPosition) < 1f;
    }

    Vector3 DirectionToTarget()
    {
        if (usePath &&follower.HasPath() )
        {
            return follower.GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return (TargetPosition-unitToMove.transform.position).normalized;
        }
    }

    public override void PerformBehaviour()
    {
        unitToMove.MoveUnit(DirectionToTarget());
        follower.OnUpdate(unitToMove.transform.position);
    }


}

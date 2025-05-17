using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFollowTarget_Behaviour : BehaviourBase
{
    GameObject objectToFollow;
    float MinDistFrom = 1f;
    public void InitBehaviour(GameObject objectToFollow, Unit me)
    {
        InitBehaviour(me);
        this.objectToFollow= objectToFollow; 
    }


    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour( (GameObject)IDManager.GetObjectByUID(typeof(Unit), (ulong)data[DataKeys.TargetUID]), performing);
    }

    float DistToTarget()
    {
        return Vector3.Distance(objectToFollow.transform.position, unitToMove.transform.position);
    }

    public override bool CanPerformBehaviour()
    {
        return DistToTarget() > MinDistFrom ;
    }

    public override bool IsBehaviourComplete()
    {
        return false;// DistToTarget()<= MinDistFrom ;
    }


    Vector3 DirectionToTarget()
    {

        return (objectToFollow.transform.position - unitToMove.transform.position).normalized;
    }

    public override void PerformBehaviour()
    {
        unitToMove.MoveUnit(DirectionToTarget());
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackObject_Behaviour : BehaviourBase
{
   
    ObjectInfo targetObject;
    Vector3 TargetPosition;
    public void InitBehaviour(ObjectInfo objectToATtack, Unit me)
    {
        InitBehaviour(me);
        targetObject=objectToATtack;
        SetTargetPosition();
    }


    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour((ObjectInfo)
            IDManager.GetObjectByUID(Type.GetType((string)data[DataKeys.MiscString]), (ulong)data[DataKeys.TargetUID]), performing);
    }

    void SetTargetPosition()
    {
        EnvironmentObject obj  = EnvironmentObjectHelpers.GetEnvironmentObject(targetObject.Name());
        if (obj == null)
        {
            TargetPosition = targetObject.Position();
        }
        else
        {
            Vector3 size = obj.Size();
            Vector3 dir = targetObject.Position() - unitToMove.Position();
            dir = dir.normalized;
            Vector3 offset = new Vector3((size.x*.5f) * dir.x, (size.y*.5f) * dir.y);
            TargetPosition = targetObject.Position() - offset;
        }
    }


    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return targetObject==null|| targetObject.Health() <= 0;
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize behaviourSpecificData = new DataToSerialize();
        behaviourSpecificData.AddDataToSerialize(DataKeys.MiscString, targetObject.GetType());
        behaviourSpecificData.AddDataToSerialize(DataKeys.TargetUID,targetObject.MyUID().Value);
        return behaviourSpecificData;
    }

    Vector3 DirectionToTarget()
    {

            return (TargetPosition - unitToMove.transform.position).normalized;

    }

    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }
   
    public override void PerformBehaviour()
    {
            if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, DirectionToTarget(),unitToMove))
            {
                unitToMove.MoveUnit(DirectionToTarget());
            }
        unitToMove.MyAttackController.AttemptAttack(targetObject, TargetPosition);

    }
}


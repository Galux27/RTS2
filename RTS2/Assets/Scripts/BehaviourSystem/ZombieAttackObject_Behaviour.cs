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

    Vector3 HorizontalDirectionToTarget()
    {
        Vector3 dir = DirectionToTarget();
        dir.y = 0;
        return dir;
    }
    Vector3 VerticalDirectionToTarget()
    {
        Vector3 dir = DirectionToTarget();
        dir.y = 0;
        return dir;
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
        }else if(BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position,VerticalDirectionToTarget(), unitToMove))
        {
            unitToMove.MoveUnit(VerticalDirectionToTarget());
        }
        else if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, HorizontalDirectionToTarget(), unitToMove))
        {
            unitToMove.MoveUnit(HorizontalDirectionToTarget());
        }
        unitToMove.MyAttackController.AttemptAttack(targetObject, TargetPosition);

    }

    public override List<string> GetDebugData()
    {
        List<string> retVal = new List<string>();

        retVal.Add("Target: " + targetObject.Position()+"("+TargetPosition+")");
        retVal.Add("Dist: " + Vector3.Distance(unitToMove.transform.position, TargetPosition));
        retVal.Add("Done: " + IsBehaviourComplete());
        retVal.Add("Can Move: " + BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, DirectionToTarget(), unitToMove));
        return retVal;
    }
}


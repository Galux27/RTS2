using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HumanBehaviour_ConstructObject : BehaviourBase
{
    Constructable toConstruct;
    PathFollower follower;

    public void InitBehaviour(Unit toPerform, Constructable obj)
    {
        base.InitBehaviour(toPerform);
        toConstruct = obj;
        follower = new PathFollower(toPerform);
        TargetPosition = toConstruct.GetPosition();
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

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour(performing, (Constructable)IDManager.GetObjectByUID(typeof(Constructable), (ulong)data[DataKeys.TargetUID]));
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.TargetUID, toConstruct.GetMyUID().Value);

        return data;
    }
    Vector3 DirectionToTarget()
    {
        if (follower.HasPath())
        {
            return follower.GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return Vector3.zero;//(TargetPosition - unitToMove.transform.position).normalized;
        }
    }

    public override void PerformBehaviour()
    {
        float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
        if (dist > 1f)
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {
            if (toConstruct.IsBuilt() == false)
            {
                toConstruct.ConstructObject();
            }
            
        }

    }

    public override KeyCode GetShortcutForBehaviour()
    {
        return KeyCode.C;
    }

}

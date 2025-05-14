using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectResources_Behaviour : BehaviourBase
{
    ResourceInstance toCollect;
    PathFollower follower;
    Vector3 TargetPosition;
    Inventory inventory;
    bool collected = false;
    public void InitBehaviour(Unit toPerform, ResourceInstance obj)
    {
        base.InitBehaviour(toPerform);
        toCollect = obj;
        follower = new PathFollower(toPerform);


        TargetPosition = toCollect.transform.position;
        inventory=toPerform.GetComponent<Inventory>();
        follower.GetPath(toPerform.transform.position, TargetPosition);
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.TargetUID, toCollect.GetMyUID().Value);

        return data;
    }


    public override bool CanPerformBehaviour()
    {
        return unitToMove != null && toCollect != null&& inventory.IsNotFull();
    }

    public override bool IsBehaviourComplete()
    {
        return collected;
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
        if (dist > 1f)
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {

            inventory.AddItemToInventory(toCollect.InstanceData);
            collected = true;
        }

    }
}

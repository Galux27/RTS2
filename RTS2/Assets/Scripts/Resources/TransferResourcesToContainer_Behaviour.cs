using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferResourcesToContainer_Behaviour : BehaviourBase
{
    Inventory toPutIn;
    Inventory transferingFrom;
    PathFollower follower;
    Vector3 TargetPosition;
    bool attemptedTransfer = false;
    public void InitBehaviour(Unit toPerform, Inventory targetInventory)
    {
        base.InitBehaviour(toPerform);
        toPutIn= targetInventory;
        transferingFrom=toPerform.GetComponent<Inventory>();
        follower = new PathFollower(toPerform);
        TargetPosition =toPutIn.transform.position;
        follower.GetPath(toPerform.transform.position, TargetPosition);
    }


    public override bool CanPerformBehaviour()
    {
        return unitToMove != null && toPutIn!=null && InventoryHelpers.DoesInventoryContainResource(transferingFrom) && toPutIn.IsNotFull();
    }

    public override bool IsBehaviourComplete()
    {
        return attemptedTransfer;
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.TargetUID,toPutIn.GetMyUID().Value);

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
            InventoryHelpers.TransferResourcesToContainer(toPutIn, transferingFrom);
            attemptedTransfer = true;

        }

    }
}

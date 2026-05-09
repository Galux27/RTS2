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
        follower = toPerform.GetFollower();
        follower.ResetFollower();


        TargetPosition = toCollect.transform.position;
        inventory=toPerform.GetComponent<Inventory>();
        follower.GetPath(toPerform.transform.position, TargetPosition);
    }
    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour(performing,(ResourceInstance)IDManager.GetObjectByUID(typeof(ResourceInstance), (ulong)data[DataKeys.TargetUID]) );
    }
    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.TargetUID, toCollect.GetMyUID().Value);

        return data;
    }


    public override bool CanPerformBehaviour()
    {
        return unitToMove != null && toCollect != null &&ResourceManager.Instance.DoWeHaveEnoughSpaceForResource(toCollect.InstanceData.Resource);
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
            return Vector3.zero;// return (TargetPosition - unitToMove.transform.position).normalized;
        }
    }

    public override void PerformBehaviour()
    {
        float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
        if (dist > PathFollower.MinDistToPoint)
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {
            ResourceManager.Instance.AddQuantityOfResource(toCollect.InstanceData.Name(), toCollect.InstanceData.Quantity);
            GameObject.Destroy(toCollect.gameObject);
            collected = true;
        }

    }
}

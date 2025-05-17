using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GatherResources_Behaviour : BehaviourBase
{
    EnvironmentObjectInstance toHarvest;
    PathFollower follower;
    Vector3 TargetPosition;
    public void InitBehaviour(Unit toPerform, EnvironmentObjectInstance obj)
    {
        base.InitBehaviour(toPerform);
        toHarvest = obj;
        follower = new PathFollower(toPerform);
        TargetPosition = toHarvest.GetPosition();
        follower.GetPath(toPerform.transform.position, TargetPosition);
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour(performing, (EnvironmentObjectInstance)IDManager.GetObjectByUID(typeof(EnvironmentObjectInstance), (ulong)data[DataKeys.TargetUID]));
    }
    public override bool CanPerformBehaviour()
    {
        return unitToMove != null && toHarvest.isHarvested==false;
    }

    public override bool IsBehaviourComplete()
    {
        return toHarvest.isHarvested;
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

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.TargetUID, toHarvest.GetMyUID().Value);

        return data;
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
            toHarvest.Harvest();
           
            //if (toConstruct.IsBuilt() == false)
            //{
            //    toConstruct.ConstructObject();
            //}

        }

    }

}

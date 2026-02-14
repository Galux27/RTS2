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

    public void InitBehaviour(Unit toPerform, EnvironmentObjectInstance obj,PathfindingNode targetNode)
    {
        base.InitBehaviour(toPerform);
        toHarvest = obj;
        follower = new PathFollower(toPerform);
        TargetPosition = toHarvest.GetPosition();
        follower.GetPath(toPerform.transform.position, targetNode);
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
    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
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

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.TargetUID, toHarvest.GetMyUID().Value);

        return data;
    }

    bool IsAtTarget()
    {
        if (follower.HasPath())
        {
            float dist = Vector3.Distance(unitToMove.transform.position, follower.GetLastNode());
            return dist < 1f;
        }
        else
        {
            float dist = Vector3.Distance(unitToMove.transform.position, TargetPosition);
            return dist< 1f;
        }
            
    }

    public override void PerformBehaviour()
    {
      
        if (!IsAtTarget())
        {
            follower.OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());
        }
        else
        {
            toHarvest.Harvest();  
        }

    }

    public override KeyCode GetShortcutForBehaviour()
    {
        return KeyCode.H;
    }

}

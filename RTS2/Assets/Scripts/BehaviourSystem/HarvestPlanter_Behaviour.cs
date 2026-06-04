using System.Collections.Generic;
using UnityEngine;

public class HarvestPlanter_Behaviour : BehaviourBase
{
    public Vector3 TargetPosition;
    PathFollower follower;
    bool usePath = true;
    PathfindingNode targetnode;
    EnvironmentObjectInstance target;
    public void InitBehaviour(Unit toPerform, EnvironmentObjectInstance target, bool UsePath = true)
    {
        base.InitBehaviour(toPerform);
        this.target = target;
        targetnode = UnitHelpers.GetWalkableNodesNearTarget(target.Position(), 50)[0];
        TargetPosition = targetnode.worldPos;
        usePath = UsePath;
        Debug.Log("Planter: init plant seeds behaviour " + TargetPosition);
        if (usePath)
        {
            follower = toPerform.GetFollower();
            follower.ResetFollower();
            follower.GetPath(toPerform.transform.position, targetnode);
        }
    }

    public void InitBehaviour(Unit toPerform, PathfindingNode target, bool UsePath)
    {
        base.InitBehaviour(toPerform);
        TargetPosition = target.worldPos;
        usePath = UsePath;
        if (usePath)
        {
            follower = toPerform.GetFollower();
            follower.ResetFollower();
            follower.GetPath(toPerform.transform.position, target);
        }
    }
    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }
    public override bool CanPerformBehaviour()
    {
        return Vector3.Distance(unitToMove.transform.position, TargetPosition) > PathFollower.MinDistToPoint && unitToMove != null;
    }
    bool done = false;
    public override bool IsBehaviourComplete()
    {
        return done;
    }

    Vector3 DirectionToTarget()
    {
        if (usePath && follower.HasPath())
        {
            return follower.GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return Vector3.zero;// return (TargetPosition-unitToMove.transform.position).normalized;
        }
    }
    ProgressBarUI progressBarUI;

    float HarvestingTimer = 5f;
    public override void PerformBehaviour()
    {
        if (!done)
        {
            Debug.Log("Planter: performing behaviour " + follower.HasPath() + " dist " + Vector3.Distance(unitToMove.Position(), TargetPosition) + "," + HarvestingTimer);
        }
        if (Vector3.Distance(unitToMove.Position(), TargetPosition) > .5f)
        {
            unitToMove.MoveUnit(DirectionToTarget());
            follower.OnUpdate(unitToMove.transform.position);
        }
        else
        {

            if (progressBarUI == null)
            {
                progressBarUI = ProgressBarUI.CreateProgressBar();
                progressBarUI.InitProgressBar(5f, 5f - HarvestingTimer, target.Position());
            }

            progressBarUI.UpdateCurrent(5f - HarvestingTimer);

            if (HarvestingTimer > 0)
            {
                HarvestingTimer -= DeltaTimeWrapper.GameplayDelta;
                if (HarvestingTimer <= 0)
                {
                    PlanterBehaviour pb = (PlanterBehaviour)target.myBehaviour;
                    if (pb != null)
                    {
                        pb.Harvest();
                        done = true;
                    }
                }
            }

            if (done)
            {
                progressBarUI.ReturnProgressBar();
                progressBarUI = null;

            }
        }
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.TargetUID, target.GetMyUID().Value);
        return retVal;
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        UID id = new UID((ulong)data[DataKeys.TargetUID]);
        InitBehaviour(performing, IDManager.GetObjectByUID(typeof(ConstructableObjectInstance), id.Value) as ConstructableObjectInstance);
    }

}
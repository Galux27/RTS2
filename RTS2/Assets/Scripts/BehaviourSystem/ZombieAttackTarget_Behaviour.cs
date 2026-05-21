using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackTarget_Behaviour : BehaviourBase
{
    Unit objectToFollow;
    EntityHealth healthOfUnitAttacking;
    PathFollower follower;
    float lastBackupPathTime = -1f;
    public void InitBehaviour(Unit objectToFollow, Unit toPerform)
    {
        InitBehaviour(toPerform);
        this.objectToFollow = objectToFollow;
        follower = toPerform.GetFollower();
        follower.ResetFollower();
        follower.SetTargetToFollow(objectToFollow);
        healthOfUnitAttacking =objectToFollow.MyHealth;
        toPerform.myRaycast = toPerform.GetRaycast(toPerform.transform.position, objectToFollow.transform.position);
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitBehaviour((Unit)IDManager.GetObjectByUID(typeof(Unit), (ulong)data[DataKeys.TargetUID]), performing);
    }


    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return objectToFollow==null
            || healthOfUnitAttacking.CurrentHealth <= 0 ;
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize behaviourSpecificData = new DataToSerialize();
        behaviourSpecificData.AddDataToSerialize(DataKeys.TargetUID, objectToFollow.GetMyUID().Value);

        return behaviourSpecificData;
    }

    Vector3 PathDirToTarget()
    {
        if (follower.HasPath())
        {
            return follower.GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 DirectDirectionToTarget()
    {
        if (objectToFollow != null)
        {
            return (objectToFollow.transform.position - unitToMove.transform.position).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }
    Vector3 DirectionToTarget()
    {
        if ( follower.HasPath())
        {
            return PathDirToTarget();
        }
        else
        {
            return DirectDirectionToTarget();
        }
    }
  
    Vector3 DirectionToEndOfRaycast(Unit performing)
    {
        return (performing.GetRaycast(performing.transform.position, objectToFollow.transform.position).GetFurthestTilePos()-performing.transform.position).normalized;
    }

    bool AreWeInRangeToAttack()
    {
        return Vector3.Distance(unitToMove.transform.position, objectToFollow.transform.position) < 1f ;
    }

    bool CanRaycastToTarget(Unit performing)
    {
        return performing.GetRaycast(performing.transform.position, objectToFollow.transform.position).
            DidRaycastHitEnd(objectToFollow.transform.position);
    }


    public override void PerformBehaviour()
    {
        if (objectToFollow != null)
        {
            if(follower.HasPath()) 
            {
                follower.OnUpdate(unitToMove.transform.position);
                {
                    if (!AreWeInRangeToAttack())
                    {
                        unitToMove.MoveUnit(PathDirToTarget());
                    }
                    unitToMove.MyAttackController.AttemptAttack(objectToFollow);
                }

            }
            else
            {
                if (CanRaycastToTarget(unitToMove))
                {
                    if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, DirectDirectionToTarget(), unitToMove))
                    {
                        if (!AreWeInRangeToAttack())
                        {
                            unitToMove.MoveUnit(DirectDirectionToTarget());
                        }
                        unitToMove.MyAttackController.AttemptAttack(objectToFollow);
                    }
                }
                else
                {
                    
                        if (GameTime.Instance.InGameTime - lastBackupPathTime > 5f&&!follower.IsWaitingOnPath())
                        {
                            follower.GetPath(unitToMove.transform.position, objectToFollow);
                            lastBackupPathTime = GameTime.Instance.InGameTime;
                        }
                }
            }
            
        }
    }
    public override List<string> GetDebugData()
    {
        List<string> data = new List<string>();
        data.Add("Has path: " + follower.HasPath());
        data.Add("Dir wanted: " + DirectionToTarget());
        data.Add("Target: " + objectToFollow.transform.position);
        data.Add("Can see target:" + CanRaycastToTarget(unitToMove));
        data.Add("target dist: " + Vector3.Distance(unitToMove.transform.position, objectToFollow.transform.position));
        data.Add("Target null: " + (objectToFollow == null));
        data.Add("Finished: " + IsBehaviourComplete().ToString());
        data.Add("Refresh Time: " + (GameTime.Instance.InGameTime - lastBackupPathTime));
        data.Add("Waiting for path: " + follower.IsWaitingOnPath());
        return data;
    }
    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }
}


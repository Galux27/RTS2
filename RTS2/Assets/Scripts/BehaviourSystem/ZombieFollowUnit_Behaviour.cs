using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ZombieFollowUnit_Behaviour : BehaviourBase
{
    //float MinDistFrom = 1f;

    //Vector2 direction = Vector2.zero;
    //float directionChangeTimer = 0f;
    //int count = 0;
    //const float directionChangeTimerLength = 10f;
    //static List<Vector2Int> PotentialDirections = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
    float timeInit = 0;
    Unit following;
    public void InitRoamBehaviour(Unit me,Unit toFollow)
    {
        Debug.Log("Zombie Following unit " + toFollow.Name() + " at " + toFollow.Position());
        InitBehaviour(me);
        timeInit = GameTime.Instance.InGameTime;
        following = toFollow;
        unitToMove.GetFollower().debugDrawPath = true;
        
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        //InitRoamBehaviour((Zombie)performing);
       // direction = (Vector3)data[DataKeys.Pos];
    }


    public override bool CanPerformBehaviour()
    {
        return following!=null;
    }

    public override bool IsBehaviourComplete()
    {
        return false;
    }


    Vector3 DirectionToTarget()
    {
        if (unitToMove.GetFollower().HasPath())
        {

            return unitToMove.GetFollower().GetDirToNode(unitToMove.transform.position);
        }
        else
        {
            return Vector3.zero;
        }
    }
        const float DistToRefreshPath = 5f;
    Vector3 lastFollowingPos= Vector3.zero;
    void CheckToSeeIfTargetHasMoved()
    {
        if(Vector3.Distance(following.transform.position,lastFollowingPos)> DistToRefreshPath)
        {
            unitToMove.GetFollower().GetPath(unitToMove.transform.position, following.transform.position);
            lastFollowingPos=following.transform.position;
        }
    }

    public override void PerformBehaviour()
    {
        if (following == null)
        {
            return;
        }
        CheckToSeeIfTargetHasMoved();

        if (unitToMove.GetFollower().HasPath())
        {
            unitToMove.GetFollower().OnUpdate(unitToMove.transform.position);
            unitToMove.MoveUnit(DirectionToTarget());

        }

        //if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, direction, unitToMove))
        //{
        //}
        //else
        //{
        //    GenerateDirectionToRoam();

        //    //  directionChangeTimer = directionChangeTimerLength;
        //}
        //directionChangeTimer += Mathf.Max(DeltaTimeWrapper.GameplayDelta, 0.01f);

        //if (directionChangeTimer > directionChangeTimerLength)
        //{
        //    GenerateDirectionToRoam();
        //    directionChangeTimer = 0f;
        //    count++;


        //}

    }

    public override List<string> GetDebugData()
    {
        List<string> retVal = new List<string>();
        retVal.Add(timeInit.ToString());
        retVal.Add(unitToMove.transform.position.ToString());
        retVal.Add((following == null).ToString());
        retVal.Add(unitToMove.GetFollower().HasPath().ToString());
     
        return retVal;
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.UID,following.GetMyUID());
        return data;
    }
    

    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }
}
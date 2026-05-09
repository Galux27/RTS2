using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;

public class ZombieRoam_Behaviour :BehaviourBase
{
    float MinDistFrom = 1f;

    Vector2 direction = Vector2.zero;
    float directionChangeTimer = 0f;
    int count = 0;
    const float directionChangeTimerLength = 10f;
   static List<Vector2Int> PotentialDirections = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
    float timeInit = 0;
    public void InitRoamBehaviour( Zombie me)
    {
        InitBehaviour(me);
        timeInit = GameTime.Instance.InGameTime;

        GenerateDirectionToRoam();
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitRoamBehaviour((Zombie)performing);
        direction = (Vector3)data[DataKeys.Pos];
    }


    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return false;
    }


    Vector3 DirectionToTarget()
    {

        return direction;
    }
   

    public override void PerformBehaviour()
    {

        if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position,direction,unitToMove))
            {
                unitToMove.MoveUnit(DirectionToTarget());
            }
            else
            {
            GenerateDirectionToRoam();

            //  directionChangeTimer = directionChangeTimerLength;
        }
        directionChangeTimer += Mathf.Max(DeltaTimeWrapper.GameplayDelta, 0.01f);

        if (directionChangeTimer > directionChangeTimerLength)
        {
            GenerateDirectionToRoam();
            directionChangeTimer = 0f;
            count++;

           
        }

     }

    public override List<string> GetDebugData()
    {
        List<string> retVal = new List<string>();
        retVal.Add(direction.ToString());
        retVal.Add(directionChangeTimer.ToString());
        retVal.Add(BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, direction, unitToMove).ToString());
        retVal.Add(timeInit.ToString());
        retVal.Add((GetType() != typeof(ZombieRoam_Behaviour)).ToString());
        return retVal;
    }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.Pos, direction);
        return data;
    }
    List<Vector2Int> validDirections = new List<Vector2Int>();
    void GenerateDirectionToRoam()
    {
        validDirections.Clear();
        for(int x=0;x< PotentialDirections.Count; x++)
        {
            if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, PotentialDirections[x], unitToMove))
            {
                validDirections.Add(PotentialDirections[x]);
            }
        }
        if (validDirections.Count > 0)
        {
            direction= validDirections[Random.Range(0,validDirections.Count)];
        }
        else
        {
            direction = Vector2.zero;

        }
    }

    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }
}

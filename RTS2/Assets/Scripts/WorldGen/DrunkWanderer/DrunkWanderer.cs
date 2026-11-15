using UnityEngine;
using System.Collections.Generic;
using System.Runtime;
public static class DrunkWanderer 
{
   
    public static int Width()
    {
        return WorldChunkManager.ChunkBatchSize * 16;
    }
    public static int Height()
    {
        return WorldChunkManager.ChunkBatchSize * 16;
    }

    public static Vector2Int GetDirectionToTarget(Vector2Int curPos,Vector2Int target,float startingDist)
    {
        Vector2Int retVal = new Vector2Int();

        if (curPos.x < target.x)
        {
            retVal.x = 1;
        }else if(curPos.x > target.x)
        {
            retVal.x = -1;
        }
        if (curPos.y < target.y)
        {
            retVal.y = 1;
        }
        else if (curPos.y > target.y)
        {
            retVal.y = -1;
        }
        float distToTarget = Vector2Int.Distance(curPos, target);
        float r = Random.Range(0f, startingDist);
        if (r < distToTarget+10)
        {
            int r2 = Random.Range(0, 100);
            if (r < 50)
            {

                retVal.x = Random.Range(-1, 1);
               
            }
            else
            {
                
                 retVal.y = Random.Range(-1, 1);

            }
        }
        Vector2Int pos = curPos + retVal;
        if (pos.x < 0|| pos.x >= Width())
        {
            retVal.x = 0;
        }
        if (pos.y < 0 || pos.y >= Width())
        {
            retVal.y = 0;
        }
        return retVal;
    }
}

public class DrunkWandererResult
{
    public DrunkWandererResult(int width,int height,List<Vector2Int> TargetPositions)
    {
        Width = width;
        Height = height;
        Data = new bool[width, height];
        Targets = new List<DrunkWandererTarget>();
        for(int x = 0; x < TargetPositions.Count; x++)
        {
            Targets.Add(new DrunkWandererTarget(TargetPositions[x]));
        }
        CalculateStartPosition();
        UpdateWanderers();
    }
    int Width, Height;
    public bool[,] Data;
    public List<DrunkWandererTarget> Targets;
    Vector2Int startPosition = new Vector2Int();

    void CalculateStartPosition()
    {
        for(int x = 0; x < Targets.Count; x++)
        {
            startPosition += Targets[x].position;
        }
        startPosition /= Targets.Count;
        for (int x = 0; x < Targets.Count; x++)
        {
            wanderers.Add(new DrunkWandererInstance(startPosition, Targets[x]));

            
        }
        Data[startPosition.x, startPosition.y] = true;
    }

    List<DrunkWandererInstance> wanderers = new List<DrunkWandererInstance>();


    public void UpdateWanderers()
    {
        int updateCount = 0;
        bool hasWanderersToUpdate = true;
        Vector2Int pos = new Vector2Int();

        while (hasWanderersToUpdate && updateCount < 1000)
        {
            hasWanderersToUpdate = false;
            for (int x = 0; x < wanderers.Count; x++)
            {
                if (!wanderers[x].ReachedTarget())
                {
                    pos = wanderers[x].Update();
                    Data[pos.x, pos.y] = true;
                    hasWanderersToUpdate = true;
                }
            }
            updateCount++;
        }
    }
}

public class DrunkWandererTarget
{
    public Vector2Int position;
    public bool reached;

    public DrunkWandererTarget(Vector2Int pos)
    {
        position = pos;
    }
}

public class DrunkWandererInstance
{
    public Vector2Int currentPosition;
    DrunkWandererTarget targetPosition;
    float startingDist = 0f;
    public DrunkWandererInstance(Vector2Int start, DrunkWandererTarget target)
    {
        currentPosition = start;
        targetPosition = target;
        startingDist=Vector2Int.Distance(start, targetPosition.position);
    }

    public bool ReachedTarget()
    {
        return targetPosition.reached ;
    }

    public Vector2Int Update()
    {
        currentPosition += DrunkWanderer.GetDirectionToTarget(currentPosition, targetPosition.position,startingDist);
        if (currentPosition == targetPosition.position)
        {
            targetPosition.reached = true;
        }
        return currentPosition;
    }
}

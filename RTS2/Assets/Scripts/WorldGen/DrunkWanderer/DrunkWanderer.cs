using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
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
        int r = Random.Range(0, 100);
        if (r < 25)
        {
            retVal.x = Random.Range(-1, 1);
        }else if (r < 50)
        {
            retVal.y = Random.Range(-1, 1);
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
    public DrunkWandererResult(int width,int height,List< List<Vector2Int>> TargetPositions,Vector2Int bottomCorner)
    {
        BottomCorner = bottomCorner;
        Width = width;
        Height = height;
        Data = new bool[width, height];
        Targets = new List<DrunkWandererTarget>();
        Debug.Log("River Gen: Generating with points " + TargetPositions.Count);
        for(int x = 0; x < TargetPositions.Count; x++)
        {
            Targets.Add(new DrunkWandererTarget(TargetPositions[x]));
        }
        CalculateStartPosition();
        UpdateWanderers();
    }
    int Width, Height;
    public Vector2Int BottomCorner;
    public bool[,] Data;
    public List<DrunkWandererTarget> Targets;
    Vector2Int startPosition = new Vector2Int();

    void CalculateStartPosition()
    {
      
        for (int x = 0; x < Targets.Count; x++)
        {
            wanderers.Add(new DrunkWandererInstance( Targets[x]));  
        }
    }

    List<DrunkWandererInstance> wanderers = new List<DrunkWandererInstance>();
    public void ExpandData(int size)
    {
        int width = DrunkWanderer.Width();
        int height = DrunkWanderer.Height();
        int dataWidth = Data.GetLength(0);
        int dataHeight = Data.GetLength(1);
        bool[,] expandedData = new bool[width, height];
        int count = 0;
        for (int x = 0; x < dataWidth; x++)
        {
            for (int y = 0; y < dataHeight; y++)
            {
                if (Data[x, y])
                {
                    for (int x1 = x - size; x1 < x + size; x1++)
                    {
                        for (int y1 = y - size; y1 < y + size; y1++)
                        {
                            if (x1 >= 0 && y1 >= 0 && x1 < width - 1 && y1 < height - 1)
                            {
                                expandedData[x1, y1] = true;
                                count++;
                            }
                        }
                    }
                }
            }
        }
        for (int x = 0; x < dataWidth; x++)
        {
            for (int y = 0; y < dataHeight; y++)
            {
                if (expandedData[x, y])
                {
                    Data[x, y] = true;
                }
            }
        }
        Debug.Log("River Gen: Points expanded " +count);

    }



    public void UpdateWanderers()
    {
        int updateCount = 0;
        bool hasWanderersToUpdate = true;
        Vector2Int pos = new Vector2Int();

        while (hasWanderersToUpdate && updateCount < 2000)
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
        ExpandData(3);
    }
}

public class DrunkWandererTarget
{
    public List<Vector2Int> positions;
    public bool reached;

    public DrunkWandererTarget(List<Vector2Int> positions)
    {
        this.positions = positions;
    }

    public Vector2Int Position(int index)
    {
        return positions[Mathf.Clamp( index,0,positions.Count-1)];
    }
}

public class DrunkWandererInstance
{
    public Vector2Int currentPosition;
    DrunkWandererTarget targetPosition;
    int currentIndex = 0;
    float startingDist = 0f;
    public DrunkWandererInstance( DrunkWandererTarget target)
    {
        currentPosition = target.Position(0);
        targetPosition = target;
        startingDist=Vector2Int.Distance(currentPosition, targetPosition.Position(currentIndex+1));
    }

    public bool ReachedTarget()
    {
        return targetPosition.reached ;
    }

    public Vector2Int Update()
    {
        currentPosition += DrunkWanderer.GetDirectionToTarget(currentPosition, targetPosition.Position(currentIndex+1), startingDist);
        if (currentPosition == targetPosition.Position(currentIndex+1))
        {
            if (currentIndex < targetPosition.positions.Count-1)
            {
                currentIndex++;
                startingDist = Vector2Int.Distance(targetPosition.Position(currentIndex), targetPosition.Position(currentIndex+1));

            }
            else
            {
                targetPosition.reached = true;

            }
        }
        return currentPosition;
    }
}

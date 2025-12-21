using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ALifeActions
{



    public static void Idle(ALifeEntity performing)
    {

    }
    static System.Random ran = new System.Random();
    static Vector2Int Dir, Pos, NewChunk;
    static Vector2 DirVec2;
    static float Dist;
    static bool HasMovedChunk(Vector2Int pos, out Vector2Int chunkDir)
    {
        chunkDir = new Vector2Int();
        if (pos.x < 0 || pos.y < 0 || pos.y >= WorldChunkManager.ChunkBatchSize || pos.x >= WorldChunkManager.ChunkBatchSize)
        {
            if (pos.x < 0)
            {
                chunkDir.x = -1;
            } else if (pos.x > WorldChunkManager.ChunkBatchSize)
            {
                chunkDir.x = 1;
            }
            if (pos.y < 0)
            {
                chunkDir.y = -1;
            }
            else if (pos.y > WorldChunkManager.ChunkBatchSize)
            {
                chunkDir.y = 1;
            }
            return true;
        }
        return false;
    }

    static bool IsNewChunkValid(Vector2Int coords)
    {
        return !(coords.y < 0 || coords.x < 0 || coords.y >= OverworldGenerator.Instance.OverworldHeight || coords.x >= OverworldGenerator.Instance.OverworldWidth);
    }

    static void ConvertLocalPosToNewChunkPos()
    {
        if(NewChunk.x < 0)
        {
            Pos.x = WorldChunkManager.ChunkBatchSize+Pos.x;
        }else if (NewChunk.x < 0)
        {
            Pos.x = WorldChunkManager.ChunkBatchSize - Pos.x;
        }

        if (NewChunk.y < 0)
        {
            Pos.y = WorldChunkManager.ChunkBatchSize + Pos.y;
        }
        else if (NewChunk.y < 0)
        {
            Pos.y = WorldChunkManager.ChunkBatchSize - Pos.y;
        }
    }
    static int debugRan=0;
    public static void Roam(ALifeEntity performing)
    {
        performing.PreviousBatchCoords=performing.CurrentBatchCoords;

        Dir = new Vector2Int(ran.Next(-2,2),ran.Next(-2,2))*performing.MoveSpeed;
        Pos = performing.LocalCoords + Dir;


        bool MovedChunk = HasMovedChunk(Pos,out NewChunk);

        

        if (MovedChunk)
        {
            performing.CurrentBatchCoords += NewChunk;
            if (IsNewChunkValid(performing.CurrentBatchCoords))
            {
                ConvertLocalPosToNewChunkPos();
                performing.LocalCoords = Pos;
                MoveBetweenChunks(performing);

            }
            else
            {
                performing.CurrentBatchCoords = performing.PreviousBatchCoords;
            }

        }
        else
        {
            performing.LocalCoords = Pos;
        }
        performing.LocalCoords = ClampToArray(performing.LocalCoords);
    }
    static Vector2Int ClampToArray(Vector2Int pos)
    {
        pos.x = Mathf.Clamp(pos.x, 0, WorldChunkManager.ChunkBatchSize - 1);
        pos.y = Mathf.Clamp(pos.y, 0, WorldChunkManager.ChunkBatchSize - 1);
        return pos;
    }
    public static void MoveBetweenChunks(ALifeEntity performing)
    {
        OverworldTile target = OverworldGenerator.Instance.GetOverworldTile(performing.CurrentBatchCoords);
        if (target.GetQuantitiyOfFeature(OverworldFeature.LargeWaterBody)>0)
        {
            performing.CurrentBatchCoords = performing.PreviousBatchCoords;
            return;
        }
        OverworldGenerator.Instance.GetOverworldTile(performing.PreviousBatchCoords).RemoveALifeEntity(performing);
        target.AddALifeEntity(performing);
    }

    public static void MoveTowardsEntity(ALifeEntity performing,ALifeEntity target)
    {
        MoveTowardsPosition(performing,target.LocalCoords);
    }

    public static void MoveTowardsPosition(ALifeEntity performing,Vector2Int target)
    {
        DirVec2 = ((target - performing.LocalCoords));
        DirVec2 = DirVec2.normalized*Mathf.Min( performing.MoveSpeed,Vector2Int.Distance(performing.LocalCoords,target));
        Dir.x = Mathf.RoundToInt(DirVec2.x);
        Dir.y = Mathf.RoundToInt(DirVec2.y);
        performing.LocalCoords += Dir;
    }

    public static void AttackTarget(ALifeEntity performing,ALifeEntity target,bool useRanged)
    {
        if (useRanged)
        {
            target.Health -= performing.RangedDamage;
            if (target.Health <= 0)
            {
                target.isDead = true;
            }
        }
        else
        {
            target.Health -= performing.AttackDamage;
            performing.Health -= target.AttackDamage;
            if (target.Health <= 0)
            {
                target.isDead = true;
            }
        }
    }
   

}

public static class ALifeDecisionMaker
{
    static float RNG = 0f;
    static System.Random ran = new System.Random();

    public static void MakeZombieDecisions(ALifeEntity performing)
    {
        if(performing.isDead||performing.isActive)
        {
            return;
        }

        RNG = ran.Next(0, 100);
        if (RNG < 75)
        {
            ALifeActions.Roam(performing);
        }
    }
}


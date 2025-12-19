using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ALifeActions
{



    public static void Idle(ALifeEntity performing)
    {

    }
    static System.Random ran = new System.Random();

    public static void Roam(ALifeEntity performing)
    {
        performing.PreviousBatchCoords=performing.CurrentBatchCoords;
        if (performing.CurrentBatchCoords.x == 0)
        {
            
            performing.CurrentBatchCoords.x += ran.Next(0,1);
        }
        else if(performing.CurrentBatchCoords.x == OverworldGenerator.Instance.OverworldWidth) 
        {
            performing.CurrentBatchCoords.x += ran.Next(-1, 0);

        }
        else
        {
            performing.CurrentBatchCoords.x += ran.Next(-1, 1);

        }

        if (performing.CurrentBatchCoords.y == 0)
        {
            performing.CurrentBatchCoords.y += ran.Next(0, 1);
        }
        else if (performing.CurrentBatchCoords.y == OverworldGenerator.Instance.OverworldHeight)
        {
            performing.CurrentBatchCoords.y += ran.Next(-1, 0);

        }
        else
        {
            performing.CurrentBatchCoords.y += ran.Next(-1, 1);

        }
        MoveBetweenChunks(performing);
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


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
        performing.PreviousCoords=performing.CurrentCoords;
        if (performing.CurrentCoords.x == 0)
        {
            
            performing.CurrentCoords.x += ran.Next(0,1);
        }
        else if(performing.CurrentCoords.x == OverworldGenerator.Instance.OverworldWidth) 
        {
            performing.CurrentCoords.x += ran.Next(-1, 0);

        }
        else
        {
            performing.CurrentCoords.x += ran.Next(-1, 1);

        }

        if (performing.CurrentCoords.y == 0)
        {
            performing.CurrentCoords.y += ran.Next(0, 1);
        }
        else if (performing.CurrentCoords.y == OverworldGenerator.Instance.OverworldHeight)
        {
            performing.CurrentCoords.y += ran.Next(-1, 0);

        }
        else
        {
            performing.CurrentCoords.y += ran.Next(-1, 1);

        }
        MoveBetweenChunks(performing);
    }

    public static void MoveBetweenChunks(ALifeEntity performing)
    {
        OverworldTile target = OverworldGenerator.Instance.GetOverworldTile(performing.CurrentCoords);
        if (target.GetQuantitiyOfFeature(OverworldFeature.LargeWaterBody)>0)
        {
            performing.CurrentCoords = performing.PreviousCoords;
            return;
        }
        OverworldGenerator.Instance.GetOverworldTile(performing.PreviousCoords).RemoveALifeEntity(performing);
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


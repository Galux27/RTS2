using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public  static class SettlementGenerator 
{
    public static GeneratedSettlement GenerateSettlement(Settlement_Settings settings)
    {
        validRoads.Clear();
        workingCopy.Clear();
        highways.Clear();
        DoneHighways = false;
        GeneratedSettlement settlement=new GeneratedSettlement();
        Vector2 startPos =GetEdge(settings);
        Vector2 dir = settings.Center - startPos;

        for(int x = 0; x < settings.StartingHighwayCount; x++)
        {
            if (IsPointFarEnoughAway(settlement.highways, settings.DistBetweenHighways ,startPos))
            {
                validRoads.Add(new Settlement_Road(startPos, dir.normalized, dir.magnitude * Random.Range(.25f, .5f)));
                //settlement.AddHighway();
               
            }
            startPos= GetEdge(settings);
            dir = settings.Center - startPos;
        }

        HighwayGenerationPass(settings);
        settlement.highways = highways;
        return settlement;
    }

    static List<Settlement_Road> workingCopy = new List<Settlement_Road>(), validRoads = new List<Settlement_Road>(),highways=new List<Settlement_Road>();
    static bool DoneHighways = false;
    static void HighwayGenerationPass(Settlement_Settings settings)
    {
        while (!DoneHighways)
        {
            for (int x = 0; x < validRoads.Count; x++)
            {
                if (!validRoads[x].EndedByLink)
                {
                    GenerateHighway(validRoads[x],settings);
                }
            }
            validRoads.Clear();
            validRoads.AddRange(workingCopy);
            workingCopy.Clear();
            if (validRoads.Count == 0)
            {
                DoneHighways = true;
            }
        }
        validRoads.Clear();
        workingCopy.Clear();
     }

    static Vector2 intersection,newStart;
    static void GenerateHighway(Settlement_Road original,Settlement_Settings settings)
    {
        bool foundEnd = false;
        Vector2 newEndPoint = original.StartPos + (original.Direction * original.Length);

        for (int x = 0; x < highways.Count; x++) 
        {
            if (LineUtil.IntersectRoadSegments2D(original.StartPos, newEndPoint, highways[x].StartPos, highways[x].EndPos,out intersection)) {
                if (Vector2.Distance(original.StartPos, highways[x].EndPos) > 1f)
                {
                    foundEnd = true;
                    newEndPoint = intersection;
                    break;
                }
            }
           
        }

        //original checks for if the new end positions are close to the start/end positions of the existing roads
        //
        original.EndPos = newEndPoint;
        highways.Add(original);

        if (!foundEnd &&!IsPositionNearEdge(newEndPoint,settings))
        {
            newStart = newEndPoint;
            workingCopy.Add(new Settlement_Road(newStart, (original.Direction + new Vector2(Random.Range(-.1f,.1f)*settings.HighwayDirOffsetScale,Random.Range(-.1f,.1f) * settings.HighwayDirOffsetScale)).normalized, original.Length));
        }

    }


    static bool IsPositionNearEdge(Vector2 pos,Settlement_Settings settings)
    {
        if (pos.x < settings.Center.x - (settings.Size.x / 2f))
        {
            return true;
        }
        if (pos.x > settings.Center.x + (settings.Size.x / 2f))
        {
            return true;
        }

        if (pos.y < settings.Center.y - (settings.Size.y / 2f))
        {
            return true;
        }
        if (pos.y > settings.Center.y + (settings.Size.y / 2f))
        {
            return true;
        }
        return false;
    }

    static bool IsPointFarEnoughAway(List<Settlement_Road> toCheck,float minDist,Vector2 position)
    {
        for(int x=0;x < toCheck.Count; x++)
        {
            if (toCheck[x].IsPointCloseToPositions(position, minDist))
            {
                return false;
            }
        }

        return true;
    }

    static Vector2 GetEdge(Settlement_Settings settings)
    {
        Vector2 edge = new Vector2();
        int r = Random.Range(0, 100);
        if (r < 50)
        {
            r = Random.Range(0, 100);
            if (r < 50)
            {
                edge.x = settings.Center.x + (settings.Size.x / 2);
                edge.y = Random.Range(settings.Center.y - (settings.Size.y / 2), settings.Center.y + (settings.Size.y / 2));
            }
            else
            {
                edge.x = settings.Center.x - (settings.Size.x / 2);
                edge.y = Random.Range(settings.Center.y - (settings.Size.y / 2), settings.Center.y + (settings.Size.y / 2));


            }
        }
        else
        {

            r = Random.Range(0, 100);
            if (r < 50)
            {
                edge.y = settings.Center.y + (settings.Size.y / 2);
                edge.x = Random.Range(settings.Center.x - (settings.Size.x / 2), settings.Center.x + (settings.Size.x / 2));
            }
            else
            {
                edge.y = settings.Center.y - (settings.Size.y / 2);
                edge.x = Random.Range(settings.Center.x - (settings.Size.x / 2), settings.Center.x + (settings.Size.x / 2));
            }
        }
      
        return edge;
    }
}

[System.Serializable]
public class GeneratedSettlement
{
    public List<Settlement_Road> highways=new List<Settlement_Road>(),avenues = new List<Settlement_Road>(), roads = new List<Settlement_Road>();
    public void AddHighway(Settlement_Road road)
    {
        highways.Add(road);
    }
}

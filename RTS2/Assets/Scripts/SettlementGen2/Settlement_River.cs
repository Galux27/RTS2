using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Settlement_River
{
    public List<Settlement_RiverSection> RiverSections = new List<Settlement_RiverSection>();
    public Vector2 MeetingPoint;

    public Settlement_River(Vector2 meetingPoint)
    {
        MeetingPoint = meetingPoint;
    }


}

public static class RiverGenerator
{
    static bool RiverGenerated = false;
    public static Settlement_River GenerateRiver(Settlement_Settings settings)
    {
        RiverGenerated = false;
        Settlement_River river = new Settlement_River(settings.Center);
        GenerateInitialPoints(settings);

        PerformRiverGenerationPass(settings, river);
        return river;
    }
    static List<Settlement_RiverSection> SectionWorkingCopy = new List<Settlement_RiverSection>(),Generated=new List<Settlement_RiverSection>();
    static void GenerateInitialPoints(Settlement_Settings settings)
    {
        Vector2 startPos = GetEdge(settings);
        Vector2 dir = settings.Center - startPos;
        for (int x = 0; x < settings.RiverPoints; x++)
        {
            SectionWorkingCopy.Add(new Settlement_RiverSection(startPos, dir.normalized, settings.RiverSectionLength, settings.RiverWidth));
            startPos = GetEdge(settings);
            dir = settings.Center - startPos;
        }
    }

    static void PerformRiverGenerationPass(Settlement_Settings settings,Settlement_River river)
    {
        int count = 0;
        Vector2 dir = Vector2.zero;
        int r = 0;
        Vector2 intersection = Vector2.zero;
        while (!RiverGenerated&&count<100)
        {
            
            for(int x = 0; x < SectionWorkingCopy.Count; x++)
            {

                if (CheckForIntersection(SectionWorkingCopy[x],Generated,out intersection))
                {
                    SectionWorkingCopy[x].UpdateEndPosition(intersection);
                    river.RiverSections.Add(SectionWorkingCopy[x]);

                }
                else
                {
                    dir = river.MeetingPoint-SectionWorkingCopy[x].EndPos;
                    dir = dir.normalized;
                    r = Random.Range(0, 100);
                    if (r < 50)
                    {
                        dir += new Vector2(Random.Range(-.1f, .1f) * settings.RiverBendScale, Random.Range(-.1f, .1f) * settings.RiverBendScale);
                    }

                    Settlement_RiverSection nextSection = new Settlement_RiverSection(SectionWorkingCopy[x].EndPos, dir, settings.RiverSectionLength, settings.RiverWidth);
                    Generated.Add(nextSection);
                    river.RiverSections.Add(SectionWorkingCopy[x]);
                }
             
            }
            SectionWorkingCopy.Clear();
            SectionWorkingCopy.AddRange(Generated);
            Generated.Clear();
            if (SectionWorkingCopy.Count == 0)
            {
                RiverGenerated = true;
            }
            count++;
        }
    }

    public static bool CheckForIntersection(Vector2 start,Vector2 end, List<Settlement_RiverSection> sectionsToCheck, out Vector2 intersection)
    {
        Vector2 currentIntersection = Vector2.zero;
        intersection = Vector2.zero;
        
        bool hasIntersected = false;
        float distFromStart = 999999f, distCheck = 0;
        for (int x = 0; x < sectionsToCheck.Count; x++)
        {
            if (sectionsToCheck[x].DoesLineIntersectRiverSection(start, end, out currentIntersection))
            {
                distCheck = Vector2.Distance(start, currentIntersection);
                if (distCheck < distFromStart)
                {
                    distFromStart = distCheck;
                    hasIntersected = true;
                    intersection = currentIntersection;
                }
            }
        }


        return hasIntersected;
    }

    static bool CheckForIntersection(Settlement_RiverSection section,List<Settlement_RiverSection> sectionsToCheck,out Vector2 intersection)
    {
        Vector2 currentIntersection = Vector2.zero;
        intersection = Vector2.zero;
        bool hasIntersected = false;
        float distFromStart = 999999f, distCheck = 0;
        for (int x=0;x< sectionsToCheck.Count; x++)
        {
            if (sectionsToCheck[x].DoesLineIntersectRiverSection(section.StartPos,section.EndPos,out currentIntersection))
            {
                distCheck = Vector2.Distance(section.StartPos,currentIntersection);
                if (distCheck < distFromStart)
                {
                    distFromStart = distCheck;
                    hasIntersected = true;
                    intersection = currentIntersection;
                }
            }
        }


        return hasIntersected;
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
public class Settlement_RiverSection
{
    public Vector2 StartPos, EndPos,Dir;
    public Vector2 PosSideStart, PosSideEnd;
    public Vector2 NegSideStart, NegSideEnd;
    public int Width, Length;

    public Settlement_RiverSection(Vector2 start,Vector2 dir,int length,int width)
    {
        dir = dir.normalized;
        Vector2 end = start + (dir * length);
        StartPos = start;
        EndPos = end;
        Dir = dir;
        Length = length;
        Width = width;
        Vector2 perp = Vector2.Perpendicular(end - start).normalized;
        PosSideStart = start + (perp * (width ));
        PosSideEnd = end + (perp * (width ));
        NegSideStart = start - (perp * (width ));
        NegSideEnd = end - (perp * (width ));
    }


    public void UpdateEndPosition(Vector2 newEnd)
    {
        EndPos = newEnd;
        Length = Mathf.RoundToInt( Vector2.Distance(StartPos, EndPos));
        Vector2 perp = Vector2.Perpendicular(EndPos - StartPos).normalized;
        PosSideStart = StartPos + (perp * (Width ));
        PosSideEnd = EndPos + (perp * (Width ));
        NegSideStart = StartPos - (perp * (Width));
        NegSideEnd = EndPos - (perp * (Width ));
    }


    public bool DoesLineIntersectRiverSection(Vector2 startPos,Vector2 endPos,out Vector2 intersection)
    {
        Vector2 finalIntersection = Vector2.zero;
        bool intersects = false;
        float distFromStart = 999999f,distCheck=0;
        if(LineUtil.IntersectLineSegments2D(startPos, endPos, StartPos, EndPos, out intersection))
        {
            distCheck = Vector2.Distance(startPos, intersection);
            if (distCheck < distFromStart)
            {
                intersects = true;
                finalIntersection = intersection;
                distFromStart = distCheck;
            }
        }
        if (LineUtil.IntersectLineSegments2D(startPos, endPos, PosSideStart, PosSideEnd, out intersection))
        {
            distCheck = Vector2.Distance(startPos, intersection);
            if (distCheck < distFromStart)
            {
                intersects = true;
                finalIntersection = intersection;
                distFromStart = distCheck;
            }
        }
        if (LineUtil.IntersectLineSegments2D(startPos, endPos, NegSideStart, NegSideEnd, out intersection))
        {
            distCheck = Vector2.Distance(startPos, intersection);
            if (distCheck < distFromStart)
            {
                intersects = true;
                finalIntersection = intersection;
                distFromStart = distCheck;
            }
        }

     
            intersection = finalIntersection;
            return intersects;
        

    }


}

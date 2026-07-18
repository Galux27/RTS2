using UnityEngine;

public class SettlementBuildingArea
{
    public Vector2 FarAlongRoad,PerpFromStart,Origin,PerpFromFarAlong;
   public SettlementBuildingArea(Vector2 farAlongRoad,Vector2 perpFromStart,Vector2 origin,Vector2 perpFromFarAlong)
    {
        FarAlongRoad= farAlongRoad;
        PerpFromStart= perpFromStart;
        Origin = origin;
        PerpFromFarAlong = perpFromFarAlong;
    }
    static Vector2 intersection;
    public bool DoesLineIntersect(Vector2 start,ref Vector2 end)
    {
        bool retVal = false;
        if (LineUtil.IntersectRoadSegments2D(start, end, Origin, FarAlongRoad, out intersection, 1f, 1f, false))
        {
            end = intersection;
            retVal = true;
        }
        
        if (LineUtil.IntersectRoadSegments2D(start, end, Origin,PerpFromStart, out intersection, 1f, 1f, false))
        {
            end= intersection;
            retVal = true;
        }
        
        if (LineUtil.IntersectRoadSegments2D(start, end, FarAlongRoad, PerpFromFarAlong, out intersection, 1f, 1f, false))
        {
            end = intersection;
            retVal = true;
        }

        if (LineUtil.IntersectRoadSegments2D(start, end, PerpFromStart, PerpFromFarAlong, out intersection, 1f, 1f, false))
        {
            end = intersection;
            retVal = true;
        }
        return retVal;
    }

}

public static class SettlementBuildingAreaHelpers
{
    const float OffsetFromRoad = 6f,MaxAreaSize=1000;
    public static SettlementBuildingArea GenerateBuildingArea(Settlement_Road road,GeneratedSettlement generatingIn,bool negative)
    {
        Vector2 validityStart = Vector2.Lerp(road.StartPos, road.EndPos, .5f);
        Vector2 validityPerp = road.Perp(negative)*10;
        Vector2 validityEnd = validityStart + validityPerp;
        
        if(CheckForIntersectionWithBuildingAreaRet(validityStart, ref validityEnd, generatingIn))
        {
            return null;
        }


        Vector2 startPos = road.StartPos;
        if (negative)
        {
            startPos += (road.Direction.normalized - Vector2.Perpendicular(road.Direction.normalized)) * OffsetFromRoad;

        }
        else
        {
            startPos += (road.Direction.normalized + Vector2.Perpendicular(road.Direction.normalized)) * OffsetFromRoad;

        }


        Vector2 endPoint = startPos + road.Direction.normalized * MaxAreaSize;
        SettlementGenerator.CheckForIntersectionAgainstSettlementEdge(startPos, ref endPoint);
        Vector2 intersection = Vector2.zero;

        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.roads, startPos, ref endPoint, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.highways, startPos, ref endPoint, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.avenues, startPos, ref endPoint, out intersection);

        Vector2 endPoint2 = startPos -( road.Direction.normalized * MaxAreaSize);
        SettlementGenerator.CheckForIntersectionAgainstSettlementEdge(startPos, ref endPoint2);

        CheckForIntersectionWithBuildingArea(startPos, ref endPoint2, generatingIn);

        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.roads, startPos, ref endPoint2, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.highways, startPos, ref endPoint2, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.avenues, startPos, ref endPoint2, out intersection); float dist1 = Vector2.Distance(endPoint, startPos);
        float dist2 = Vector2.Distance(endPoint2,startPos);
        if (dist2 > dist1)
        {
            endPoint = endPoint2;
            dist1 = dist2;
        }
        if (dist2 > MaxAreaSize - 10)
        {
            return null;
        }

        Vector2 perp = Vector2.Perpendicular(road.Direction.normalized);

        Vector2 perpEndPoint = startPos + (perp * MaxAreaSize);
        SettlementGenerator.CheckForIntersectionAgainstSettlementEdge(startPos, ref perpEndPoint);

        CheckForIntersectionWithBuildingArea(startPos, ref perpEndPoint, generatingIn);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.roads, startPos, ref perpEndPoint, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.highways, startPos, ref perpEndPoint, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.avenues, startPos, ref perpEndPoint, out intersection);

        Vector2 perpEndPoint2 = startPos - (perp * MaxAreaSize);
        SettlementGenerator.CheckForIntersectionAgainstSettlementEdge(startPos, ref perpEndPoint2);

        CheckForIntersectionWithBuildingArea(startPos, ref perpEndPoint2, generatingIn);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.roads, startPos, ref perpEndPoint2, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.highways, startPos, ref perpEndPoint2, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.avenues, startPos, ref perpEndPoint2, out intersection);
        float perpDist1 = Vector2.Distance(perpEndPoint, startPos);
        float perpDist2 = Vector2.Distance(perpEndPoint2, startPos);
       
        if (perpDist2 > perpDist1)
        {
            perpEndPoint = perpEndPoint2;
            perpDist1=perpDist2;
            perp *= -1f;
        }

        if (perpDist1 > MaxAreaSize - 10)
        {
            return null;
        }

        Vector2 perpEndPoint3 = endPoint + (perp * MaxAreaSize);
        SettlementGenerator.CheckForIntersectionAgainstSettlementEdge(endPoint, ref perpEndPoint3);

        CheckForIntersectionWithBuildingArea(endPoint, ref perpEndPoint3, generatingIn);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.roads, endPoint, ref perpEndPoint3, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.highways, endPoint, ref perpEndPoint3, out intersection);
        SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.avenues, endPoint, ref perpEndPoint3, out intersection);

        if (Vector2.Distance(endPoint, perpEndPoint3) < Vector2.Distance(startPos, perpEndPoint))
        {
            Vector2 newEndPoint = LineUtil.FindNearestPointOnLine(startPos, perpEndPoint - startPos, perpEndPoint3);
            Vector2 originalNewEnd = newEndPoint;
            SettlementGenerator.CheckForIntersectionAgainstSettlementEdge(perpEndPoint3, ref newEndPoint);

            CheckForIntersectionWithBuildingArea(perpEndPoint3, ref newEndPoint, generatingIn);
            SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.roads, perpEndPoint3, ref newEndPoint, out intersection);
            SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.highways, perpEndPoint3, ref newEndPoint, out intersection);
            SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.avenues, perpEndPoint3, ref newEndPoint, out intersection);
            if (Vector2.Distance(originalNewEnd, newEndPoint) > .5f)
            {
                return null;
            }
            else
            {
                return new SettlementBuildingArea(endPoint, newEndPoint, startPos, perpEndPoint3);

            }
        }
        else
        {
            Vector2 newEndPoint = LineUtil.FindNearestPointOnLine(endPoint, perpEndPoint3 - endPoint, perpEndPoint);
            Vector2 originalNewEnd = newEndPoint;
            SettlementGenerator.CheckForIntersectionAgainstSettlementEdge(perpEndPoint3, ref newEndPoint);

            CheckForIntersectionWithBuildingArea(perpEndPoint, ref newEndPoint, generatingIn);
            SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.roads, perpEndPoint, ref newEndPoint, out intersection);
            SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.highways, perpEndPoint, ref newEndPoint, out intersection);
            SettlementGenerator.CheckForIntersectionAgainstAll(generatingIn.avenues, perpEndPoint, ref newEndPoint, out intersection);
            if (Vector2.Distance(originalNewEnd, newEndPoint) > .5f)
            {
                return null;
            }
            else
            {
                return new SettlementBuildingArea(endPoint, perpEndPoint, startPos, newEndPoint);

            }
        }


        //add a check for the edge of the settlement - DONE
        //make second perp line from the end of the line that follows the road,
        //check if the end point of the shorter of the two lines can reach the longer
        //if so create the area using that intersection as the final point
        //else ababdon the area
        return new SettlementBuildingArea(endPoint, perpEndPoint, startPos,perpEndPoint3);
    }

    static void CheckForIntersectionWithBuildingArea(Vector2 start,ref Vector2 end,GeneratedSettlement settlement)
    {
        for(int x = 0; x < settlement.BuildingAreas.Count; x++)
        {
            settlement.BuildingAreas[x].DoesLineIntersect(start,ref end);
        }
    }
    static bool CheckForIntersectionWithBuildingAreaRet(Vector2 start, ref Vector2 end, GeneratedSettlement settlement)
    {
        for (int x = 0; x < settlement.BuildingAreas.Count; x++)
        {
            if(settlement.BuildingAreas[x].DoesLineIntersect(start, ref end))
            {
                return true;
            }
        }
        return false;
    }
}
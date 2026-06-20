using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class Settlement_Road 
{
    public Vector2 StartPos, Direction;
    public float Length;
    public Vector2 endPos;
    public bool EndedByLink = false;
    public Color debugColor;
    public List<float> PointsToSplitAt = new List<float>();
    public Settlement_Road(Vector2 start,Vector2 dir,float len)
    {
        StartPos= start;
        Direction= dir.normalized;
        Length= len;
        EndPos = StartPos + (Direction * len);
        debugColor=new Color(Random.value,Random.value,Random.value,1f);
    }

    public void UpdateEndPosition(Vector2 pos)
    {
        Length=Vector2.Distance(StartPos, pos);
        EndPos = pos;
    }


    public Vector2 EndPos
    {
        get
        {
            return endPos;
        }
        set
        {
            endPos = value;
        }
    }

    public bool IsPointCloseToPositions(Vector2 pos,float dist)
    {
        return Vector2.Distance(StartPos,pos) > dist && Vector2.Distance(pos,EndPos)>dist;
    }

    public Vector2 Perp(bool negative)
    {
        if (negative)
        {
            return Vector2.Perpendicular(endPos - StartPos)*-1;
        }
        else
        {
            return Vector2.Perpendicular(endPos - StartPos);
        }
    }

   
    public void AddPointToSplit(float point)
    {
        PointsToSplitAt.Add(point);
    }
    public Vector2 GetPositionOnRoad(float pos)
    {
        return Vector2.Lerp(StartPos, EndPos, pos);
    }

    public Settlement_Road[] SplitRoad(float pointToSplit)
    {
        Settlement_Road[] retVal = new Settlement_Road[2];
        Vector2 end = Vector2.Lerp(StartPos,EndPos,pointToSplit);
        Vector2 dir = end - StartPos;
        retVal[0] = new Settlement_Road(StartPos, dir.normalized, dir.magnitude);
        dir = EndPos - end;
        retVal[1]=new Settlement_Road(end, dir.normalized, dir.magnitude);
        return retVal;
    }


}

public static class LineUtil
{
    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public static bool Approximately(float a, float b, float tolerance = 1e-5f)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }

    public static float CrossProduct2D(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }


    public static bool IntersectRoadSegments2D(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end, out Vector2 intersection, float p1Width = 5f, float p2Width = 5f, bool checkWidth = false)
    {
        bool hit = false;
        hit = IntersectLineSegments2D(p1start, p1end, p2start, p2end, out intersection);
        if (checkWidth)
        {
            if (!hit)
            {
                Vector2 lStart = Vector2.zero, lEnd = Vector2.zero;
                GetLeftSideOfLine(p2start, p2end, p2Width, out lStart, out lEnd);
                hit = IntersectLineSegments2D(p1start, p1end, lStart, lEnd, out intersection);
                if (!hit)
                {
                    Vector2 rStart = Vector2.zero, rEnd = Vector2.zero;
                    GetRightSideOfLine(p2start, p2end, p2Width, out rStart, out rEnd);
                    hit = IntersectLineSegments2D(p1start, p1end, rStart, rEnd, out intersection);
                }
            }
            else
            {
                bool hitLeft = false;
                Vector2 leftIntersection = Vector2.zero;
                Vector2 lStart = Vector2.zero, lEnd = Vector2.zero;
                GetLeftSideOfLine(p2start, p2end, p2Width, out lStart, out lEnd);
                hitLeft = IntersectLineSegments2D(p1start, p1end, lStart, lEnd, out leftIntersection);
                if (hitLeft)
                {
                    if (Vector2.Distance(p1start, leftIntersection) < Vector2.Distance(p1start, intersection))
                    {
                        intersection = leftIntersection;
                    }
                }

                bool hitRight = false;
                Vector2 rightIntersection = Vector2.zero;
                Vector2 rStart = Vector2.zero, rEnd = Vector2.zero;
                GetRightSideOfLine(p2start, p2end, p2Width, out rStart, out rEnd);
                hitRight = IntersectLineSegments2D(p1start, p1end, rStart, rEnd, out rightIntersection);
                if (hitRight)
                {
                    if (Vector2.Distance(p1start, rightIntersection) < Vector2.Distance(p1start, intersection))
                    {
                        intersection = rightIntersection;
                    }
                }
            }
        }

        return hit;
    }

    /// <summary>
    /// Determine whether 2 lines intersect, and give the intersection point if so.
    /// </summary>
    /// <param name="p1start">Start point of the first line</param>
    /// <param name="p1end">End point of the first line</param>
    /// <param name="p2start">Start point of the second line</param>
    /// <param name="p2end">End point of the second line</param>
    /// <param name="intersection">If there is an intersection, this will be populated with the point</param>
    /// <returns>True if the lines intersect, false otherwise.</returns>
    public static bool IntersectLineSegments2D(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end,
        out Vector2 intersection)
    {
        // Consider:
        //   p1start = p
        //   p1end = p + r
        //   p2start = q
        //   p2end = q + s
        // We want to find the intersection point where :
        //  p + t*r == q + u*s
        // So we need to solve for t and u
        var p = p1start;
        var r = p1end - p1start;
        var q = p2start;
        var s = p2end - p2start;
        var qminusp = q - p;

        float cross_rs = CrossProduct2D(r, s);

        if (Approximately(cross_rs, 0.25f))
        {
            // Parallel lines
            if (Approximately(CrossProduct2D(qminusp, r), 0f))
            {
                // Co-linear lines, could overlap
                float rdotr = Vector2.Dot(r, r);
                float sdotr = Vector2.Dot(s, r);
                // this means lines are co-linear
                // they may or may not be overlapping
                float t0 = Vector2.Dot(qminusp, r / rdotr);
                float t1 = t0 + sdotr / rdotr;
                if (sdotr < 0)
                {
                    // lines were facing in different directions so t1 > t0, swap to simplify check
                    Swap(ref t0, ref t1);
                }

                if (t0 <= 1 && t1 >= 0)
                {
                    // Nice half-way point intersection
                    float t = Mathf.Lerp(Mathf.Max(0, t0), Mathf.Min(1, t1), 0.5f);
                    intersection = p + t * r;
                    return true;
                }
                else
                {
                    // Co-linear but disjoint
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else
            {
                // Just parallel in different places, cannot intersect
                intersection = Vector2.zero;
                return false;
            }
        }
        else
        {
            // Not parallel, calculate t and u
            float t = CrossProduct2D(qminusp, s) / cross_rs;
            float u = CrossProduct2D(qminusp, r) / cross_rs;
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                intersection = p + t * r;
                return true;
            }
            else
            {
                // Lines only cross outside segment range
                intersection = Vector2.zero;
                return false;
            }
        }
    }

    static void GetRightSideOfLine(Vector2 start, Vector2 end, float width, out Vector2 newStart, out Vector2 newEnd)
    {
        Vector2 dir = end - start;
        dir = dir.normalized;
        Vector2 perp = Vector2.Perpendicular(dir).normalized;
        newStart = start - (perp * (width / 2f));
        newEnd = end - (perp * (width / 2f));
    }
    static void GetLeftSideOfLine(Vector2 start, Vector2 end, float width, out Vector2 newStart, out Vector2 newEnd)
    {
        Vector2 dir = end - start;
        dir = dir.normalized;
        Vector2 perp = Vector2.Perpendicular(dir).normalized;
        newStart = start + (perp * (width / 2f));
        newEnd = end + (perp * (width / 2f));
    }

    /// <summary>
    /// Finds the nearest point on line made of a start and a direction.
    /// </summary>
    /// <returns>The nearest point on line.</returns>
    /// <param name="origin">Origin.</param>
    /// <param name="direction">Direction.</param>
    /// <param name="point">Point.</param>
    public static Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
    {
        direction.Normalize();
        Vector2 lhs = point - origin;

        float dotP = Vector2.Dot(lhs, direction);
        return origin + direction * dotP;
    }

    /// <summary>
    /// Finds the nearest point on a line with defined start and end points.
    /// </summary>
    /// <returns>The nearest point on set line.</returns>
    /// <param name="origin">Origin.</param>
    /// <param name="end">End.</param>
    /// <param name="point">Point.</param>
    public static Vector2 FindNearestPointOnSetLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        //Get heading
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }


}

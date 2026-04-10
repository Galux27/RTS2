using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Settlement : OverworldFeatureToWorldConverter
{

    const int MaxRoads = 25;
    const int MinBuildingArea = 25* 25;

    //make it so top & right edges have the size edited on the split and not the road generation
    //have building sections check for intersections with roads and divide them based on that
    //split starting area manually based on starting roads
    //add summit to stop minor road connections being generated if a major one exists in the same direction
    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        List<RoadData> roadsAtStart = new List<RoadData>();
        roadsAtStart.AddRange(toGenerateIn.Roads);
        List<Bounds> ExistingBounds = new List<Bounds>();
        List<ForcedSplit> forcedSplits = new List<ForcedSplit>();
        for(int x = 0; x < roadsAtStart.Count; x++)
        {
            Bounds b = new Bounds(Vector2.Lerp(roadsAtStart[x].StartPos, roadsAtStart[x].EndPos, .5f),Vector3.one);
            b.Encapsulate(new Vector3(roadsAtStart[x].LeftStart.x, roadsAtStart[x].LeftEnd.y,0));
            b.Encapsulate(new Vector3(roadsAtStart[x].RightEnd.x, roadsAtStart[x].RightEnd.y, 0));
            b.Encapsulate(new Vector3(roadsAtStart[x].StartPos.x, roadsAtStart[x].StartPos.y, 0));
            b.Encapsulate(new Vector3(roadsAtStart[x].EndPos.x, roadsAtStart[x].EndPos.y, 0));

            DrawBounds(b, Color.magenta, 99f);

            if (roadsAtStart[x].StartPos.x == roadsAtStart[x].EndPos.x)
            {
                ForcedSplit leftSplit = new ForcedSplit();

                leftSplit.axisToSplit = Axis.Horizontal;
                leftSplit.Position = roadsAtStart[x].LeftStart;

                ForcedSplit rightSplit = new ForcedSplit();

                rightSplit.axisToSplit = Axis.Horizontal;
                rightSplit.Position = roadsAtStart[x].RightStart;

                forcedSplits.Add(leftSplit); forcedSplits.Add(rightSplit);
            }
            else
            {
                ForcedSplit leftSplit = new ForcedSplit();

                leftSplit.axisToSplit = Axis.Vertical;
                leftSplit.Position = roadsAtStart[x].LeftStart;

                ForcedSplit rightSplit = new ForcedSplit();

                rightSplit.axisToSplit = Axis.Vertical;
                rightSplit.Position = roadsAtStart[x].RightStart;

                forcedSplits.Add(leftSplit); forcedSplits.Add(rightSplit);
            }

            ExistingBounds.Add(b);
        }




        //create splits that make existing roads into their own chunks that can't be split
        Debug.Log("Roads at start " + toGenerateIn.Roads.Count+" at " + toGenerateIn.coords);
        List<SettlementArea> areas = new List<SettlementArea>();
        areas.Add(new SettlementArea(toGenerateIn.coords, new Vector2(WorldChunkManager.ChunkBatchSize-4, WorldChunkManager.ChunkBatchSize - 4),toGenerateIn.coords));
        for(int x = 0; x < forcedSplits.Count; x++)
        {
            int ix = 0;
            while (ix < areas.Count)
            {
                if (areas[ix].CanSplitOnManualSplit(forcedSplits[x]))
                {
                    areas.AddRange(areas[ix].SplitManually(forcedSplits[x]));
                    areas.Remove(areas[ix]);
                }
                else
                {
                    ix++;
                }
            }
        }
        
        
        int count = 0;
        int index = 0;
        SettlementArea[] split = null;
        while (count < 100)
        {
            index = Random.Range(0, areas.Count);
            split = areas[index].Split();
            bool valid = true;
            for(int x=0;x< split.Length; x++)
            {
                if (!IsSplitValid(split[x])) {
                    valid = false;
                    break;
                }

            }
            if (valid)
            {
                areas.RemoveAt(index);
                areas.Add(split[0]);
                areas.Add(split[1]);
            }
            count++;
        }
        List<RoadData> data = new List<RoadData>();
        for(int x = 0; x < areas.Count; x++)
        {
            areas[x].CreateRoadsFromSplit(ref data);
        }
        for(int x = 0; x < data.Count; x++)
        {
            bool valid = true;
            for(int q = 0; q < ExistingBounds.Count; q++)
            {
                if (ExistingBounds[q].Contains( Vec2IntToVec(data[x].StartPos)) && ExistingBounds[q].Contains(Vec2IntToVec(data[x].EndPos)))
                {
                    valid = false; 
                    break;
                }
            }
            
            if (valid)
            {
                toGenerateIn.AddRoad(data[x]);
            }
        }
        List<SettlementArea> zonesToSplitOnRoad = new List<SettlementArea>();
        List<List<Vector2>> splitPositions = new List<List<Vector2>>();
        List<Vector2> splits = new List<Vector2>();
        for(int x = 0; x < areas.Count; x++)
        {
            bool valid = true;
            List<Vector2> allSplits = new List<Vector2>();
            for(int y=0;y< roadsAtStart.Count; y++)
            {
                splits = areas[x].buildingZone.IntersectsRoad(roadsAtStart[y]);
                if (splits.Count>0)
                {
                    allSplits.AddRange(splits);
                    valid = false;
                    break;
                }
            }
            if (valid==false)
            {
                zonesToSplitOnRoad.Add(areas[x]);
                splitPositions.Add(allSplits);
            }
            splits = new List<Vector2>();
            if (valid && AreBoundsValid(areas[x].buildingZone.GetBounds()))
            {
                areas[x].buildingZone.GenerateBuildingZones(BuildingDataManager.Instance.BuildingTemplates["House"]);
                if (areas[x].buildingZone.Populated)
                {
                    toGenerateIn.Zones.Add(areas[x].buildingZone);
                }
                for(int o = 0; o < areas[x].buildingZone.Buildings.Count; o++)
                {
                    DrawBounds(areas[x].buildingZone.Buildings[o].GetBounds(), Color.red, 99f);
                }
            }
            else
            {
                DrawBounds(areas[x].buildingZone.GetBounds(), Color.yellow, 99f);

            }

        }

        if (toGenerateIn.Zones.Count > 0)
        {
            BuildingPlacementController.Instance.BatchesWithBuildings.Add(toGenerateIn);
        }

        Debug.Log("Generated settlement, final road count " + toGenerateIn.Roads.Count+" in " + toGenerateIn.coords);
    }



    void DrawBounds(Bounds b,Color c, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, c, delay);
        Debug.DrawLine(p2, p3, c, delay);
        Debug.DrawLine(p3, p4,c, delay);
        Debug.DrawLine(p4, p1, c, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, c, delay);
        Debug.DrawLine(p6, p7, c, delay);
        Debug.DrawLine(p7, p8, c, delay);
        Debug.DrawLine(p8, p5, c, delay);

        // sides
        Debug.DrawLine(p1, p5, c, delay);
        Debug.DrawLine(p2, p6, c, delay);
        Debug.DrawLine(p3, p7, c, delay);
        Debug.DrawLine(p4, p8, c, delay);
    }
    Vector2 Vec2IntToVec(Vector2Int val)
    {
        return new Vector2(val.x, val.y);
    }

    bool AreBoundsValid(Bounds b)
    {
        if (b.size.x * b.size.y >= MinBuildingArea && b.size.x > 6 && b.size.y > 6)
        {
            return true;
        }
        return false;
    }

    bool IsBuildingZoneValid(BuildingZone zone)
    {
        Debug.Log("BZ: Checking building zone size " + zone.Position + "," + zone.Size);
        if (zone.Size.x * zone.Size.y >= MinBuildingArea && zone.Size.x > 6 && zone.Size.y > 6)
        {
            return true;
        }
        return false;
    }

    bool IsSplitValid(SettlementArea area)
    {
        Debug.Log("Set: Checking building zone size " + (area.position+area.parentChunkBatch)+ "," + area.size);
        return AreBoundsValid(area.buildingZone.GetBounds());
        if (area.size.x * area.size.y >= MinBuildingArea &&area.size.x>6&&area.size.y>6)
        {
            return true;
        }
        return false;
    }


    public override OverworldFeature GetFeatureIGenerate()
    {
        return OverworldFeature.Settlement;
    }
    
  

    static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num, offset;
        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x; x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x; x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p2.y; y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y; y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//

        // alpha tests//
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }

        e = Ax * Cy - Ay * Cx;  // beta numerator//

        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }

        // check if they are parallel
        if (f == 0) return false;

        // compute intersection coordinates //
        num = d * Ax;   // numerator //
        offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;  // round direction //
        intersection.x = p1.x + (num + offset) / f;

        num = d * Ay;
        offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;
        intersection.y = p1.y + (num + offset) / f;

        return true;
    }

    private static bool same_sign(float a, float b)
    {
        return ((a * b) >= 0f);
    }
}

//break up areas into random grids then make roads and buildings based off this (only use right and top edges for roads
public class SettlementArea
{
    public Vector2 position, size;
    public Vector2Int parentChunkBatch;
    public BuildingZone buildingZone;
    public SettlementArea(Vector2 pos, Vector2 s, Vector2Int parent)
    {
        this.position = pos;
        this.size = s;
        parentChunkBatch = parent;

        buildingZone = new BuildingZone(
           new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)),
           new Vector2Int(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y)), 7);
    }

    public void CreateRoadsFromSplit(ref List<RoadData> toAddTo)
    {
        Vector2Int start = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y + size.y));
        Vector2Int start2 = new Vector2Int(Mathf.RoundToInt(position.x+size.x), Mathf.RoundToInt(position.y));
        Vector2Int end = new Vector2Int(start2.x, start.y);
        toAddTo.Add(new RoadData(start, end+Vector2Int.right, 7, RoadType.MinorRoad));
        toAddTo.Add(new RoadData(start2,end+Vector2Int.up,7, RoadType.MinorRoad));
        buildingZone = new BuildingZone(
            new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)),
            new Vector2Int(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y)), 7);
    }

    public bool CanSplitOnManualSplit(ForcedSplit toUse)
    {
        if (toUse.axisToSplit == Axis.Horizontal)
        {
            return toUse.Position.x > position.x && toUse.Position.x < (position.x + size.x);
        }
        else
        {
            return toUse.Position.y > position.y && toUse.Position.y < (position.y + size.y);
        }
    }


    public SettlementArea[] SplitManually(ForcedSplit toUse)
    {
        SettlementArea[] retVal = new SettlementArea[2];
        if (toUse.axisToSplit == Axis.Horizontal)
        {
            float x = toUse.Position.x;
            float firstSize = x - position.x;
            float secondSize = (position.x + size.x) - x;
            retVal[0] = new SettlementArea(position, new Vector2(firstSize, size.y), parentChunkBatch);
            retVal[1] = new SettlementArea(new Vector2(x, position.y), new Vector2(secondSize, size.y), parentChunkBatch);
        }
        else
        {
            float y = toUse.Position.y;
            float firstSize = y - position.y;
            float secondSize = (position.y + size.y) - y;
            retVal[0] = new SettlementArea(position, new Vector2(size.x, firstSize), parentChunkBatch);
            retVal[1] = new SettlementArea(new Vector2(position.x, y), new Vector2(size.x, secondSize), parentChunkBatch);
        }
        return retVal;
    }

    public SettlementArea[] Split()
    {
        SettlementArea[] retVal = new SettlementArea[2];
        int r = Random.Range(0, 100);
        if (r < 50)
        {
            float x = Random.Range(position.x, position.x + size.x);
            float firstSize = x - position.x;
            float secondSize = (position.x + size.x) - x;
            retVal[0] = new SettlementArea(position, new Vector2(firstSize, size.y), parentChunkBatch);
            retVal[1]=new SettlementArea(new Vector2(x,position.y),new Vector2(secondSize, size.y), parentChunkBatch);

        }
        else
        {
            float y = Random.Range(position.y, position.y + size.y);
            float firstSize = y - position.y;
            float secondSize = (position.y + size.y) - y;
            retVal[0] = new SettlementArea(position, new Vector2(size.x,firstSize), parentChunkBatch);
            retVal[1] = new SettlementArea(new Vector2(position.x,y), new Vector2(size.x,secondSize), parentChunkBatch);
        }

        return retVal;
    }

    public List<SettlementArea> SplitOnRoadIntersection(List<Vector2> points)
    {
        List<SettlementArea> retVal = new List<SettlementArea>();
        Vector2 max = position + size;

        Vector2 MinSplit = max;
        Vector2 MaxSplit = position;
        string allSplits = "";
        for(int x = 0; x < points.Count; x++)
        {
            allSplits += points[x].ToSafeString() + ",";
            if (points[x].x < MinSplit.x)
            {
                MinSplit.x = points[x].x;
            }
            if (points[x].x >MaxSplit.x)
            {
                MaxSplit.x = points[x].x;
            }
            if (points[x].y < MinSplit.y)
            {
                MinSplit.y = points[x].y;
            }
            if (points[x].y > MaxSplit.y)
            {
                MaxSplit.y = points[x].y;
            }
        }
        Vector2 size1 = new Vector2(Mathf.Abs(MinSplit.x - position.x), Mathf.Abs(MinSplit.y - position.y));
        Vector2 size2 = new Vector2(Mathf.Abs(max.x - MaxSplit.x), Mathf.Abs(max.y - MaxSplit.y));
        Debug.Log("Split: " + position + "," + max + "," + MinSplit + "," + MaxSplit+","+size1+","+size2+" all "+ allSplits);

        retVal.Add(new SettlementArea(position,size1, parentChunkBatch));
        retVal.Add(new SettlementArea(MaxSplit,size2, parentChunkBatch));


        return retVal;
        

        
    }

}
public class BuildingZone
{
    public Vector2Int Position, Size;
    
    public int RoadWidth;
    public bool Populated = false, Generated = false;
    public List<BuildingZoneBuilding> Buildings = new List<BuildingZoneBuilding>();
    Vector2 p1;
    Vector2 p2;
    Vector2 p3;
    Vector2 p4;
    public BuildingZone (Vector2Int pos,Vector2Int size,int width)
    {
        Position = pos;
        RoadWidth = width;
        Size = size;
        size.x -= RoadWidth;
        size.y -=RoadWidth;
        p1 = Position;
        p2 = Position + new Vector2Int(Size.x, 0);
        p3 = Position + new Vector2Int(0, Size.y);
        p4 = Position + Size;
    }
    List<Vector2> Intersections = new List<Vector2>();

    public List<Vector2> IntersectsRoad(RoadData data)
    {
        Intersections.Clear();

        Intersections.AddRange(CheckIntersections(p1, p2, data));
        Intersections.AddRange(CheckIntersections(p1, p3, data));
        Intersections.AddRange(CheckIntersections(p3, p4, data));
        Intersections.AddRange(CheckIntersections(p2, p4, data));

        string debug = "Intersections for " + p1+","+Size+" ("+Intersections.Count+")";
        for(int x = 0; x < Intersections.Count; x++)
        {
            debug += Intersections[x].ToSafeString() + ",";
        }

        Debug.Log(debug);
        return Intersections;
    }
    List<Vector2> retVal = new List<Vector2>();

    List<Vector2> CheckIntersections(Vector2 p1,Vector2 p2,RoadData data)
    {
        retVal.Clear();
        Vector2 pos = Vector2.zero;
        //if (data.IntersectsLeftEdge(p1, p2, ref pos))
        //{
        //    retVal.Add(pos);
        //}
        if (data.IntersectsCenterLine(p1, p2, ref pos))
        {
            retVal.Add(pos);
        }
        //if (data.IntersectsRightEdge(p1, p2, ref pos))
        //{
        //    retVal.Add(pos);
        //}
       
        return retVal;
    }

    public Bounds GetBounds()
    {
        return new Bounds(new Vector3(Position.x,Position.y)+ new Vector3(Size.x*.5f, Size.y *.5f, 0), new Vector3(Size.x-RoadWidth,Size.y-RoadWidth,0));
    }

    public void GenerateBuildingZones(BuildingTemplate template)
    {
        if (Size.x < template.MinWidth || Size.y < template.MinHeight)
        {
            Debug.Log("Building Zones: height invalid, returning");

            return;
        }
        int Width= Random.Range(template.MinWidth, template.MaxHeight);
        int Height = Random.Range(template.MinHeight, template.MaxHeight);

        int WidthDivisions = Mathf.Max(1, Mathf.FloorToInt( Size.x/Width)-1);
        int HeightDivisions = Mathf.Max(1, Mathf.FloorToInt(Size.y/Height)-1);
        if (WidthDivisions == 0 || HeightDivisions == 0)
        {
            bool xvalid = false,yvalid=false;
            while (!xvalid&&!yvalid)
            {
                if (HeightDivisions == 0)
                {
                    if (Height < template.MinHeight)
                    {
                        yvalid = true;
                    }
                    else
                    {
                        Height--;
                        HeightDivisions = Mathf.Max(1, Mathf.FloorToInt(Size.y / Height) - 1);
                    }
                }
                else
                {
                    yvalid = true;
                }

                if (WidthDivisions == 0)
                {
                    if (Width < template.MinWidth)
                    {
                        xvalid = true;
                    }
                    else
                    {
                        Width--;
                        WidthDivisions = Mathf.Max(1, Mathf.FloorToInt(Size.x / Width) - 1);
                    }
                }
                else
                {
                    xvalid = true;
                }
            }
            
        }

        if (WidthDivisions == 0 || HeightDivisions == 0)
        {
            return;
        }
            float widthremainder = Width % Size.x;
        float heightremainder = Height % Size.y;
        Debug.Log("Building Zones: total divisions made " + WidthDivisions + "," + HeightDivisions+","+widthremainder+","+heightremainder);
        Vector2Int pos = Vector2Int.zero ;
        Vector2Int StartOffset = new Vector2Int(Mathf.RoundToInt( widthremainder / 2),Mathf.RoundToInt( heightremainder / 2));
        for(int x = 0; x < WidthDivisions; x++)
        {
            for(int y = 0; y < HeightDivisions; y++)
            {
                pos= StartOffset+Position + new Vector2Int((x*Width)+x, (y*Height)+y);
                BuildingZoneBuilding bz = new BuildingZoneBuilding(pos, new Vector2Int(Width, Height), template.BuildingName);
                if (BoundsContainsBoundsEntirely(GetBounds(),bz.GetBounds()))
                {
                    Buildings.Add(bz);
                }
            }
        }
        Populated = true;
    }

    bool BoundsContainsBoundsEntirely(Bounds checking,Bounds toCheck)
    {
        if(checking.Contains(toCheck.center)&&checking.Contains(toCheck.max)&&checking.Contains(toCheck.min)
            &&checking.Contains(toCheck.center+new Vector3(toCheck.extents.x,toCheck.extents.y*-1,0))
            && checking.Contains(toCheck.center + new Vector3(toCheck.extents.x * -1, toCheck.extents.y , 0)))
        {
            return true;
        }

        return false;
    }
}

public class BuildingZoneBuilding
{
    public bool Drawn = false;
    public Vector2Int Position, Size;
    public string Template = "";
    public BuildingZoneBuilding(Vector2Int p,Vector2Int s,string template)
    {
        Position = p;
        Size = s;
        Template = template;
    }

    public Bounds GetBounds()
    {
        return new Bounds(new Vector3(Position.x+(Size.x/2),Position.y+(Size.y/2)), new Vector3(Size.x,Size.y));
    }
}

public class ForcedSplit
{
    public Vector2Int Position;
    public Axis axisToSplit;
}
public enum Axis
{
    Horizontal,
    Vertical
}

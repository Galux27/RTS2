using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SettlementMK2 : OverworldFeatureToWorldConverter
{

    const int MaxRoads = 25;
    const int MinBuildingArea = 12 * 12;

    //make it so top & right edges have the size edited on the split and not the road generation
    //have building sections check for intersections with roads and divide them based on that
    //split starting area manually based on starting roads
    //add summit to stop minor road connections being generated if a major one exists in the same direction
    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        OverworldTile tile = OverworldGenerator.Instance.OverworldTiles[toGenerateIn.OverworldCoords.x, toGenerateIn.OverworldCoords.y];
       
        List<OverworldTile> AdjacentTiles = OverworldGenerator.Instance.GetNeighbours(toGenerateIn.OverworldCoords);
        OverworldSettlementDetails details = tile.SettlementDetails; 

        if (details == null)
        {
            for(int x = 0; x < AdjacentTiles.Count; x++)
            {
                if (AdjacentTiles[x].SettlementDetails != null)
                {
                    details = AdjacentTiles[x].SettlementDetails;
                }
            }
            if (details == null)
            {
                Debug.LogError("Abandoning settlement gen in " + toGenerateIn.coords + " due to no details");
                return;
            }
        }
        OverworldGenerator.Instance.GetOverworldStartingCoords();
        Vector2Int worldCenter = Vector2Int.zero ;
        OverworldSettlement toGenerate= OverworldGenerator.Instance.GetSettlementByID(details.ID);
        if (toGenerate.GeneratedInstance == null)
        {
            toGenerate.GenerateSettlement();
        }
        Debug.LogError("Generated settlement at " + toGenerateIn.OverworldCoords.ToString()+","+OverworldGenerator.Instance.GetOverworldStartingCoords()+","+toGenerateIn.coords);
        GeneratedSettlementArea area = toGenerate.GeneratedInstance.AreaFromCoords(toGenerateIn.coords);//GetAreaFromOverworld(toGenerateIn.OverworldCoords,toGenerateIn.coords);
        if (area == null)
        {
            return;
        }
        
        List<RoadData> roads = new List<RoadData>();
       
       
        
        RoadDetails road = RoadTypeManager.Instance.AllRoadDetails[RoadType.MajorRoad.ToString()];

        List<Settlement_Road> roadsToAdd = area.highways;//toGenerate.GeneratedInstance.GetRoadsInWorldBatch(toGenerateIn, toGenerate.GeneratedInstance.highways);
        
        for(int x = 0; x < roadsToAdd.Count; x++)
        {
            toGenerateIn.AddRoad(new RoadData(roadsToAdd[x].StartPos, roadsToAdd[x].endPos, road.RoadWidth, RoadType.MajorRoad));
        }
        roadsToAdd = area.avenues;//toGenerate.GeneratedInstance.GetRoadsInWorldBatch(toGenerateIn, toGenerate.GeneratedInstance.avenues);
        road = RoadTypeManager.Instance.AllRoadDetails[RoadType.MinorRoad.ToString()];
        for (int x = 0; x < roadsToAdd.Count; x++)
        {
            toGenerateIn.AddRoad(new RoadData(roadsToAdd[x].StartPos, roadsToAdd[x].endPos, road.RoadWidth, RoadType.MinorRoad));
        }
        roadsToAdd = area.roads;//toGenerate.GeneratedInstance.GetRoadsInWorldBatch(toGenerateIn, toGenerate.GeneratedInstance.roads);

        road = RoadTypeManager.Instance.AllRoadDetails[RoadType.Backroad.ToString()];
        for (int x = 0; x < roadsToAdd.Count; x++)
        {
            toGenerateIn.AddRoad(new RoadData(roadsToAdd[x].StartPos, roadsToAdd[x].endPos, road.RoadWidth, RoadType.Backroad));
        }

        toGenerateIn.SetBuildings(area.Buildings);
        Debug.LogError("Total Roads Found " + toGenerateIn.Roads.Count);
        //toGenerateIn.Roads = roads;
        //if (toGenerateIn.Zones.Count > 0)
        //{
        //    BuildingPlacementController.Instance.BatchesWithBuildings.Add(toGenerateIn);
        //}

    }



    void DrawBounds(Bounds b, Color c, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, c, delay);
        Debug.DrawLine(p2, p3, c, delay);
        Debug.DrawLine(p3, p4, c, delay);
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

    bool AreBoundsValid(Bounds b, WorldChunkBatch toGenerateIn)
    {
        for (int x = 0; x < toGenerateIn.Rivers.Count; x++)
        {
            if (toGenerateIn.Rivers[x].MyBounds.Contains(b.center))
            {
                return false;
            }
        }

        if (b.size.x * b.size.y >= MinBuildingArea && b.size.x > 6 && b.size.y > 6)
        {
            return true;
        }



        return false;
    }

    bool IsBuildingZoneValid(BuildingZone zone)
    {
        if (zone.Size.x * zone.Size.y >= MinBuildingArea && zone.Size.x > 6 && zone.Size.y > 6)
        {
            return true;
        }
        return false;
    }

    bool IsSplitValid(SettlementArea area, WorldChunkBatch generatingIn)
    {
        return AreBoundsValid(area.buildingZone.GetBounds(), generatingIn);
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

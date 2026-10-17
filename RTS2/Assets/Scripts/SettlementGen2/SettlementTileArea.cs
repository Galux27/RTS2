using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class SettlementTileArea 
{
    public const int MinWidth = 10, MinHeight = 10;
    public int[,] TileArea, BuildingArea;
    Vector2 bottomLeft, topRight;
    int width,height;
    public bool Valid = true;
    public List<SettlementTileAreaSection> Sections=new List<SettlementTileAreaSection>();
    public SettlementTileArea(GeneratedSettlement settlement,Settlement_Settings settings)
    {
        TileArea = new int[Mathf.RoundToInt(settings.Size.x), Mathf.RoundToInt(settings.Size.y)];
        BuildingArea= new int[Mathf.RoundToInt(settings.Size.x), Mathf.RoundToInt(settings.Size.y)];
        width = TileArea.GetLength(0);
        height= TileArea.GetLength(1);
       bottomLeft = settings.Center - (settings.Size / 2);
       topRight = settings.Center + (settings.Size / 2);
        RoadDetails road = null;
        for (int x = 0; x < settlement.roads.Count; x++)
        {
            road = RoadTypeManager.Instance.AllRoadDetails[RoadType.Backroad.ToString()];

            ConvertSettlementRoadToGrid(settlement.roads[x], settings,road);
        }
        for (int x = 0; x < settlement.avenues.Count; x++)
        {
            road = RoadTypeManager.Instance.AllRoadDetails[RoadType.MinorRoad.ToString()];

            ConvertSettlementRoadToGrid(settlement.avenues[x], settings, road);
        }
        for (int x = 0; x < settlement.highways.Count; x++)
        {
            road = RoadTypeManager.Instance.AllRoadDetails[RoadType.MajorRoad.ToString()];

            ConvertSettlementRoadToGrid(settlement.highways[x], settings, road);
        }
        for (int x = 0; x < settlement.avenues.Count; x++)
        {
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.avenues[x], settings, false), Sections.Count+1, settlement.avenues[x]));
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.avenues[x], settings, true), Sections.Count + 1, settlement.avenues[x]));

        }

        for (int x = 0; x < settlement.roads.Count; x++)
        {
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.roads[x], settings, false),Sections.Count + 1, settlement.roads[x]));
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.roads[x], settings, true), Sections.Count + 1, settlement.roads[x]));

        }

        Sections = FilterIntersectingBuildingSections(Sections);
        SettlementBuildingMoniter BuildingMoniter = new SettlementBuildingMoniter(SettlementBuildingTypeManager.Instance.GetRandomData());
        List<TileAreaRoad> RoadsCreatedInTileAreas = new List<TileAreaRoad>();
        for(int q = 0; q < Sections.Count; q++)
        {
            Sections[q].Expand(this,settlement);
            
            Debug.Log("Finished area was " + Sections[q].Low + "->" + Sections[q].High+","+(Sections[q].High- Sections[q].Low));
            if (Sections[q].IsValid)
            {
                Sections[q].IsValid = Sections[q].DoesAreaMeetMinSize();
            }
            if (Sections[q].IsValid) {
                for (int x = Sections[q].Low.x; x < Sections[q].High.x; x++)
                {
                    for (int y = Sections[q].Low.y; y < Sections[q].High.y; y++)
                    {
                        SetTileArea(x, y, Sections[q].ID);
                    }
                }
                Sections[q].GenerateBuildingAreas(ref BuildingMoniter,ref RoadsCreatedInTileAreas);
                for(int i = 0; i < Sections[q].BuildingAreas.Count; i++)
                {
                    for(int x = Sections[q].BuildingAreas[i].Low.x;x< Sections[q].BuildingAreas[i].High.x; x++)
                    {
                        for (int y = Sections[q].BuildingAreas[i].Low.y; y < Sections[q].BuildingAreas[i].High.y; y++)
                        {
                            SetBuildingTileArea(x, y, i+1);
                        }
                    }
                }
            }
        }
        
        for(int x = 0; x < RoadsCreatedInTileAreas.Count; x++)
        {
            ConvertTileAreaRoadToSettlementRoad(RoadsCreatedInTileAreas[x], settings, ref settlement);
        }

        for (int q = 0; q < Sections.Count; q++)
        {
            Sections[q].ApplyWorldOffsetToArea(settings);
        }
        //GenerateBuildingsInSections(settlement);
        
    }


   

    List<SettlementTileAreaSection> FilterIntersectingBuildingSections(List<SettlementTileAreaSection> areas)
    {
        List<SettlementTileAreaSection> retVal = new List<SettlementTileAreaSection>();
        HashSet<int> removed = new HashSet<int>();
        
        for(int x = 0; x < areas.Count; x++)
        {
            bool valid = true;
            for (int q = 0; q < areas.Count; q++)
            {
                if (removed.Contains(q)||x==q)
                {
                    continue;
                }
                if (areas[x].IsIntersectingSegment(areas[q]))
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
            {
                removed.Add(x);
            }
            else
            {
                retVal.Add(areas[x]);
            }
        }
        Debug.Log("Filtering intersecting areas: " + areas.Count + "->" + retVal.Count);
        return retVal;
    }

    void ConvertTileAreaRoadToSettlementRoad(TileAreaRoad road,Settlement_Settings settings,ref GeneratedSettlement settlement)
    {
        road.StartPos = ConvertGridCoordsToPos(road.StartPos,settings);
        road.EndPos = ConvertGridCoordsToPos(road.EndPos, settings);
        settlement.AddRoad(road.ConvertToSettlementRoad());
    }
   

    public void GenerateBuildingsInSections(GeneratedSettlement settlement)
    {
       
        for (int q = 0; q < Sections.Count; q++)
        {
            if (Sections[q].IsValid)
            {
                for (int i = 0; i < Sections[q].BuildingAreas.Count; i++)
                {
                    Sections[q].BuildingAreas[i].GenerateBuildingForArea(settlement);
                }
            }

        }
    }

    void ConvertSettlementRoadToGrid(Settlement_Road road, Settlement_Settings settings,RoadDetails roadDeets)
    {
        Vector2 startPos = road.StartPos;
        Vector2 endPos = road.EndPos;
        float inc = 1f / Vector2.Distance(startPos, endPos);
        Vector2 pos = startPos;
        Vector2Int coords = Vector2Int.zero;
        for(float f = 0f; f <= 1f; f += inc/2f)
        {

            pos=Vector2.Lerp(startPos, endPos, f);
            coords = ConvertPosToGridCoords(pos, settings);
            SetTileArea(coords, RoadTypeToInt(road),roadDeets.RoadWidth+roadDeets.EdgeWidth+2, roadDeets.RoadWidth + roadDeets.EdgeWidth+2);
        }
    }

    public Texture2D GenerateDebugTexture()
    {
        Texture2D retVal = new Texture2D(TileArea.GetLength(0), TileArea.GetLength(1));
        for(int x = 0; x < TileArea.GetLength(0); x++)
        {
            for(int y = 0; y < TileArea.GetLength(1);y++)
            {
               // if (TileArea[x, y] < 0)
                {
                    retVal.SetPixel(x, y, IntToColor(TileArea[x, y],x,y));
                }
            }
        }
        retVal.filterMode = FilterMode.Point;
        retVal.Apply();

        return retVal;
    }

    Color IntToColor(int i,int x,int y)
    {
        if (i > 0)
        {
            if (Sections[i - 1].BuildingAreas.Count > 0 && CoordsValid(x,y))
            {
                try
                {
                    int val = Mathf.Clamp(BuildingArea[x, y] - 1, 0, int.MaxValue);
                    return Sections[i - 1].BuildingAreas[val].DebugColor;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("error parsing " + i + "," + x + "," + y + "," + BuildingArea[x, y]+","+ Sections[i - 1].BuildingAreas.Count+","+Sections.Count+e.ToSafeString());
                    return Color.yellow;
                }
            }
            else
            {
                return Sections[i - 1].DebugColour;
            }
        }
        if (i == -1)
        {
            return Color.green;
        }else if (i == -2)
        {
            return Color.green;
        }else if (i == -3)
        {
            return Color.green;
        }
        else if (i == -4)
        {
            return Color.green;
        }
        return Color.white;
    }


    int RoadTypeToInt(Settlement_Road road)
    {
        switch (road.RoadType)
        {
            case Settlement_RoadType.Highway: return -1;break;
            case Settlement_RoadType.Avenue: return -2; break;
            case Settlement_RoadType.Road: return -3; break;
            case Settlement_RoadType.Dirt: return -4; break;
            default:return 0; break;
        }
    }

    void SetTileArea(Vector2Int coords,int val,int width,int height)
    {
        try
        {
            for(int x = coords.x - width / 2; x < coords.x + width / 2; x++)
            {
                for (int y = coords.y - height / 2; y < coords.y + height / 2; y++)
                {
                    if (CoordsValid(x, y))
                    {
                        TileArea[x, y] = val;
                    }
                }
            }
        }
        catch
        {
            Debug.LogError("error setting " + coords);
        }
    }

    bool CoordsValid(int x,int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return false;
        }
        return true;
    }
    void SetBuildingTileArea(int x, int y, int val)
    {
        try
        {
            //if (BuildingArea[x, y] == 0)
            {
                BuildingArea[x, y] = val;
            }
        }
        catch
        {
            Debug.LogError("error setting " + x + "," + y+","+BuildingArea.GetLength(0)+","+BuildingArea.GetLength(1));
        }
    }

    void SetTileArea(int x,int y, int val)
    {
        try
        {
            TileArea[x, y] = val;
        }
        catch
        {
            Debug.LogError("error setting " + x+","+y);
        }
    }
    Vector2Int ConvertPosToGridCoords(Vector2 pos,Settlement_Settings settings)
    {
        Vector2 lerp = new Vector2(Mathf.InverseLerp(bottomLeft.x, topRight.x, pos.x), Mathf.InverseLerp(bottomLeft.y, topRight.y, pos.y));
        Vector2 val = new Vector2(Mathf.Lerp(0,settings.Size.x,lerp.x),Mathf.Lerp(0,settings.Size.y,lerp.y));
        return new Vector2Int(Mathf.RoundToInt(val.x),Mathf.RoundToInt(val.y));
    }

    Vector2Int ConvertGridCoordsToPos(Vector2Int gridCoords,Settlement_Settings settings)
    {
        Vector2 lerp = new Vector2(Mathf.InverseLerp(0, settings.Size.x, gridCoords.x), Mathf.InverseLerp(0, settings.Size.y, gridCoords.y));
        Vector2 val = new Vector2(Mathf.Lerp(bottomLeft.x, topRight.x, lerp.x), Mathf.Lerp(bottomLeft.y, topRight.y, lerp.y));
        return new Vector2Int(Mathf.RoundToInt(val.x),Mathf.RoundToInt(val.y));
    }

    Vector2Int GetStartingCoordsFromRoad(Settlement_Road road, Settlement_Settings settings, bool negative = false)
    {
        if (!negative)
        {
            return ConvertPosToGridCoords(road.StartPos + Vector2.Perpendicular(road.Direction) * 5, settings);
        }
        else
        {
            return ConvertPosToGridCoords(road.StartPos + Vector2.Perpendicular(road.Direction) * -5, settings);

        }
    }
    
}

public class SettlementTileAreaSection
{
    public Vector2Int StartPosition;
    public int ID;
    public List<TileAreaDir> ValidDirections;
    public Vector2Int Low, High;
    public Color DebugColour;
    public bool IsValid = true;
    public List<BuildingTileArea> BuildingAreas = new List<BuildingTileArea>();
    public Settlement_Road RoadOriginatingFrom;
    public void ApplyWorldOffsetToArea(Settlement_Settings settings)
    {
        Vector2Int bottomCorner = new Vector2Int(Mathf.RoundToInt( settings.Center.x - (settings.Size.x / 2)),Mathf.RoundToInt( settings.Center.y - (settings.Size.y / 2)));
        Low += bottomCorner;
        High += bottomCorner;
        for(int x = 0; x < BuildingAreas.Count; x++)
        {
            BuildingAreas[x].ApplyWorldOffsetToArea(bottomCorner);
        }
    }

    public bool IsIntersectingSegment(SettlementTileAreaSection area)
    {
        return LineUtil.SquareIntersection(Low, High, area.Low, area.High);
    }
    public void DebugDrawArea()
    {
     
        Vector3 low = new Vector3(Low.x, Low.y);
        Vector3 high = new Vector3(High.x, High.y);

        Vector3 c1 = new Vector3(Low.x, High.y);
        Vector3 c2 = new Vector3(High.x, Low.y);
        Debug.DrawLine(low , c1, DebugColour);
        Debug.DrawLine( c1,high, DebugColour);
        Debug.DrawLine( high,c2, DebugColour);
        Debug.DrawLine(c2, low, DebugColour);

    }
    public SettlementTileAreaSection(Vector2Int start,int id,Settlement_Road roadBuiltFrom)
    {
        this.StartPosition = start;
        this.ID = id;
        Low = StartPosition;
        High=StartPosition;
        ValidDirections = new List<TileAreaDir>
        {
            TileAreaDir.HorizontalNegative,
            TileAreaDir.HorizontalPositive,
            TileAreaDir.VerticalNegative,
            TileAreaDir.VerticalPositive
        };
        RoadOriginatingFrom = roadBuiltFrom;
        DebugColour = new Color(Random.value, 0, 0, 1f);
    }
    #region AreaGeneration
    public bool DoesAreaMeetMinSize()
    {
        return High.x-Low.x>SettlementTileArea.MinWidth && High.y-Low.y>SettlementTileArea.MinHeight;
    }

    public void Expand(SettlementTileArea area,GeneratedSettlement currentlyGenerating)
    {
        try
        {
            if (area.TileArea[Low.x, Low.y] != 0)
            {
                IsValid = false;
                return;
            }
        }
        catch
        {
            IsValid = false;
            return;
        }
        while (ValidDirections.Count > 0)
        {
            if (ValidDirections.Contains(TileAreaDir.HorizontalNegative))
            {
                Low.x--;
                CheckForHorizontalNegativeValid(area,currentlyGenerating);
            }
            if (ValidDirections.Contains(TileAreaDir.VerticalNegative))
            {
                Low.y--;
                CheckForVerticalNegativeValid(area, currentlyGenerating);
            }

            if (ValidDirections.Contains(TileAreaDir.HorizontalPositive))
            {
                High.x++;
                CheckForHorizontalPositiveValid(area, currentlyGenerating);
            }
            if (ValidDirections.Contains(TileAreaDir.VerticalPositive))
            {
                High.y++;
                CheckForVerticalPositiveValid(area, currentlyGenerating);
            }
        }

    }

    void CheckForHorizontalNegativeValid(SettlementTileArea area, GeneratedSettlement currentlyGenerating)
    {
        if (Low.x < 0)
        {
            Low.x = 0;
            ValidDirections.Remove(TileAreaDir.HorizontalNegative);
            return;
        }
        bool hitAnything = false;
        for(int y=Low.y; y < High.y; y++)
        {
            if (area.TileArea[Low.x, y] != 0 || currentlyGenerating.GetAreaFromPosition(new Vector2(Low.x,y),false).CanUse == false)
            {
                hitAnything = true;
                break;
            }
        }
        if (hitAnything)
        {
            Low.x++;
            ValidDirections.Remove(TileAreaDir.HorizontalNegative);
        }
    }

    void CheckForHorizontalPositiveValid(SettlementTileArea area, GeneratedSettlement currentlyGenerating)
    {
        if (High.x >=area.TileArea.GetLength(0))
        {
            High.x = area.TileArea.GetLength(0)-1;
            ValidDirections.Remove(TileAreaDir.HorizontalPositive);
            return;
        }
        bool hitAnything = false;
        for (int y = Low.y; y < High.y; y++)
        {
            if (area.TileArea[High.x, y] != 0 || currentlyGenerating.GetAreaFromPosition(new Vector2(High.x, y), false).CanUse == false)
            {
                hitAnything = true;
                break;
            }
        }
        if (hitAnything)
        {
            High.x--;
            ValidDirections.Remove(TileAreaDir.HorizontalPositive);
        }
    }

    void CheckForVerticalNegativeValid(SettlementTileArea area, GeneratedSettlement currentlyGenerating)
    {
        if (Low.y < 0)
        {
            Low.y = 0;
            ValidDirections.Remove(TileAreaDir.VerticalNegative);
            return;
        }
        bool hitAnything = false;
        for (int x = Low.x; x < High.x; x++)
        {
            if (area.TileArea[x, Low.y] != 0 || currentlyGenerating.GetAreaFromPosition(new Vector2(x, Low.y), false).CanUse == false)
            {
                hitAnything = true;
                break;
            }
        }
        if (hitAnything)
        {
            Low.y++;
            ValidDirections.Remove(TileAreaDir.VerticalNegative);
        }
    }

    void CheckForVerticalPositiveValid(SettlementTileArea area, GeneratedSettlement currentlyGenerating)
    {
        if (High.y >= area.TileArea.GetLength(1))
        {
            High.y = area.TileArea.GetLength(1)-1;
            ValidDirections.Remove(TileAreaDir.VerticalPositive);
            return;
        }
        bool hitAnything = false;
        for (int x = Low.x; x < High.x; x++)
        {
            if (area.TileArea[x, High.y] != 0 || currentlyGenerating.GetAreaFromPosition(new Vector2(x, High.y), false).CanUse == false)
            {
                hitAnything = true;
                break;
            }
        }
        if (hitAnything)
        {
            High.y--;
            ValidDirections.Remove(TileAreaDir.VerticalPositive);
        }
    }
    #endregion
    

    BuildingTileArea GetAreaIIntersect(Vector2Int start,Vector2Int end,out Vector2Int intersection)
    {
        BuildingTileArea retVal = null;
        intersection = end ;
        float dist = Vector2.Distance(start, end);
        for(int x=0;x < BuildingAreas.Count; x++)
        {
            if (BuildingAreas[x].DoesLineIntersectBuilding(start,end,out intersection))
            {
                float dist2 = Vector2.Distance(start, intersection);
                if (dist2 < dist)
                {
                    retVal=BuildingAreas[x];
                    dist = dist2;
                }
            }
        }


        return retVal;
    } 


    void GenerateAlleyway(ref SettlementBuildingMoniter buildingData, ref List<TileAreaRoad> roads, Vector2Int currentPosition)
    {
        Vector2Int startPos = currentPosition;
        Vector2Int endPosition = new Vector2Int(startPos.x, High.y);
        Vector2Int intersection = endPosition;
        while (startPos.y < endPosition.y)
        {
            BuildingTileArea area = GetAreaIIntersect(startPos, endPosition,out intersection);
            if (area == null)
            {
                roads.Add(new TileAreaRoad(startPos, endPosition, RoadType.Alley));
                startPos = endPosition;

            }
            else
            {

            }
        }

    }

    public void GenerateBuildingAreas(ref SettlementBuildingMoniter buildingData,ref List<TileAreaRoad> roadsCreated)
    {
        int areaWidth = High.x - Low.x;
        int areaHeight = High.y - Low.y;
        //get start position in bottom left
        //calculate size and work out valid building types based on that size

        Vector2Int areaSize = new Vector2Int(areaWidth, areaHeight);
        buildingData.SetValidBuildings(areaSize);
        Debug.Log("Generating buildings for area off road at " + this.RoadOriginatingFrom.StartPos + " to " + this.RoadOriginatingFrom.endPos+" startpos was "+ StartPosition);
        int startY = Low.y + 2;

        Vector2Int startPos = new Vector2Int(Low.x + 2, startY);
        Vector2Int localSize = Vector2Int.zero;
        SettlementBuilding building = null;
        bool ableToPlace =true;
        int horizontalEdge = startPos.x;
        int lastRoad = startPos.x;
        while (ableToPlace)
        {
            areaSize = High - startPos;
       
            while (startPos.y < High.y)
            {
                //check up to max building size on both axis
                localSize.x = areaSize.x;
                localSize.y = areaSize.y;
                building = buildingData.GetBuilding(startPos, localSize);
                if (building != null)
                {
                    localSize.x = UnityEngine.Random.Range(building.Building.MinWidth, building.Building.MaxWidth);
                    localSize.y = UnityEngine.Random.Range(building.Building.MinHeight, building.Building.MaxHeight);
                    horizontalEdge = Mathf.Max(horizontalEdge,startPos.x+ localSize.x);
                    BuildingAreas.Add(new BuildingTileArea(startPos, startPos + localSize, building.Building.BuildingName));
                    building = null;
                    startPos.y += localSize.y + 2;
                    areaSize = High - startPos;
                    buildingData.SetValidBuildings(areaSize);

                }
                else
                {
                    startPos.y+=5;
                }
            }
            startPos.y =startY;
            startPos.x = Mathf.Max(startPos.x+5, horizontalEdge + 1);
            if(startPos.x-lastRoad > 25 && (High.x - startPos.x) > 20)
            {
                GenerateAlleyway(ref buildingData, ref roadsCreated, startPos);
                startPos.x += 4;
                lastRoad = startPos.x;
            }
            if (startPos.x >= High.x)
            {
                ableToPlace = false;
            }
        }
       

        //get a building that would fit into the area found
        //place building
        //record width of building placed if higher than current largest
        //increase y of start position of building placed
        //repeat until you can't place any more buildings
        //add highest width to start positions x coord
        //repeat process until the start positions x coord reaches the opposite edge of the grid
        //then ya done



        /*string template = BuildingDataManager.Instance.GetRandomBuildingTemplateID();
        BuildingTemplate houseTemplate = BuildingDataManager.Instance.BuildingTemplates[template];
        int width=Random.Range(houseTemplate.MinWidth,houseTemplate.MaxWidth);
        int height = Random.Range(houseTemplate.MinHeight,houseTemplate.MaxHeight);
       
        int horizontalSpacing = 2, verticalSpacing = 2;
        int horizontalBorder=2,verticalBorder=2;
        if(Low.x + width + (horizontalSpacing + horizontalBorder) > High.x - (horizontalBorder))
        {
            horizontalSpacing = 0;
            
        }
        if (Low.y + height + (verticalSpacing + verticalBorder) > High.y - (verticalBorder))
        {
            verticalSpacing = 0;

        }
        Vector2Int size= new Vector2Int(width, height);
        Vector2Int pos = Low;

        int xStart = Low.x + horizontalBorder;
        int yStart = Low.y + verticalBorder;
        int xEnd = High.x - (width + horizontalBorder);
        int yEnd = High.y - (height + verticalBorder);
        int xIncrement = width + horizontalSpacing;
        int yIncrement = height + verticalBorder;
        int xRemainder = xStart;
        int xCount = 0;
        int yCount = 0;
        while (xRemainder + xIncrement < xEnd)
        {
            xCount++;
            xRemainder += xIncrement;
        }
        int yRemainder = yStart;
        while(yRemainder + yIncrement < yEnd)
        {
            yCount++;
            yRemainder += yIncrement;
        }
        yRemainder = yEnd - yRemainder;
        xRemainder = xEnd - xRemainder;
        yRemainder /= Mathf.Max(1, yCount);
        xRemainder/=Mathf.Max(1, xCount);

        for (int x =xStart; x <xEnd; x+=xIncrement+xRemainder)
        {
            for (int y = yStart; y < yEnd; y +=yIncrement+yRemainder)
            {
                pos.x = x;
                pos.y=y;
                BuildingAreas.Add(new BuildingTileArea(pos, pos + size, template));

            }
        }*/
    }
}

public class TileAreaRoad
{

    public TileAreaRoad(Vector2Int s,Vector2Int e,RoadType type)
    {
        StartPos = s;
        EndPos = e;
        Type = type;
    }




    public Vector2Int StartPos, EndPos;
    public RoadType Type;


    public Settlement_Road ConvertToSettlementRoad()
    {
        Vector2 dir = EndPos - StartPos;
        float length = Vector2Int.Distance(StartPos, EndPos);
        return new Settlement_Road(StartPos, dir, length, GetTypeAsSettlementRoadType());
    }

    Settlement_RoadType GetTypeAsSettlementRoadType()
    {
        switch (Type)
        {
            case RoadType.None:
                break;
            case RoadType.MajorRoad:
                return Settlement_RoadType.Highway;
                break;
            case RoadType.MinorRoad:
                return Settlement_RoadType.Avenue;
                break;
            case RoadType.Backroad:
                return Settlement_RoadType.Road;
                break;
            case RoadType.Alley:
                return Settlement_RoadType.Alley;
                break;
            default:
                break;
        }
        return Settlement_RoadType.Dirt;
    }
}

public class BuildingTileArea
{
    public Vector2Int Low, High;
    public Color DebugColor;
    public string BuildingID;
    public BuildingTileArea(Vector2Int low,Vector2Int high,string buildingToGenerate)
    {
        Low = low;
        High = high;
        BuildingID = buildingToGenerate;
        DebugColor = new Color(Random.value, Random.value, Random.value, 1f);
    }

    public void ApplyWorldOffsetToArea(Vector2Int bottomCorner)
    {
        Low += bottomCorner;
        High += bottomCorner;

    }


    public void DebugDrawArea()
    {
        Vector3 low = new Vector3(Low.x, Low.y);
        Vector3 high = new Vector3(High.x, High.y);

        Vector3 c1 = new Vector3(Low.x, High.y);
        Vector3 c2 = new Vector3(High.x, Low.y);
        Debug.DrawLine(low, c1, DebugColor);
        Debug.DrawLine(c1, high, DebugColor);
        Debug.DrawLine(high, c2, DebugColor);
        Debug.DrawLine(c2, low, DebugColor);

    }

    public GeneratedBuilding MyBuilding;
    public void GenerateBuildingForArea(GeneratedSettlement settlement)
    {
        Vector2Int Size = High - Low;

        BuildingFloorplan floorplan = BuildingFloorplan.GetFloorplanByType(BuildingDataManager.Instance.BuildingTemplates[BuildingID].FloorplanType);
        Debug.Log("Generating building at " +Low + " size " + Size);
        MyBuilding=floorplan.Generate(BuildingGenerator.Instance.RoomGen, Size.x, Size.y, Low, BuildingDataManager.Instance.BuildingTemplates[BuildingID], 50);
    }

    public bool DoesLineIntersectBuilding(Vector2Int start,Vector2Int end,out Vector2Int intersection)
    {
        intersection = Vector2Int.zero;

        float dist = Vector2.Distance(start, end);
        float dist2 = dist;
        Vector2 latestIntersection= new Vector2();
        Vector2Int p1 = Low;
        Vector2Int p2 = new Vector2Int(High.x, Low.y);
        if(LineUtil.IntersectLineSegments2D(start,end,p1,p2,out latestIntersection))
        {
            dist2 = Vector2.Distance(start, latestIntersection);
            if (dist2 < dist)
            {
                intersection = new Vector2Int(Mathf.FloorToInt( latestIntersection.x), Mathf.FloorToInt(latestIntersection.y));
                dist = dist2;
            }
        }
        p2.y = High.y;
        p1.x = High.x;
        if (LineUtil.IntersectLineSegments2D(start,latestIntersection, p1, p2, out latestIntersection))
        {
            dist2 = Vector2.Distance(start, latestIntersection);
            if (dist2 < dist)
            {
                intersection = new Vector2Int(Mathf.FloorToInt(latestIntersection.x), Mathf.FloorToInt(latestIntersection.y));
                dist = dist2;
            }
        }
        p1 = High;
        p2.x = Low.x;
        if (LineUtil.IntersectLineSegments2D(start, latestIntersection, p1, p2, out latestIntersection))
        {
            dist2 = Vector2.Distance(start, latestIntersection);
            if (dist2 < dist)
            {
                intersection = new Vector2Int(Mathf.FloorToInt(latestIntersection.x), Mathf.FloorToInt(latestIntersection.y));
                dist = dist2;
            }
        }

        p1 = Low;
        if (LineUtil.IntersectLineSegments2D(start, latestIntersection, p1, p2, out latestIntersection))
        {
            dist2 = Vector2.Distance(start, latestIntersection);
            if (dist2 < dist)
            {
                intersection = new Vector2Int(Mathf.FloorToInt(latestIntersection.x), Mathf.FloorToInt(latestIntersection.y));
                dist = dist2;
            }
        }

        return false;
    }
}

public enum TileAreaDir
{
    HorizontalNegative,
    HorizontalPositive,
    VerticalNegative,
    VerticalPositive
}

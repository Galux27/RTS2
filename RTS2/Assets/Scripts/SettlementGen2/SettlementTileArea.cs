using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SettlementTileArea 
{
    public const int MinWidth = 10, MinHeight = 10;
    public int[,] TileArea;
    Vector2 bottomLeft, topRight;
    int width,height;
    public bool Valid = true;
    public List<SettlementTileAreaSection> Sections=new List<SettlementTileAreaSection>();
    public SettlementTileArea(GeneratedSettlement settlement,Settlement_Settings settings)
    {
        TileArea = new int[Mathf.RoundToInt(settings.Size.x), Mathf.RoundToInt(settings.Size.y)];
        width = TileArea.GetLength(0);
        height= TileArea.GetLength(1);
       bottomLeft = settings.Center - (settings.Size / 2);
       topRight = settings.Center + (settings.Size / 2);
        for (int x = 0; x < settlement.roads.Count; x++)
        {
            ConvertSettlementRoadToGrid(settlement.roads[x], settings);
        }
        for (int x = 0; x < settlement.avenues.Count; x++)
        {
            ConvertSettlementRoadToGrid(settlement.avenues[x], settings);
        }
        for (int x = 0; x < settlement.highways.Count; x++)
        {
            ConvertSettlementRoadToGrid(settlement.highways[x], settings);
        }
        for (int x = 0; x < settlement.avenues.Count; x++)
        {
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.avenues[x], settings, false), Sections.Count));
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.avenues[x], settings, true), Sections.Count));

        }

        for (int x = 0; x < settlement.roads.Count; x++)
        {
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.roads[x], settings, false),Sections.Count));
            Sections.Add(new SettlementTileAreaSection(GetStartingCoordsFromRoad(settlement.roads[x], settings, true), Sections.Count));

        }

        for(int q = 0; q < Sections.Count; q++)
        {
            Sections[q].Expand(this);
            
            Debug.Log("Finished area was " + Sections[q].Low + "->" + Sections[q].High+","+(Sections[q].High- Sections[q].Low));
            Sections[q].IsValid = Sections[q].DoesAreaMeetMinSize();
            if (Sections[q].IsValid) {
                for (int x = Sections[q].Low.x; x < Sections[q].High.x; x++)
                {
                    for (int y = Sections[q].Low.y; y < Sections[q].High.y; y++)
                    {
                        SetTileArea(x, y, Sections[q].ID);
                    }
                }
            }
        }
    }


    void ConvertSettlementRoadToGrid(Settlement_Road road, Settlement_Settings settings)
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
            SetTileArea(coords, RoadTypeToInt(road),4,4);
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
                    retVal.SetPixel(x, y, IntToColor(TileArea[x, y]));
                }
            }
        }
        retVal.filterMode = FilterMode.Point;
        retVal.Apply();

        return retVal;
    }

    Color IntToColor(int i)
    {
        if (i > 0)
        {
            return Sections[i - 1].DebugColour;
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
                for (int y = coords.y - width / 2; y < coords.y + width / 2; y++)
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
    public SettlementTileAreaSection(Vector2Int start,int id)
    {
        this.StartPosition = start;
        this.ID = id;
        Low = StartPosition;
        High=StartPosition;
        ValidDirections = new List<TileAreaDir>();
        ValidDirections.Add(TileAreaDir.HorizontalNegative);
        ValidDirections.Add(TileAreaDir.HorizontalPositive);
        ValidDirections.Add(TileAreaDir.VerticalNegative);
        ValidDirections.Add(TileAreaDir.VerticalPositive);
        DebugColour = new Color(Random.value, 0, 0, 1f);
    }

    public bool DoesAreaMeetMinSize()
    {
        return High.x-Low.x>SettlementTileArea.MinWidth && High.y-Low.y>SettlementTileArea.MinHeight;
    }

    public void Expand(SettlementTileArea area)
    {
        while (ValidDirections.Count > 0)
        {
            if (ValidDirections.Contains(TileAreaDir.HorizontalNegative))
            {
                Low.x--;
                CheckForHorizontalNegativeValid(area);
            }
            if (ValidDirections.Contains(TileAreaDir.VerticalNegative))
            {
                Low.y--;
                CheckForVerticalNegativeValid(area);
            }

            if (ValidDirections.Contains(TileAreaDir.HorizontalPositive))
            {
                High.x++;
                CheckForHorizontalPositiveValid(area);
            }
            if (ValidDirections.Contains(TileAreaDir.VerticalPositive))
            {
                High.y++;
                CheckForVerticalPositiveValid(area);
            }
        }

    }

    void CheckForHorizontalNegativeValid(SettlementTileArea area)
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
            if (area.TileArea[Low.x, y] != 0)
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

    void CheckForHorizontalPositiveValid(SettlementTileArea area)
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
            if (area.TileArea[High.x, y] != 0)
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

    void CheckForVerticalNegativeValid(SettlementTileArea area)
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
            if (area.TileArea[x, Low.y] != 0)
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

    void CheckForVerticalPositiveValid(SettlementTileArea area)
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
            if (area.TileArea[x, High.y] != 0)
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
}

public enum TileAreaDir
{
    HorizontalNegative,
    HorizontalPositive,
    VerticalNegative,
    VerticalPositive
}

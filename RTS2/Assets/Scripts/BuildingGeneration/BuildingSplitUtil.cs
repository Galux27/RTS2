using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
public static class BuildingSplitUtil
{
   public static Dictionary<Vector2Int,GeneratedBuilding> SplitBuildingIntoChunks(GeneratedBuilding toSplit)
    {
        //Calculate all chunks needed, start with the chunk the building starts in and work out size any others
        //Create 
        Vector2Int size = new Vector2Int(toSplit.Width,toSplit.Height);
        Vector2Int buildingTopPoint = toSplit.Position + size;
        Vector2Int startChunk= WorldChunkManager.Instance.ConvertPositionToChunkBatchCoordsVec2(toSplit.Position);
        Dictionary<Vector2Int, GeneratedBuilding> retVal = new Dictionary<Vector2Int, GeneratedBuilding>();

        Vector2Int endChunk = WorldChunkManager.Instance.ConvertPositionToChunkBatchCoordsVec2(buildingTopPoint);



        if (startChunk == endChunk)
        {
            retVal.Add(startChunk, toSplit);
            return retVal;
        }else
        {
            List<BuildingSplitCoords> splits = new List<BuildingSplitCoords>();

            Vector2Int initialSize = (startChunk + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize)) - toSplit.Position;
            if (startChunk.x == endChunk.x)
            {
                initialSize.x = toSplit.Width;
            }
            splits.Add(new BuildingSplitCoords(toSplit.Position,initialSize));
            int start = startChunk.x;

           

            if (startChunk.x != endChunk.x)
            {
                while (start < buildingTopPoint.x)
                {
                    start += WorldChunkManager.ChunkBatchSize;
                    Vector2Int convertedPosition = WorldChunkManager.Instance.ConvertPositionToChunkBatchCoordsVec2(new Vector2Int(start, toSplit.Position.y));
                    convertedPosition.y = toSplit.Position.y;
                    if (convertedPosition.x < buildingTopPoint.x)
                    {
                        splits.Add(new BuildingSplitCoords(convertedPosition, CalculateSize(new Vector2Int(start, toSplit.Position.y), toSplit, buildingTopPoint)));
                    }
                }
            }
            start = startChunk.y;
            List<BuildingSplitCoords> verticalSplits = new List<BuildingSplitCoords>();
            while (start < Mathf.Min( buildingTopPoint.y,endChunk.y))
            {
                start += WorldChunkManager.ChunkBatchSize;
                Vector2Int convertedPosition = WorldChunkManager.Instance.ConvertPositionToChunkBatchCoordsVec2(new Vector2Int( toSplit.Position.x, start));
                for (int x = 0; x < splits.Count; x++)
                {
                    convertedPosition.x = splits[x].Position.x;
                    verticalSplits.Add(new BuildingSplitCoords(convertedPosition, CalculateSize(new Vector2Int(splits[x].Position.x,start), toSplit, buildingTopPoint)));

                }
            }
            splits.AddRange(verticalSplits);
            List<GeneratedRoomProp> props = new List<GeneratedRoomProp>();
            for (int x = 0; x < splits.Count; x++)
            {
                splits[x].DebugDraw();
                Vector2Int startIndex = splits[x].Position - toSplit.Position;
                Vector2Int endIndex =  startIndex + splits[x].Size;

                Vector2Int batch = WorldChunkManager.Instance.ConvertPositionToChunkBatchCoordsVec2(splits[x].Position);
                GeneratedBuilding building = new GeneratedBuilding(splits[x].Size.x, splits[x].Size.y, splits[x].Position, toSplit.buildingType);

                List<RoomTile> TilesTaken = new List<RoomTile>();
                List<RoomTile> TilesCreated = new List<RoomTile>();
                try
                {
                    for (int x1 = startIndex.x; x1 < Mathf.Min(endIndex.x, toSplit.Tiles.GetLength(0)); x1++)
                    {
                        for (int y1 = startIndex.y; y1 < Mathf.Min(endIndex.y,toSplit.Tiles.GetLength(1)); y1++)
                        {
                            building.Tiles[x1 - startIndex.x, y1 - startIndex.y] = toSplit.Tiles[x1, y1];
                            TilesTaken.Add(toSplit.Tiles[x1, y1]);

                        }
                    }
                }
                catch(System.Exception e)
                {
                    Debug.LogError(e.ToString());
                    Debug.LogError("Error making building " + building.Tiles.GetLength(0) + "," + building.Tiles.GetLength(1) + " to split " + toSplit.Tiles.GetLength(0) + "," + toSplit.Tiles.GetLength(1)
                        +" start ind " +startIndex+" end index "+endIndex);
                }
                props=toSplit.GetPropsFromSlicedRoom(TilesTaken);
                if (props.Count > 0)
                {
                    GeneratedRoom propRoom = new GeneratedRoom(new Vector2Int(building.Width, building.Height), Vector2Int.zero, "Props", 1);
                    for(int q = 0; q < props.Count; q++)
                    {
                        Vector2Int splitOffset =  building.Position-toSplit.Position;
                        Vector2Int newPosition = props[q].pos - building.Position;// toSplit.Position;  
                        propRoom.AddEnvObject(propRoom.RoomTiles[newPosition.x, newPosition.y], props[q]);
                    }
                    building.AddRoom(propRoom);
                }
                retVal.Add(batch, building);

            }
        }

        return retVal;
    }

    static Vector2Int CalculateSize(Vector2Int position,GeneratedBuilding building,Vector2Int buildingTopPoint)
    {
        Vector2Int worldTopPoint = new Vector2Int(Mathf.Min(position.x + WorldChunkManager.ChunkBatchSize, buildingTopPoint.x),
            Mathf.Min(position.y + WorldChunkManager.ChunkBatchSize, buildingTopPoint.y));
        return (worldTopPoint - position)+Vector2Int.one;
    }
    public static void DebugDraw(Vector2Int Position,Vector2Int Size, UnityEngine.Color c)
    {
        Vector3 p1 = new Vector3(Position.x, Position.y);
        Vector3 p2 = new Vector3(Position.x + Size.x, Position.y + Size.y);
        Debug.DrawLine(p1, p1 + new Vector3(Size.x, 0, 0), c, 100f);
        Debug.DrawLine(p1, p1 + new Vector3(0, Size.y, 0), c, 100f);
        Debug.DrawLine(p1 + new Vector3(Size.x, 0), p2, c, 100f);
        Debug.DrawLine(p1 + new Vector3(0, Size.y, 0), p2, c, 100f);

    }
}

public class BuildingSplitCoords
{
    public Vector2Int Position;
    public Vector2Int Size;
    public BuildingSplitCoords(Vector2Int batch,Vector2Int size)
    {
        Position= batch;
        Size= size;
    }

    public void DebugDraw()
    {
        return;
        Debug.Log("Splits: Drawing splits " + Position + " size " + Size);
        UnityEngine.Color c = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.yellow, .5f);
        Vector3 p1 = new Vector3(Position.x, Position.y);
        Vector3 p2 = new Vector3(Position.x + Size.x, Position.y + Size.y);
        Debug.DrawLine(p1, p1 + new Vector3(Size.x, 0,0),c,100f);
        Debug.DrawLine(p1, p1 + new Vector3(0, Size.y, 0), c, 100f);
        Debug.DrawLine( p1 + new Vector3( Size.x, 0),p2, c, 100f);
        Debug.DrawLine( p1 + new Vector3(0, Size.y, 0),p2, c, 100f);

    }
}

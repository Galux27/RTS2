using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
  static BuildingGenerator instance;
    public static BuildingGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<BuildingGenerator>();
            }
            return instance;
        }
    }

    public RoomGenerator RoomGen;
    public RoomTemplate TestTemplate;
    public void GenerateBuilding()
    {
        int width = Random.Range(5, 7);
        int height = Random.Range(width-2,width+2);

        RoomGen = new RoomGenerator();
        Vector2Int camPos = new Vector2Int((int)CameraController.Instance.transform.position.x,(int)CameraController.Instance.transform.position.y);
        GeneratedRoom r = RoomGen.GenerateRoom(Vector2Int.zero, new Vector2Int(width, height), TestTemplate);
        ApplyRoomToWorld(camPos,r);
    }

    void ApplyRoomToWorld(Vector2Int coords,GeneratedRoom r)
    {
        Vector2Int pos = coords;
        RoomTile cur = null;
        Vector2Int batchCoords, chunkCoords, localCoords;
        uint ID = 0;

        for (int x = 0; x <  r.size.x; x++)
        {
            for (int y = 0; y < r.size.y; y++)
            {
                pos = coords;
                pos.x += x;
                pos.y += y;
                cur = r.RoomTiles[x,y];
                WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out batchCoords, out chunkCoords, out localCoords);
                ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(cur.FloorTile);

                if (cur.HasWall)
                {
                    WallHelpers.CreateWallBuildableStructure(pos.x, pos.y, WorldController.Instance.BuildingTilemap, 
                        WallTypeManager.Instance.GetWallTile(cur.WallTile),new Vector3(pos.x,pos.y,0), new Vector3(.5f, .5f, 0f));

                }

                if (cur.HasFloor)
                {
                    WorldChunkManager.Instance.ChunkBatches[batchCoords].Chunks[chunkCoords.x, chunkCoords.y].UpdateTile(localCoords.x, localCoords.y, cur.FloorTile, ID);
                }

            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateBuilding();
        }
    }
}

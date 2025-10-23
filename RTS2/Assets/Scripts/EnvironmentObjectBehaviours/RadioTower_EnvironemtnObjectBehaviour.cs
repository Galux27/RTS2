using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Radio Tower Behavior", menuName = "ScriptableObjects/ConstructableObjectBehaviours/Radio Tower", order = 1)]
public class RadioTower_EnvironemtnObjectBehaviour : EnvironmentObjectBehaviourBase
{
    /// <summary>
    /// check for spawning civilians every x frames
    /// </summary>
    const int CheckRate = 1000;
    int checkCount = 0;

    List<Vector2Int> BatchesICanSpawnIn;
    Vector2Int batch, chunk, coords;
    Vector3 target = Vector3.zero;
    WorldTile myTile = null;
    public override void PassInVector(Vector3 data, string use)
    {
       base.PassInVector(data, use);
       // WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(data.x, data.y, out batch, out chunk, out coords);
       
        BatchesICanSpawnIn = new List<Vector2Int>();
        BatchesICanSpawnIn.Add(batch + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0));
        BatchesICanSpawnIn.Add(batch + new Vector2Int(-WorldChunkManager.ChunkBatchSize, 0));
        BatchesICanSpawnIn.Add(batch + new Vector2Int( 0, WorldChunkManager.ChunkBatchSize));
        BatchesICanSpawnIn.Add(batch + new Vector2Int(0, -WorldChunkManager.ChunkBatchSize));
    }

    void FindTileForPathing()
    {
        myTile = Pathfinding.GetTileFromPosition(myPosition);
        batch = myTile.Batch; 
        chunk = myTile.Chunk;
        coords = myTile.Coords();
        Vector3 startPos = new Vector3(myTile.Coords().x, myTile.Coords().y);
        Vector3 curTilePos = new Vector3();
        WorldTile toCheck = null;
        float closestDist = 99999999f;
        WorldChunk wc = WorldChunkManager.Instance.GetChunkBatch(batch).Chunks[chunk.x, chunk.y];

        for(int x=-5;x<5;x++)
        {
            for(int y=-5;y<5;y++)
            {
                curTilePos = myPosition;
                curTilePos.x += x;
                curTilePos.y += y;

                toCheck=Pathfinding.GetTileFromPosition(curTilePos);
                if (toCheck.TileTraversable())
                {
                    float dist = Vector3.Distance(curTilePos, startPos);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        myTile = toCheck;
                    }
                    Debug.DrawLine(myPosition, curTilePos, Color.green, 999f);
                }
            }
        }

        //for (int x=0;x<WorldChunkManager.ChunkSize;x++)
        //{
        //    for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
        //    {
        //        //wc.ChunkTiles[x, y];
        //        curTilePos = new Vector3(toCheck.Coords().x, toCheck.Coords().y);

        //        if (toCheck.TileTraversable() && wc.PathfindingNodes[x,y].IsPassable)
        //        {
        //            float dist = Vector3.Distance(curTilePos,startPos);
        //            if(dist<closestDist)
        //            {
        //                closestDist = dist;
        //                myTile = toCheck;
        //            }
                 
        //        }
              
        //    }
        //}

        target = GetTargetNode().worldPos;
        Debug.Log("Radio tower target was " + GetTargetNode().worldPos +" "+ GetTargetNode().IsPassable);
    }

    bool InitData = false;

    public override bool HasUpdate()
    {
        return true;
    }
   
    Vector2Int GetBatch()
    {
        return BatchesICanSpawnIn[Random.Range(0, BatchesICanSpawnIn.Count)];
    }


    public override void OnUpdate()
    {
        checkCount++;
        if (checkCount >= CheckRate)
        {
            if (UnitCapacityManager.GetRemainingCapacityForType("Civilian") > 0)
            {
                CreateCivilian();
            }
            checkCount = 0;
        }
    }

    void InitDataForPathfinding()
    {
        FindTileForPathing();

    }
    void CreateCivilian()
    {
        if (!InitData)
        {
            InitDataForPathfinding();
            InitData = true;
        }
        Vector2Int batch = GetBatch();
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batch) == false)
        {
            return;
        }

        Vector2Int chunk = new Vector2Int(Random.Range(0,WorldChunkManager.ChunkSize), Random.Range(0, WorldChunkManager.ChunkSize));
        Vector2Int tile = new Vector2Int(Random.Range(0, WorldChunkManager.ChunkSize), Random.Range(0, WorldChunkManager.ChunkSize));

        int xCoord = 0;
        int yCoord = 0;

        WorldTile toSpawnOn = WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[tile.x, tile.y];
        if (toSpawnOn.TileTraversable())
        {
            Vector3 worldPos = new Vector3(toSpawnOn.Coords().x,toSpawnOn.Coords().y);
            UnitTypeSO civ = UnitTypesController.Instance.Units["Civilian"];
            GameObject g = Instantiate(civ.Prefab,worldPos , Quaternion.identity);

            MoveTo_Behaviour moveTo = new MoveTo_Behaviour();
            moveTo.InitBehaviour(g.GetComponent<Unit>(), GetTargetNode(), true);
            moveTo.IsUserInstruction = true;
            g.GetComponent<BehaviourRunner>().SetBehaviour(moveTo);

            Debug.DrawLine(worldPos, target,Color.green,999f);
            Debug.DrawLine(worldPos, myPosition, Color.red, 999f);

        }
    }
    PathfindingNode GetTargetNode()
    {
        return WorldChunkManager.Instance.GetChunkBatch(myTile.Batch).Chunks[myTile.Chunk.x, myTile.Chunk.y].PathfindingNodes[myTile.Local.x, myTile.Local.y];
    }
}

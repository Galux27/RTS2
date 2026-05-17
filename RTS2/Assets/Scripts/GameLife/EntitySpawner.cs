using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntitySpawner
{
    public static GameObject SpawnEntity(WorldChunkBatch batch, string entityType,string factionID, int forceChunkX = -1, int forceChunkY = -1,int forceTileX=-1,int forceTileY=-1)
    {
        if (DebugCheats.Instance.DoWeSpawnEnemies() == false && factionID==FactionController.ZOMBIE_FACTION)
        {
            return null ;
        }
        GameObject retVa = null;
        if(UnitTypesController.Instance.UnitKeys.Contains(entityType))
        {
            UnitTypeSO data = UnitTypesController.Instance.Units[entityType];
            if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batch.coords) == false)
            {
                return null;
            }

            Vector2Int chunk = new Vector2Int(Random.Range(1, WorldChunkManager.ChunksPerBatch - 1), Random.Range(1, WorldChunkManager.ChunksPerBatch - 1));
            if (forceChunkX != -1)
            {
                chunk.x = forceChunkX;
            }
            if (forceChunkY != -1)
            {
                chunk.y = forceChunkY;
            }

            Vector2Int tile = new Vector2Int(Random.Range(0, WorldChunkManager.ChunkSize), Random.Range(0, WorldChunkManager.ChunkSize));
            if (forceTileX != -1)
            {
                tile.x = forceTileX;
            }
            if (forceTileY != -1)
            {
                tile.y = forceTileY;
            }
            WorldTile toSpawnOn = WorldChunkManager.Instance.ChunkBatches[batch.coords].Chunks[chunk.x, chunk.y].ChunkTiles[tile.x, tile.y];
            if (toSpawnOn.TileTraversable())
            {
                Vector3 worldPos = new Vector3(toSpawnOn.Coords().x, toSpawnOn.Coords().y);
                UnitTypeSO unit = data;
                GameObject g = GameObject.Instantiate(unit.Prefab, worldPos, Quaternion.identity);
                g.GetComponent<Unit>().MyFaction.MyFactionID = factionID;

                Debug.Log("A Life: dpawn zomb crossing chunks " + worldPos, g);
                return g;
            }
        }
        return retVa;
    }
}

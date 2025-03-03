using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string roomName = "";
    public Color displayColour;
    public List<Vector2Int> tilesInRoom = new List<Vector2Int>();
    public RoomUseType roomType;
    public List<Vector2Int> EdgeTiles,InvalidEdge;
    public static Action<Room> OnRoomChanged;

    public List<ConstructableObjectInstance> ObjectsInRoom = new List<ConstructableObjectInstance>();


    public Room()
    {
        roomName = "Room " + RoomManager.Instance.roomList.Count;
        displayColour = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), .25f);
    }


    public void AddTiles(List<Vector2Int> tilesInRoom)
    {
        List<Vector2Int> addedTiles = new List<Vector2Int>();
        for(int x=0;x<tilesInRoom.Count;x++)
        {
            if (!this.tilesInRoom.Contains(tilesInRoom[x]))
            {
                this.tilesInRoom.Add(tilesInRoom[x]);
                addedTiles.Add(tilesInRoom[x]);
            }
        }
        OnRoomChanged?.Invoke(this);
        CheckForItemsThatCouldBeInRoom(addedTiles);
        RefreshRoom();
    }

    public void RemoveTiles(List<Vector2Int> tilesInRoom)
    {
        for(int i = 0; i < tilesInRoom.Count; i++)
        {
            this.tilesInRoom.Remove(tilesInRoom[i]);

        }
        CheckForConstructablesNoLongerInRoom(tilesInRoom);
        OnRoomChanged?.Invoke(this);
        RefreshRoom();

    }

    void CheckForItemsThatCouldBeInRoom(List<Vector2Int> NewTilesInRoom)
    {
        HashSet<Vector2Int> chunksChecked = new HashSet<Vector2Int>();

        List<ConstructableObjectInstance> newObjects = new List<ConstructableObjectInstance>();
        for(int x = 0; x < NewTilesInRoom.Count; x++)
        {
            Vector2Int chunkCoords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(NewTilesInRoom[x]);
            if(!chunksChecked.Contains(chunkCoords))
            {
                WorldChunk chunk = WorldChunkManager.Instance.Chunks[chunkCoords.x, chunkCoords.y];
                for(int q = 0; q < chunk.EnvironmentObjectsInChunk.Count; q++)
                {
                    if (chunk.EnvironmentObjectsInChunk[q].coords == NewTilesInRoom[x])
                    {
                        if(chunk.EnvironmentObjectsInChunk[q].GetType().Equals(typeof(ConstructableObjectInstance)))
                        {
                            newObjects.Add(chunk.EnvironmentObjectsInChunk[q] as ConstructableObjectInstance);
                        }
                    }
                }
            }
        }
        ObjectsInRoom.AddRange(newObjects);
    }


    void CheckForConstructablesNoLongerInRoom(List<Vector2Int> tilesInRoom)
    {
        List<ConstructableObjectInstance> newObjectsInRoom = new List<ConstructableObjectInstance>();
        for (int y = 0; y < ObjectsInRoom.Count; y++)
        {
            if (tilesInRoom.Contains(ObjectsInRoom[y].coords)==false)
            {
                newObjectsInRoom.Add(ObjectsInRoom[y] );
            }
        }
       
    }

    public string GetDetailsForRoom()
    {
        return "Room Size: " + tilesInRoom.Count + " tiles" ;
    }


    public bool DoesRoomHaveNeededObjects()
    {
        return RoomUtils.IsValid(this);
    }

    public string GetValidityDetailsForRoom(Room r)
    {
        bool isValid = RoomUtils.IsValid(this);

        if (isValid)
        {
            return "True";
        }
        else
        {
            return isValid.ToString()+RoomUtils.GetValiditiyIssues(this);
        }
    }

    bool CanUseRoomValue = false;
    public virtual bool CanUseRoom()
    {
        return CanUseRoomValue;
    }
    

    public virtual void SetCanUseRoom(bool value)
    {
        CanUseRoomValue = value;
    }

    public bool DoesRoomContainPosition(Vector2Int coords)
    {
        return tilesInRoom.Contains(coords);
    }

    public void OnObjectAddedToRoom(ConstructableObjectInstance obj)
    {
        ObjectsInRoom.Add(obj);
    }


    public void RefreshRoom()
    {
        SetCanUseRoom(DoesRoomHaveNeededObjects());
    }
}

public enum RoomUseType 
{
    None,
    Barracks,
    Warehouse,
    Dwelling
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room:ISerialize
{
    public string roomName = "";
    public Color displayColour;
    public List<Vector2Int> tilesInRoom = new List<Vector2Int>();
    public bool Render = true,IsDrawn=false;
    Bounds roomBounds;
    RoomUseType roomUseType;
    public RoomUseType roomType
    {
        get
        {
            return roomUseType;
        }
        set
        {
            roomUseType = value;
            RefreshRoom();
        }
    }
    public List<Vector2Int> EdgeTiles,InvalidEdge;
    public static Action<Room> OnRoomChanged;

    public List<ConstructableObjectInstance> ObjectsInRoom = new List<ConstructableObjectInstance>();


    public Room(string name="Unnamed Room")
    {
        roomName = name;
        RefreshRoom();
        
    }

    bool HasInitRoom = false;
    void InitRoom()
    {
        
        displayColour = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), .25f);
        HasInitRoom = true;
    }

    public bool DoesRoomContainPoint(Vector2Int pos)
    {
        if (roomBounds == null)
        {
            return false;
        }
        if(roomBounds.Contains(new Vector3(pos.x, pos.y, 0.5f)))
        {
            Debug.Log("Room: bounds contains " + pos+" tiles "+  tilesInRoom.Contains(pos));
            return DoesRoomContainPosition(pos);
        }
        return false;
    }

    public void AddTiles(List<Vector2Int> tilesInRoom)
    {
        Vector3 toEncapsulate = new Vector3(0, 0, 0);

        if (!HasInitRoom)
        {
            InitRoom();
            toEncapsulate.x = tilesInRoom[0].x+.5f;
            toEncapsulate.y = tilesInRoom[0].y+.5f;
            toEncapsulate.z = tilesInRoom[0].y;

            roomBounds = new Bounds(toEncapsulate,new Vector3(1,1,909999));
            roomBounds.Encapsulate(toEncapsulate + Vector3.one);
            roomBounds.Encapsulate(toEncapsulate - Vector3.one);
        }


        List<Vector2Int> addedTiles = new List<Vector2Int>();
        for(int x=0;x<tilesInRoom.Count;x++)
        {
            if (!this.tilesInRoom.Contains(tilesInRoom[x]))
            {
                this.tilesInRoom.Add(tilesInRoom[x]);
                addedTiles.Add(tilesInRoom[x]);
                toEncapsulate.x = tilesInRoom[x].x;
                toEncapsulate.y = tilesInRoom[x].y;
                roomBounds.Encapsulate(toEncapsulate+Vector3.one);
                roomBounds.Encapsulate(toEncapsulate - Vector3.one);

            }
        }
        CheckForObjectsInRoom(addedTiles);

        CheckForItemsThatCouldBeInRoom(addedTiles);
        RoomManager.Instance.OnRoomChange?.Invoke(this);

    }


    void CheckForObjectsInRoom(List<Vector2Int> coords)
    {
        List<WorldChunk> chunksChecked = new List<WorldChunk>();
        for(int x=0;x<coords.Count;x++)
        {
            WorldChunk chunk = WorldChunkManager.Instance.GetWorldChunkFromTileCoords(coords[x]);
            if (!chunksChecked.Contains(chunk))
            {
                chunksChecked.Add(chunk);
            }
        }
        for(int x = 0; x < chunksChecked.Count; x++)
        {
            for(int y = 0; y < chunksChecked[x].EnvironmentObjectsInChunk.Count; y++)
            {
                ConstructableObjectInstance obj = chunksChecked[x].EnvironmentObjectsInChunk[y] as ConstructableObjectInstance;
                if (obj!=null && ObjectsInRoom.Contains(obj)==false && DoesRoomContainPoint(chunksChecked[x].EnvironmentObjectsInChunk[y].coords))
                {
                    OnObjectAddedToRoom(obj);
                }
            }
        }
    }

  
    public List<Unit> GetAllUnitsInRoom()
    {
        List<WorldChunk> chunksChecked = new List<WorldChunk>();
        Debug.Log("Hospital Update: tiles in room " + tilesInRoom.Count);

        for (int x = 0; x < tilesInRoom.Count; x++)
        {
            WorldChunk chunk = WorldChunkManager.Instance.GetWorldChunkFromTileCoords(tilesInRoom[x]);
            if (!chunksChecked.Contains(chunk))
            {
                chunksChecked.Add(chunk);
            }
        }
        List<Unit> retVal = new List<Unit>();
        for (int x = 0; x < chunksChecked.Count; x++)
        {
            for(int q= 0; q < chunksChecked[x].UnitsInChunk.Count; q++)
            {
                if (chunksChecked[x].UnitsInChunk[q].MyFaction.MyFactionID==FactionController.USER_FACTION 
                    && DoesRoomContainPoint(chunksChecked[x].UnitsInChunk[q].GetLastCoords()))
                {
                    retVal.Add(chunksChecked[x].UnitsInChunk[q]);
                }
            }
        }
        return retVal;
    }

    public void DrawRoomBounds()
    {
        DrawBounds(roomBounds, 0f);
    }
    void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }


    public void RemoveTiles(List<Vector2Int> tilesInRoom)
    {
        for(int i = 0; i < tilesInRoom.Count; i++)
        {
            this.tilesInRoom.Remove(tilesInRoom[i]);

        }
        CheckForConstructablesNoLongerInRoom(tilesInRoom);
        RoomManager.Instance.OnRoomChange?.Invoke(this);

    }

    void CheckForItemsThatCouldBeInRoom(List<Vector2Int> NewTilesInRoom)
    {
       // HashSet<Vector2Int> chunksChecked = new HashSet<Vector2Int>();

       // List<ConstructableObjectInstance> newObjects = new List<ConstructableObjectInstance>();
       // for(int x = 0; x < NewTilesInRoom.Count; x++)
       // {
       //     Vector2Int chunkCoords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(NewTilesInRoom[x]);
       //     if(!chunksChecked.Contains(chunkCoords))
       //     {
       //         WorldChunk chunk = WorldChunkManager.Instance.Chunks[chunkCoords.x, chunkCoords.y];
       //         for(int q = 0; q < chunk.EnvironmentObjectsInChunk.Count; q++)
       //         {
       //             if (chunk.EnvironmentObjectsInChunk[q].coords == NewTilesInRoom[x])
       //             {
       //                 if(chunk.EnvironmentObjectsInChunk[q].GetType().Equals(typeof(ConstructableObjectInstance)))
       //                 {
       //                     newObjects.Add(chunk.EnvironmentObjectsInChunk[q] as ConstructableObjectInstance);
       //                 }
       //             }
       //         }
       //     }
       // }
       // for(int x = 0; x < newObjects.Count; x++)
       // {
       //     string key = newObjects[x].ObjectKey;
       //     EnvironmentObject obj = EnvironmentObjectHelpers.GetEnvironmentObject(key);
       //     if (obj.CapacityData!=null&&obj.CapacityData.CapacityData.Count>0)
       //     {
       //         for(int q = 0; q < obj.CapacityData.CapacityData.Count; q++)
       //         {
       //             ResourceManager.Instance.UpdateResourceCapacity(obj.CapacityData.CapacityData[q].CapacityProvidedFor);
       //         }
       //     }
       // }
       // ResourceManager.Instance.UpdateResourceUI();
       //ObjectsInRoom.AddRange(newObjects);
    }


    void CheckForConstructablesNoLongerInRoom(List<Vector2Int> tilesInRoom)
    {
        string coords = "";
        for(int x=0;x<tilesInRoom.Count;x++)
        {
            coords += tilesInRoom[x].ToString() + ",";
        }

        List<ConstructableObjectInstance> newObjectsInRoom = new List<ConstructableObjectInstance>();
        for (int y = 0; y < ObjectsInRoom.Count; y++)
        {
            if (DoesRoomContainPoint(ObjectsInRoom[y].coords))
            {
                newObjectsInRoom.Add(ObjectsInRoom[y] );
            }
        }
        ObjectsInRoom = newObjectsInRoom;


        for (int x = 0; x < ObjectsInRoom.Count; x++)
        {
            string key = ObjectsInRoom[x].ObjectKey;
            EnvironmentObject obj = EnvironmentObjectHelpers.GetEnvironmentObject(key);
            if (obj.CapacityData != null && obj.CapacityData.CapacityData.Count > 0)
            {
                for (int q = 0; q < obj.CapacityData.CapacityData.Count; q++)
                {
                    ResourceManager.Instance.UpdateResourceCapacity(obj.CapacityData.CapacityData[q].CapacityProvidedFor);
                }
            }
        }
        ResourceManager.Instance.UpdateResourceUI();
        RoomManager.Instance.OnRoomChange?.Invoke(this);

    }

    public void OnObjectDestroyed(ConstructableObjectInstance obj)
    {
        if (ObjectsInRoom == null || !ObjectsInRoom.Contains(obj))
        {
            return;
        }
        ObjectsInRoom.Remove(obj);

        for (int x = 0; x < ObjectsInRoom.Count; x++)
        {
            string key = ObjectsInRoom[x].ObjectKey;
            EnvironmentObject envObj = EnvironmentObjectHelpers.GetEnvironmentObject(key);
            if (envObj.CapacityData != null && envObj.CapacityData.CapacityData.Count > 0)
            {
                for (int q = 0; q < envObj.CapacityData.CapacityData.Count; q++)
                {
                    ResourceManager.Instance.UpdateResourceCapacity(envObj.CapacityData.CapacityData[q].CapacityProvidedFor);
                }
            }
        }
        ResourceManager.Instance.UpdateResourceUI();
        RoomManager.Instance.OnRoomChange?.Invoke(this);

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
        if (CanUseRoomValue)
        {
            if (ObjectsInRoom != null)
            {
                for (int x = 0; x < ObjectsInRoom.Count; x++)
                {
                    string key = ObjectsInRoom[x].ObjectKey;
                    EnvironmentObject obj = EnvironmentObjectHelpers.GetEnvironmentObject(key);
                    if (obj.CapacityData != null && obj.CapacityData.CapacityData!=null && obj.CapacityData.CapacityData.Count > 0)
                    {
                        for (int q = 0; q < obj.CapacityData.CapacityData.Count; q++)
                        {
                            ResourceManager.Instance.UpdateResourceCapacity(obj.CapacityData.CapacityData[q].CapacityProvidedFor);
                        }
                    }
                }
            }
        }
        ResourceManager.Instance.UpdateResourceUI();
    }

    bool DoesRoomContainPosition(Vector2Int coords)
    {
        return tilesInRoom.Contains(coords);
    }

    public void OnObjectAddedToRoom(ConstructableObjectInstance obj)
    {
        
        ObjectsInRoom.Add(obj);
        
            string key = obj.Name();
            EnvironmentObject objData = EnvironmentObjectHelpers.GetEnvironmentObject(key);

        Debug.Log("Room: getting obj data from " + key+"|"+(objData==null)+"|"+(objData.CapacityData.CapacityData==null)+" is valid "+ CanUseRoom());
            if (objData.CapacityData != null && objData.CapacityData.CapacityData!=null && objData.CapacityData.CapacityData.Count > 0)
            {
                for (int q = 0; q < objData.CapacityData.CapacityData.Count; q++)
                {
                    ResourceManager.Instance.UpdateResourceCapacity(objData.CapacityData.CapacityData[q].CapacityProvidedFor);
                }
            }
        ResourceManager.Instance.UpdateResourceUI();
        RoomManager.Instance.OnRoomChange?.Invoke(this);
    }


    public void OnRoomDelete()
    {
        RemoveTiles(tilesInRoom);
    }


    public void RefreshRoom()
    {
        SetCanUseRoom(DoesRoomHaveNeededObjects()); 
        UnitCapacityManager.RefreshCapacities();
    }

   public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.RoomName, roomName);
        data.AddDataToSerialize(DataKeys.RoomType, (int)roomType);
        data.AddDataToSerialize(DataKeys.RoomTiles, tilesInRoom);
        return data;
    }

   

    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new NotImplementedException();
    }
    UID myUid;
    public UID GetMyUID()
    {
        if (myUid.Value == 0)
        {
            myUid = IDManager.GetUIDForObject();
            IDManager.OnUIDCreated(this, myUid);

        }
        return myUid;
    }

    public void SetMyUID(ulong uid)
    {
       myUid=new UID(uid);
        IDManager.OnUIDCreated(this, myUid);

    }
}

public enum RoomUseType 
{
    None,
    Barracks,
    Warehouse,
    Dwelling,
    Workshop,
    Hospital,
    RepairShop,
    Farm,
    Lab

}


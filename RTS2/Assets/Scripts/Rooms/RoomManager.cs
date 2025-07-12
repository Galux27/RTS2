using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    static RoomManager instance;
    public static RoomManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<RoomManager>(true);
            }
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
        LoadData();
        InitEvents();
        WallManager.OnWallAdded += OnWallSegmentCreated;
        WallManager.OnWallRemoved += OnWallSegmentRemoved;
        EnvironmentObjectManager.OnEnvironmentObjectDestroyed += OnEnvironmentObjectDestroyed;
    }

    public Dictionary<RoomUseType, RoomValidityData> ValidityData;

    const string ValidityDataPath = "RoomData";

    void LoadData()
    {
        ValidityData = new Dictionary<RoomUseType, RoomValidityData>();
        UnityEngine.Object[] data = Resources.LoadAll(ValidityDataPath);
        for (int x = 0; x < data.Length; x++)
        {
            RoomValidityData i = (RoomValidityData)data[x];
            if (ValidityData.ContainsKey(i.TypeToCheckFor) == false)
            {
                ValidityData.Add(i.TypeToCheckFor, i);
            }
        }
    }
    private void Update()
    {
        if (roomList != null)
        {
            for(int x = 0; x < roomList.Count; x++)
            {
                roomList[x].DrawRoomBounds();
            }
        }
    }
    void InitEvents()
    {
        EventManager.Instance.OnConstructableObjectCreated+= OnConstructableCreated;
    }

   public List<Room> roomList=new List<Room>();  

    public bool DoesAnyRoomContainPosition(Vector2Int pos)
    {
        for(int x = 0; x < roomList.Count; x++)
        {
            if (roomList[x].tilesInRoom.Contains(pos))
            {
                return true;
            }
        }

        return false;
    }

    public Action<Room> OnRoomAdded, OnRoomRemoved, OnRoomSelected,OnRoomChange;

    public void AddRoom(Room room)
    {
        roomList.Add(room);
        Debug.Log("Room: adding new room " + room.roomName + " of type " + room.roomType);
        RoomDrawrer.Instance.OnCreateRoom(room);
        OnRoomAdded?.Invoke(room);

    }

    public Room CreateRoom(List<Vector2Int> coordsForRoom)
   {
        Room r = new Room();
        r.AddTiles(coordsForRoom);
        roomList.Add(r);
        OnRoomAdded?.Invoke(r);
        RoomDrawrer.Instance.OnCreateRoom(r);
        return r;
    }

    public void SetSelectedRoom(Room r)
    {
        SelectedRoom = r;

        OnRoomSelected?.Invoke(r);
    }
    public Room SelectedRoom;
  
    public Room GetRoom()
    {
        return SelectedRoom;
    }

    public void DeleteSelected()
    {
        GetRoom().OnRoomDelete();
        roomList.Remove(GetRoom());
        RoomDrawrer.Instance.OnDestroyRoom(GetRoom());
        RoomsUIParent.Instance.RedrawRoomSelectionButtons();
        OnRoomRemoved?.Invoke(SelectedRoom);
        SelectedRoom = null;

    }

    void OnEnvironmentObjectDestroyed(EnvironmentObjectInstance destroyed)
    {
        ConstructableObjectInstance obj = destroyed as ConstructableObjectInstance;
        if (obj != null)
        {
            Vector2Int coords = obj.coords;
            for (int x = 0; x < roomList.Count; x++)
            {
                if (roomList[x].DoesRoomContainPoint(coords))
                {
                    roomList[x].OnObjectDestroyed(obj);
                }
            }

        }
    }
        public void OnConstructableCreated(Vector2Int coords, ConstructableObjectInstance Created)
    {
        Debug.Log("room: trying to Constructable added to room at " + coords+" "+Created.ObjectKey+" "+roomList.Count);

        for (int x=0;x< roomList.Count;x++)
        {
            if (roomList[x].DoesRoomContainPoint(coords))
            {
                Debug.Log("invalid: Constructable added to room at " + coords + "|" + roomList[x].roomType);
                roomList[x].OnObjectAddedToRoom(Created);
            }
        }
    }

    public void OnWallSegmentRemoved(Vector2Int coords)
    {
        for (int x = 0; x < roomList.Count; x++)
        {
            if (roomList[x].DoesRoomContainPoint(coords))
            {
                RoomUtils.IsRoomEnclosed(roomList[x]);
                Debug.Log("Room: wall Removed to room at " + coords + "|" + roomList[x].roomType);
                roomList[x].IsDrawn = false;
                OnRoomChange?.Invoke(roomList[x]);

            }
        }
    }

    public void OnWallSegmentCreated(Vector2Int coords)
    {
        for (int x = 0; x < roomList.Count; x++)
        {
            if (roomList[x].DoesRoomContainPoint(coords))
            {
                RoomUtils.IsRoomEnclosed(roomList[x]);
                Debug.Log("Room: wall added to room at " + coords + "|" + roomList[x].roomType);
                roomList[x].IsDrawn = false;
                OnRoomChange?.Invoke(roomList[x]);
            }
        }
    }

}

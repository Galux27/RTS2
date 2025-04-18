using System;
using System.Collections;
using System.Collections.Generic;
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
        LoadData();
        InitEvents();
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

    public Action<Room> OnRoomAdded, OnRoomRemoved, OnRoomSelected;

    public void AddRoom(Room room)
    {
        roomList.Add(room);
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
        OnRoomSelected?.Invoke(r);
        SelectedRoom = r;
    }
    public Room SelectedRoom;


    public void DeleteSelected()
    {
        SelectedRoom.OnRoomDelete();
        roomList.Remove(SelectedRoom);
        RoomDrawrer.Instance.OnDestroyRoom(SelectedRoom);
        SelectedRoom = null;
        RoomsUIParent.Instance.RedrawRoomSelectionButtons();
        OnRoomRemoved?.Invoke(SelectedRoom);

    }

    public void OnConstructableCreated(Vector2Int coords, ConstructableObjectInstance Created)
    {
        Debug.Log("trying to Constructable added to room at " + coords);

        for (int x=0;x< roomList.Count;x++)
        {
            if (roomList[x].DoesRoomContainPosition(coords))
            {
                Debug.Log("invalid: Constructable added to room at " + coords + "|" + roomList[x].roomType);
                roomList[x].OnObjectAddedToRoom(Created);
                roomList[x].RefreshRoom();
            }
        }
    }

}

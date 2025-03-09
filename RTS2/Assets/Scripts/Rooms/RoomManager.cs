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
        Object[] data = Resources.LoadAll(ValidityDataPath);
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

   public void CreateRoom(List<Vector2Int> coordsForRoom)
    {
        Room r = new Room();
        r.AddTiles(coordsForRoom);
        roomList.Add(r);
        RoomDrawrer.Instance.OnCreateRoom(r);
    }


    public Room SelectedRoom;


    public void DeleteSelected()
    {
        SelectedRoom.OnRoomDelete();
        roomList.Remove(SelectedRoom);
        RoomDrawrer.Instance.OnDestroyRoom(SelectedRoom);
        SelectedRoom = null;
        RoomsUIParent.Instance.RedrawRoomSelectionButtons();
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

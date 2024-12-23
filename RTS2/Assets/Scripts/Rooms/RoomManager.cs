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
        roomList.Remove(SelectedRoom);
        RoomDrawrer.Instance.OnDestroyRoom(SelectedRoom);
        SelectedRoom = null;
        RoomsUIParent.Instance.RedrawRoomSelectionButtons();
    }

}

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

   public void CreateRoom(List<Vector2Int> coordsForRoom)
    {
        Room r = new Room();
        r.AddTiles(coordsForRoom);
        roomList.Add(r);
    }


    public Room SelectedRoom;


    public void DeleteSelected()
    {
        roomList.Remove(SelectedRoom);
        SelectedRoom = null;
        RoomsUIParent.Instance.RedrawRoomSelectionButtons();
    }

}

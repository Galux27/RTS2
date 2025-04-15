using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButtonUIElement : BaseUIElement
{
    public Room Room;
    public TextMeshProUGUI RoomName;
    public Button SelectRoom, ZoomTo;

    public void InitButton(Room r)
    {
        Room = r;
        RoomName.text = r.roomName;
        SelectRoom.onClick.AddListener(OnSelectRoomClick);
        ZoomTo.onClick.AddListener(OnZoomToClick);
    }

    void OnSelectRoomClick()
    {
        RoomManager.Instance.SetSelectedRoom(Room);

        RoomDrawrer.Instance.CleanupAllRooms();
        RoomDrawrer.Instance.RenderAllRooms();
    }

    void OnZoomToClick()
    {
        if (Room.tilesInRoom.Count == 0)
        {
            return;
        }
        CameraController.Instance.SetToAutoMove(new Vector3(Room.tilesInRoom[0].x, Room.tilesInRoom[0].y));
    }
}

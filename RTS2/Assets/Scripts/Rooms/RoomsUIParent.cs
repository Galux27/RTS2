using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomsUIParent : MonoBehaviour
{
  static RoomsUIParent instance;
    public static RoomsUIParent Instance
    {
        get 
        {
            if(instance == null)
            {
                instance = FindObjectOfType<RoomsUIParent>(true);
            }
            return instance; 
        }
    }

    public Transform SelectRoomParent;
    public GameObject SelectRoomButton;
    public Button ExpandRoomButton,ReduceRoomButton, DeleteRoomButton;
    public TMP_InputField RoomName;
    public TextMeshProUGUI RoomDetails,IsValid;
    public TMPro.TMP_Dropdown RoomType;
    private void Awake()
    {
        RedrawRoomSelectionButtons();
        ExpandRoomButton.onClick.AddListener(() => RoomsSelectionMode.CurrentMode = RoomMode.Expand);
        ReduceRoomButton.onClick.AddListener(() => RoomsSelectionMode.CurrentMode = RoomMode.Remove);
        DeleteRoomButton.onClick.AddListener(RoomManager.Instance.DeleteSelected);
        RoomName.onValueChanged.AddListener( OnNameTextChanged);

        RoomType.options.Clear();
        List<string> TypeOptions = new List<string>();
        TypeOptions.Add(RoomUseType.None.ToString());
        TypeOptions.Add(RoomUseType.Barracks.ToString());
        TypeOptions.Add(RoomUseType.Warehouse.ToString());
        RoomType.AddOptions(TypeOptions);
        RoomType.onValueChanged.AddListener(OnRoomTypeChange);

    }

    private void OnNameTextChanged(string arg0)
    {
        if(RoomManager.Instance.SelectedRoom!= null)
        {
            RoomManager.Instance.SelectedRoom.roomName = arg0;
        }
    }

  

    public void RedrawRoomSelectionButtons()
    {
        for(int x=0;x<SelectRoomParent.childCount;x++)
        {
            Destroy(SelectRoomParent.GetChild(x).gameObject);
        }
        for (int i = 0;i<RoomManager.Instance.roomList.Count;i++)
        {
            DrawRoomButton(RoomManager.Instance.roomList[i]);
        }
        DrawNewRoomButton();
    }

    

    void DrawRoomButton(Room r)
    {
        GameObject room = Instantiate(SelectRoomButton, SelectRoomParent);
        room.GetComponent<Button>().onClick.AddListener(()=>SelectRoom(r));
        room.GetComponentInChildren<TextMeshProUGUI>().text = r.roomType.ToString();
    }

    void SelectRoom(Room r)
    {
        RoomManager.Instance.SelectedRoom = r;
        RoomDrawrer.Instance.RenderAllRooms();
        RoomName.text = r.roomName;
        RoomDetails.text = r.GetDetailsForRoom();
        IsValid.text = r.GetValidityDetailsForRoom();
        Debug.Log("Room: set current room ");
    }

    void OnRoomTypeChange(int i)
    {
        RoomManager.Instance.SelectedRoom.roomType = (RoomUseType)i;
        IsValid.text = RoomManager.Instance.SelectedRoom.GetValidityDetailsForRoom();
        RoomManager.Instance.SelectedRoom.SetCanUseRoom(RoomManager.Instance.SelectedRoom.DoesRoomHaveNeededObjects());
    }

    void DrawNewRoomButton()
    {
        GameObject room = Instantiate(SelectRoomButton, SelectRoomParent);
        room.GetComponent<Button>().onClick.AddListener(() => RoomManager.Instance.CreateRoom(new List<Vector2Int>()));
        room.GetComponent<Button>().onClick.AddListener(() => RedrawRoomSelectionButtons());
        room.GetComponentInChildren<TextMeshProUGUI>().text = "New Room";
    }
}

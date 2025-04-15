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


        ExpandRoomButton.onClick.AddListener(()=>OnSelectModeButton(ExpandRoomButton));
        ReduceRoomButton.onClick.AddListener(() => OnSelectModeButton(ReduceRoomButton));

        RoomName.onValueChanged.AddListener( OnNameTextChanged);

        RoomType.options.Clear();
        List<string> TypeOptions = new List<string>();
        TypeOptions.Add(RoomUseType.None.ToString());
        TypeOptions.Add(RoomUseType.Barracks.ToString());
        TypeOptions.Add(RoomUseType.Warehouse.ToString());
        TypeOptions.Add(RoomUseType.Dwelling.ToString());
        TypeOptions.Add(RoomUseType.Workshop.ToString());

        RoomType.AddOptions(TypeOptions);
        RoomType.onValueChanged.AddListener(OnRoomTypeChange);
        Room.OnRoomChanged += RefreshUI;
    }

    void OnSelectModeButton(Button b)
    {
        ExpandRoomButton.GetComponent<Image>().color = Color.white; 
        ReduceRoomButton.GetComponent<Image>().color = Color.white;
        b.GetComponent<Image>().color = Color.green;
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
        if (r == RoomManager.Instance.SelectedRoom)
        {
            room.GetComponent<Image>().color = Color.green;
        }
    }

    void SelectRoom(Room r)
    {
        RoomManager.Instance.SetSelectedRoom( r);
        RoomDrawrer.Instance.CleanupAllRooms();
        RoomDrawrer.Instance.RenderAllRooms();
        RefreshUI(r);
    }
    //create room first then adding furniture doesn't update contents of room
    void RefreshUI(Room r)
    {
        if (r == RoomManager.Instance.SelectedRoom)
        {
            RoomName.text = r.roomName;
            RoomDetails.text = r.GetDetailsForRoom();
            IsValid.text = r.GetValidityDetailsForRoom(r);
            RoomType.SetValueWithoutNotify((int)r.roomType);
            RoomDrawrer.Instance.CleanupRoom(r);
            RoomDrawrer.Instance.RenderRoom(r);
        }
        RedrawRoomSelectionButtons();
    }


    void OnRoomTypeChange(int i)
    {
        if (RoomManager.Instance.SelectedRoom==null)
        {
            return;
        }
        RoomManager.Instance.SelectedRoom.roomType = (RoomUseType)i;
        RoomManager.Instance.SelectedRoom.SetCanUseRoom(RoomManager.Instance.SelectedRoom.DoesRoomHaveNeededObjects());
        RefreshUI(RoomManager.Instance.SelectedRoom);

    }

    void DrawNewRoomButton()
    {
        GameObject room = Instantiate(SelectRoomButton, SelectRoomParent);
        room.GetComponent<Button>().onClick.AddListener(() => RoomManager.Instance.CreateRoom(new List<Vector2Int>()));
        room.GetComponent<Button>().onClick.AddListener(() => RedrawRoomSelectionButtons());
        room.GetComponentInChildren<TextMeshProUGUI>().text = "New Room";
    }
}

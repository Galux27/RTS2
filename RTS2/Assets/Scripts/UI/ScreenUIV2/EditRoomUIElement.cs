using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditRoomUIElement : BaseUIElement
{
    public Button ExpandRoom, ReduceRoom, DeleteRoom;
    public TextMeshProUGUI ValidityDetails,SelectedRoom;
    private void OnEnable()
    {
        DrawUI();
    }


    public override void DrawUI()
    {
        base.DrawUI(); 
        Init();
        RefreshUI();
    }

    bool init = false;
    void Init()
    {
        if (init)
        {
            return;
        }
        ExpandRoom.onClick.AddListener(() => RoomsSelectionMode.CurrentMode = RoomMode.Expand);
        ExpandRoom.onClick.AddListener(() => SetButtonVisuals(ExpandRoom));
        ReduceRoom.onClick.AddListener(() => RoomsSelectionMode.CurrentMode = RoomMode.Remove);
        ReduceRoom.onClick.AddListener(() => SetButtonVisuals(ReduceRoom));
        DeleteRoom.onClick.AddListener(RoomManager.Instance.DeleteSelected);
        RoomManager.Instance.OnRoomRemoved += RefreshOnRoomChange;
        RoomManager.Instance.OnRoomAdded += RefreshOnRoomChange;
        RoomManager.Instance.OnRoomSelected += RefreshOnRoomChange;
        RoomManager.Instance.OnRoomChange += RefreshOnRoomChange;
        init = true;
    }

    void RefreshOnRoomChange(Room r)
    {
        RefreshUI();
    }

    void SetButtonVisuals(Button selected)
    {
        ExpandRoom.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        ReduceRoom.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;


        if (selected != null)
        {
            selected.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        }
    }


    public override void RefreshUI()
    {
        base.RefreshUI();
        if(RoomManager.Instance.SelectedRoom == null)
        {
            SelectedRoom.text = "No room selected";
            ValidityDetails.text = "";
        }
        else
        {
            SelectedRoom.text = "Editing: " + RoomManager.Instance.SelectedRoom.roomName;
            ValidityDetails.text= RoomManager.Instance.SelectedRoom.GetValidityDetailsForRoom(RoomManager.Instance.SelectedRoom);
        }
    }

    public override void HideUI()
    {
        base.HideUI();
        RoomsSelectionMode.CurrentMode = RoomMode.None;
        SetButtonVisuals(null);
    }
}

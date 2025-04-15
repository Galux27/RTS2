using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class CreateNewRoomUIElement : BaseUIElement
{
    private void OnEnable()
    {
        DrawUI();
    }
    public override void DrawUI()
    {
        InitUI();
        base.DrawUI();
    }
    public CustomInputField NameInput;
    public Button CreateRoom;
    public CustomDropdown TypeDropDown;
    bool init = false;
    void InitUI()
    {
        if (init)
        {
            return;
        }
       
        TypeDropDown.CreateNewItem(RoomUseType.None.ToString(), null);
        TypeDropDown.CreateNewItem(RoomUseType.Barracks.ToString(), null);
        TypeDropDown.CreateNewItem(RoomUseType.Warehouse.ToString(), null);
        TypeDropDown.CreateNewItem(RoomUseType.Dwelling.ToString(), null);
        TypeDropDown.CreateNewItem(RoomUseType.Workshop.ToString(), null);

        CreateRoom.onClick.AddListener(OnCreateRoomClick);
        init = true;
    }

    void ResetInputFields()
    {
        NameInput.inputText.text = "";
        TypeDropDown.index = 0;
    }

    void OnCreateRoomClick()
    {
        Room r = new Room();
        r.tilesInRoom = new List<Vector2Int>();
        r.roomName = NameInput.inputText.ToString();
        r.roomType = (RoomUseType)TypeDropDown.index;
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.AddRoom(r);
        }
        ResetInputFields();
    }
}

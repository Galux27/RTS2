using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
public class RoomUI : BaseUIElement
{
    const string ButtonPool = "RoomButton";
    public Transform ButtonDisplayParent;
    public Button New, Expand, Contract, Delete;

    public TextMeshProUGUI CurrentRoomDisplay,ValidityDetails;
    public TMP_InputField NameInput;
    public TMP_Dropdown RoomType;
    const int MaxRoomTypes = 9;
    private void Awake()
    {
        RoomManager.Instance.OnRoomSelected += OnRoomSelected;
        RoomManager.Instance.OnRoomAdded+= OnRoomAdded;
        RoomManager.Instance.OnRoomRemoved += OnRoomDeleted;
        RoomManager.Instance.OnRoomChange += UpdateRoomValidityDetails;

        PopulateRoomTypeDropdown();
        New.onClick.AddListener(OnNewButtonPress);
        Expand.onClick.AddListener(OnExpandButtonPress);
        Contract.onClick.AddListener(OnContractButtonPress);
        Delete.onClick.AddListener(OnDeleteButtonPress);
    }

    void OnRoomTypeDropdownChanged(int newVal)
    {
        if (RoomManager.Instance.SelectedRoom != null)
        {
            RoomManager.Instance.SelectedRoom.roomType = (RoomUseType)RoomType.value;
            RoomManager.Instance.OnRoomChange?.Invoke(RoomManager.Instance.SelectedRoom);
        }
    }

    void PopulateRoomTypeDropdown()
    {
        List<string> options = new List<string>();
        for(int x = 0; x < MaxRoomTypes; x++)
        {
            options.Add(((RoomUseType)x).ToString());
        }
        RoomType.ClearOptions();
        RoomType.AddOptions(options);
        RoomType.onValueChanged.AddListener(OnRoomTypeDropdownChanged);
    }

    void OnNewButtonPress()
    {
        Room r = new Room();
        r.tilesInRoom = new List<Vector2Int>();
        r.roomName = NameInput.text.ToString();
        r.roomType = (RoomUseType)RoomType.value;
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.AddRoom(r,true);
        }
    }

    void OnExpandButtonPress()
    {
        if (RoomsSelectionMode.CurrentMode != RoomMode.Expand)
        {
            RoomsSelectionMode.CurrentMode = RoomMode.Expand;
        }
        else
        {
            RoomsSelectionMode.CurrentMode = RoomMode.None;

        }
    }

    void OnContractButtonPress()
    {
        if (RoomsSelectionMode.CurrentMode != RoomMode.Remove)
        {
            RoomsSelectionMode.CurrentMode = RoomMode.Remove;
        }
        else
        {
            RoomsSelectionMode.CurrentMode = RoomMode.None;

        }
    }

    void OnDeleteButtonPress()
    {
        RoomManager.Instance.DeleteSelected();
    }


    void OnRoomSelected(Room room)
    {
        if (room != null)
        {
            CurrentRoomDisplay.text = room.GetDetailsForRoom();
        }
        else
        {
            CurrentRoomDisplay.text = "No room selected";
        }
        UpdateRoomValidityDetails(room);
    }

    void UpdateRoomValidityDetails(Room room)
    {
        if (room != null)
        {
            ValidityDetails.text = room.GetValidityDetailsForRoom(room);
        }
        else
        {
            ValidityDetails.text = "";
        }
    }


    void OnRoomAdded(Room room)
    {
        RefreshUI();
    }

    void OnRoomDeleted(Room room)
    {
        RefreshUI();
    }

    List<GameObject> Buttons = new List<GameObject>();
    void Cleanup()
    {
        for(int x = 0; x < Buttons.Count; x++)
        {
            Buttons[x].transform.parent = null;
            Buttons[x].SetActive(false);
            GameObjectPoolManager.Instance.ReturnObjectToPool(Buttons[x], ButtonPool);
        }
        Buttons.Clear();
    }

    void GenerateButtonForRoom(Room r)
    {
        GameObject button = GameObjectPoolManager.Instance.GetObjectFromPool(ButtonPool);
        button.gameObject.transform.parent = ButtonDisplayParent;
        button.gameObject.SetActive(true);
        button.GetComponent<RoomButtonUIElement>().InitButton(r);
    }

    void RefreshUI()
    {
        Cleanup();
        for(int x = 0; x < RoomManager.Instance.roomList.Count; x++)
        {
            GenerateButtonForRoom(RoomManager.Instance.roomList[x]);
        }
        base.RefreshUI();
    }
}

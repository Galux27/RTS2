using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;
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
        UIEventManager.OnRoomEdited += OnRoomEdited;
        PopulateRoomTypeDropdown();
        New.onClick.AddListener(OnNewButtonPress);
        Expand.onClick.AddListener(OnExpandButtonPress);
        Contract.onClick.AddListener(OnContractButtonPress);
        Delete.onClick.AddListener(OnDeleteButtonPress);
        NameInput.onValueChanged.AddListener( OnNameTextChanged);
    }

    void OnRoomEdited(Room r)
    {
        if (r == RoomManager.Instance.SelectedRoom)
        {
            OnRoomSelected(r);
            
        }
        UnitMoniter.Instance.OnUnitCountsChanged();

    }
  
    private void OnEnable()
    {
        if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Rooms)
        {
            SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Rooms);
        }
    }

    void OnNameTextChanged(string text)
    {
        
        if (RoomManager.Instance.SelectedRoom != null)
        {
            RoomManager.Instance.SelectedRoom.roomName = text;
            RegenerateSpecificButton(RoomManager.Instance.SelectedRoom);
        }

    }



    void OnRoomTypeDropdownChanged(int newVal)
    {
        if (RoomManager.Instance.SelectedRoom != null)
        {
            RoomManager.Instance.SelectedRoom.roomType = (RoomUseType)RoomType.value;
            UIEventManager.OnRoomEdited?.Invoke(RoomManager.Instance.SelectedRoom);
            RegenerateSpecificButton(RoomManager.Instance.SelectedRoom);
        }
    }

    void PopulateRoomTypeDropdown()
    {
        RoomType.ClearOptions();

        List<string> options = new List<string>();
        for(int x = 0; x < MaxRoomTypes; x++)
        {
            Debug.Log("Adding room type option " + ((RoomUseType)x).ToString());
            options.Add(((RoomUseType)x).ToString());
        }
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

            NameInput.text = room.roomName;
        }
        else
        {
            CurrentRoomDisplay.text = "No room selected";
            NameInput.text = "";
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

    Dictionary<Room,GameObject> Buttons = new Dictionary<Room, GameObject>();
    void Cleanup()
    {
        foreach(KeyValuePair<Room,GameObject> pair in Buttons)
        {
            pair.Value.transform.parent = null;
            pair.Value.SetActive(false);
            GameObjectPoolManager.Instance.ReturnObjectToPool(pair.Value, ButtonPool);

        }
       
        Buttons.Clear();
    }

    void RegenerateSpecificButton(Room r)
    {
        Buttons[r].GetComponent<RoomButtonUIElement>().RefreshButton(r);
    }

    void GenerateButtonForRoom(Room r)
    {
        GameObject button = GameObjectPoolManager.Instance.GetObjectFromPool(ButtonPool);
        button.gameObject.transform.parent = ButtonDisplayParent;
        button.gameObject.SetActive(true);
        button.GetComponent<RoomButtonUIElement>().InitButton(r);
        Buttons.Add(r,button);
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

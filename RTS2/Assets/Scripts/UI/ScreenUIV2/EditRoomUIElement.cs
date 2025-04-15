using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Searcher;

public class EditRoomUIElement : BaseUIElement
{
    public Button ExpandRoom, ReduceRoom, DeleteRoom;
    public TextMeshProUGUI ValidityDetails;
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
        init = true;
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
            ValidityDetails.text = "No room selected";
        }
        else
        {
            ValidityDetails.text= RoomManager.Instance.SelectedRoom.GetValidityDetailsForRoom(RoomManager.Instance.SelectedRoom);
        }

        
    }
}

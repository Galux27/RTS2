using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsUIElement : BaseUIElement
{
    public MultiWindowUI RoomControls;
    public EditRoomUIElement EditRoomUI;
    public CreateNewRoomUIElement CreateNewRoomUI;
    public Transform RoomDisplayParent;
    public GameObject RoomButtonPrefab;
    private void Awake()
    {
        DrawUI();
    }


    public override void DrawUI()
    {
        Init();
        base.DrawUI();
        RoomControls.DrawUI();
        SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Rooms);
        RefreshUI();
    }
    bool init = false;
    void Init()
    {
        if(init)
        {
            return;
        }
        RoomManager.Instance.OnRoomAdded += RefreshDueToRoomChanges;
        RoomManager.Instance.OnRoomRemoved += RefreshDueToRoomChanges;
        init = true;
    }

    void RefreshDueToRoomChanges(Room r)
    {
        RefreshUI();
    }


    public override void HideUI()
    {
        RoomControls.HideUI();
        base.HideUI();
    }


    public override void RefreshUI()
    {
        base.RefreshUI();
        for(int x = 0; x < RoomDisplayParent.childCount; x++)
        {
            GameObject.Destroy(RoomDisplayParent.GetChild(x).gameObject);
        }


        for(int x=0;x<RoomManager.Instance.roomList.Count;x++)
        {
            GameObject button = GameObject.Instantiate(RoomButtonPrefab, RoomDisplayParent);
            button.GetComponent<RoomButtonUIElement>().InitButton(RoomManager.Instance.roomList[x]);
        }
    }
}

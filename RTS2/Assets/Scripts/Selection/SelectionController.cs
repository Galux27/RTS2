using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for controlling what should happen when the user clicks on objects in the world based on the mode they're in
/// </summary>
public class SelectionController : MonoBehaviour
{
    static SelectionController instance;
    public static SelectionController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType< SelectionController>(true);
            }
            return instance;
        }
    }
    public static Action<CurrentSelectionMode> OnSwitchSelectionMode;
    public CurrentSelectionMode selectionMode;
    public SelectionMode None, Units,Buildings,Construction, CurrentSelectionModeObj,Rooms;


    public void SetCursorSelectionMode(CurrentSelectionMode mode)
    {
        if (selectionMode == mode)
        {
            return;
        }
        OnCloseSelectionMode();
        selectionMode = mode;

        if (mode == CurrentSelectionMode.None)
        {
            CurrentSelectionModeObj = None;
        }
        else if (mode == CurrentSelectionMode.Units)
        {
            CurrentSelectionModeObj = Units;
        }
        else if (mode == CurrentSelectionMode.Furniture)
        {
            CurrentSelectionModeObj = Buildings;
            RoomDrawrer.Instance.RenderAllRooms();

        }
        else if (mode == CurrentSelectionMode.Structures)
        {
            CurrentSelectionModeObj = Construction;
            RoomDrawrer.Instance.RenderAllRooms();

        }
        else if (mode == CurrentSelectionMode.Rooms)
        {
            CurrentSelectionModeObj= Rooms;
            RoomDrawrer.Instance.RenderAllRooms();
        }
        SelectableManager.Instance.ClearSelectables();
        OnSwitchSelectionMode?.Invoke(mode);
    }

    void OnCloseSelectionMode()
    {
        ConstructableObjectManager.Instance.selectedToConstruct = null;
    }
    private void Awake()
    {
        None = new SelectionMode();
        Units=new Units_SelectionMode();
        CurrentSelectionModeObj = Units;
        Construction = new StructureSelectionMode();
        Buildings = new FurnitureSelectionMode();
        Rooms = new RoomsSelectionMode();
        selectionMode = CurrentSelectionMode.None;
    }
   
    private void Update()
    {

        if (ScreenUIUtilities.IsCursorOverUI())
        {
            return;
        }
        CursorSelect.Instance.UpdateSelectionPoints();

        if (CurrentSelectionModeObj == null)
        {
            return;
        }

        CurrentSelectionModeObj.OnHover();

        if (Input.GetMouseButtonUp(0))
        {
            CurrentSelectionModeObj.OnLeftMouseUp();
        }

        if (Input.GetMouseButtonUp(1))
        {
            CurrentSelectionModeObj.OnRightMouseUp();
        }
    }
}

public enum CurrentSelectionMode 
{ 
    None,
    Units,
    Furniture,
    Structures,
    Rooms
}


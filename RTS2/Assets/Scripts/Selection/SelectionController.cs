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
    public CurrentSelectionMode selectionMode;
    public SelectionMode None, Units,Buildings,Construction, CurrentSelectionModeObj,Rooms,Hybrid;
    public float LastLeftClick=-1,LastRightClick=-1;

    public void SetCursorSelectionMode(CurrentSelectionMode mode)
    {
       
        OnCloseSelectionMode();
        selectionMode = mode;

        if (mode == CurrentSelectionMode.None)
        {
            CurrentSelectionModeObj = Hybrid;
           ((Hybrid_SelectionMode) Hybrid).ResetSelected();
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
            ConstructableObjectManager.Instance.selectedToConstruct = null;
            CurrentSelectionModeObj = Construction;
            RoomDrawrer.Instance.RenderAllRooms();
        }
        else if (mode == CurrentSelectionMode.Rooms)
        {
            ConstructableObjectManager.Instance.selectedToConstruct = null;
            CurrentSelectionModeObj = Rooms;
            RoomDrawrer.Instance.RenderAllRooms();
        }
        SelectableManager.Instance.ClearSelectables();
        UIEventManager.OnSwitchSelectionMode?.Invoke(mode);
        Debug.Log("Set cursor selection mode " + mode.ToString());
    }

    void OnCloseSelectionMode()
    {

    }
    private void Awake()
    {
        None = new SelectionMode();
        Units=new Units_SelectionMode();
        Construction = new StructureSelectionMode();
        Buildings = new FurnitureSelectionMode();
        Rooms = new RoomsSelectionMode();
        Hybrid = new Hybrid_SelectionMode();

        CurrentSelectionModeObj = Hybrid;

        selectionMode = CurrentSelectionMode.None;
    }


   public float blockInputTimer = 0f;
   
    private void Update()
    {
        Debug.Log("Selection Controller: on hover current mode null " + (CurrentSelectionModeObj == null)+","+selectionMode.ToString());
        if (CursorSelect.Instance.IsMouseDown())
        {
            CursorSelect.Instance.UpdateSelectionPoints(!ScreenUIUtilities.IsCursorOverUI());
        }
        else
        {
            if (ScreenUIUtilities.IsCursorOverUI())
            {
                Debug.Log("Selection Controller: returning due to over UI");

                return;
            }
            CursorSelect.Instance.UpdateSelectionPoints(true);

        }
        if (CurrentSelectionModeObj == null)
        {
            return;
        }


       

        CurrentSelectionModeObj.OnHover();

        
        if(blockInputTimer > 0f) {
            blockInputTimer -= DeltaTimeWrapper.GameplayDelta;
            return;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            CurrentSelectionModeObj.OnLeftMouseUp();
            LastLeftClick = Time.time;
        }

        if (Input.GetMouseButtonUp(1))
        {
            CurrentSelectionModeObj.OnRightMouseUp();
            LastRightClick = Time.time;
        }
    }

    public float GetTimeSinceLastLeftClick()
    {
        return Time.time - LastLeftClick;
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


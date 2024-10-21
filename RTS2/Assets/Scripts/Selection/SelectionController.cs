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
    public SelectionMode None, Units, CurrentSelectionModeObj;




    private void Awake()
    {
        None = new SelectionMode();
        Units=new Units_SelectionMode();
        CurrentSelectionModeObj = Units; 
        selectionMode = CurrentSelectionMode.Units;
    }

    private void Update()
    {
        CursorSelect.Instance.UpdateSelectionPoints();


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
    Buildings
}


using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.UI;
public class SelectModeUI : MonoBehaviour
{

    public Button None, Units, Buildings,Construction,Rooms;


    public GameObject NoneUI, UnitsUI, BuildingUI, ConstructionUI,RoomsUI;
    private void Awake()
    {
        SelectionController.OnSwitchSelectionMode += OnChangeCursorMode;

        None.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.None); });
        Units.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Units); });
        Buildings.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Furniture); });
        Construction.onClick.AddListener(()=> { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Structures); });
        Rooms.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode (CurrentSelectionMode.Rooms); });
    }

    public void OnChangeCursorMode(CurrentSelectionMode switchedTo)
    {
        DisableUI();
        switch (switchedTo)
        {
            case CurrentSelectionMode.None:
                NoneUI.SetActive(true);
                break;
            case CurrentSelectionMode.Units:
                UnitsUI.SetActive(true);

                break;
            case CurrentSelectionMode.Furniture:
                FurnitureSelectButtonManager.Instance.RefreshUI();
              BuildingUI.SetActive(true);

                break;
            case CurrentSelectionMode.Structures:
                ConstructionUI.SetActive(true);

                break;
            case CurrentSelectionMode.Rooms:
                RoomsUI.SetActive(true);
                break;
            default:
                break;
        }
    }

    void DisableUI()
    {
        RoomDrawrer.Instance.CleanupRoom();
        NoneUI.SetActive(false);
        UnitsUI.SetActive(false);
        BuildingUI.SetActive(false);
        ConstructionUI.SetActive(false);
        RoomsUI.SetActive(false);

    }

}

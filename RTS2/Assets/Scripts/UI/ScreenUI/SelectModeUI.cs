using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.UI;
public class SelectModeUI : MonoBehaviour
{

    public Button None, Units, Buildings,Construction;


    public GameObject NoneUI, UnitsUI, BuildingUI, ConstructionUI;
    private void Awake()
    {
        SelectionController.OnSwitchSelectionMode += OnChangeCursorMode;

        None.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.None); });
        Units.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Units); });
        Buildings.onClick.AddListener(() => { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Buildings); });
        Construction.onClick.AddListener(()=> { SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Construction); });
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
            case CurrentSelectionMode.Buildings:
                BuildingSelectButtonManager.Instance.RefreshUI();
              BuildingUI.SetActive(true);

                break;
            case CurrentSelectionMode.Construction:
                ConstructionUI.SetActive(true);

                break;
            default:
                break;
        }
    }

    void DisableUI()
    {
        NoneUI.SetActive(false);
        UnitsUI.SetActive(false);
        BuildingUI.SetActive(false);
        ConstructionUI.SetActive(false);
    }

}

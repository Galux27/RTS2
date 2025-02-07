using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectButtonsManager : BaseUI
{
    static UnitSelectButtonsManager instance;
    public static UnitSelectButtonsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<UnitSelectButtonsManager>();
            }
            return instance;
        }
    }
    public GameObject ButtonPrefab;
    public Transform ButtonParent;
    private void Awake()
    {
        SelectableManager.OnSelectionChanged += RefreshUI;
        this.gameObject.SetActive(false);
    }

    public override void RefreshUI()
    {
        for (int x = 0; x < ButtonParent.transform.childCount; x++)
        {
            Destroy(ButtonParent.transform.GetChild(x).gameObject);
        }

        if (SelectableManager.Instance.CurrentSelectedType != SelectableType.Unit
            ||SelectionController.Instance.selectionMode!=CurrentSelectionMode.Units && SelectionController.Instance.selectionMode != CurrentSelectionMode.None)
        {
            this.gameObject.SetActive(false);
            return;
        }
       
        if (SelectableManager.Instance.CurrentlySelected.Count > 0)
        {
            this.gameObject.SetActive(true);

            Dictionary<UnitType, List<Unit>> units = SelectableManager.Instance.FilterUnitsByType();
            foreach (var item in units)
            {
                GameObject button = Instantiate(ButtonPrefab, ButtonParent);
                UnitSelectButton usb = button.GetComponent<UnitSelectButton>();
                usb.SetUnit(item.Key, item.Value.Count);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SelectedObjectsUI : BaseUI
{
    static SelectedObjectsUI instance;
    public static SelectedObjectsUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<SelectedObjectsUI>(true);
            }
            return instance;
        }
    }

    public TextMeshProUGUI InfoDisplay;
    public GameObject ButtonPrefab;
    public Transform ButtonParent;

    private void Awake()
    {
        SelectableManager.OnSelectionChanged += RefreshUI;

    }


    public void OpenUI()
    {
        this.gameObject.SetActive(true);
        RefreshUI();
    }

    public void CloseUI()
    {
        CleanupUI();
        this.gameObject.SetActive(false);
    }

    void CleanupUI()
    {
        for(int x=0;x< ButtonParent.transform.childCount;x++)
        {
            GameObject.Destroy(ButtonParent.transform.GetChild(x));
        }
    }

    public override void RefreshUI()
    {
        CleanupUI();
        switch (SelectableManager.Instance.CurrentSelectedType)
        {
            case SelectableType.None:
            case SelectableType.Unit:
                return;
                break;
            case SelectableType.Structure:
                DrawStructures();

                break;
            case SelectableType.ConstructableObject:
                DrawFurniture();
                break;
            case SelectableType.Item:
                DrawItems();
                break;
            case SelectableType.UnderConstructionObject:
                DrawUnderConstructionObjects();
                break;
            case SelectableType.Resource:
                DrawResources();
                break;
            default:
                break;
        }

        //Structures (walls & doors) that are built
        void DrawStructures()
        {
            
        }

        //Objects that can be built that are built
        void DrawFurniture()
        {

        }

        //objects & structures that are in the process of being built
        void DrawUnderConstructionObjects()
        {

        }

        //resources in the map that are yet to be harvested
        void DrawResources()
        {

        }

        //items that can be picked up
        void DrawItems()
        {

        }
    }


}

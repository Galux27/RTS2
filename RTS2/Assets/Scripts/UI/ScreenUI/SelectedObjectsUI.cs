using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using System.Xml.Serialization;
using UnityEditor.Rendering;

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
        CloseUI();
    }


    public void OpenUI()
    {
        this.gameObject.SetActive(true);
        RefreshUI();
    }

    public void CloseUI()
    {
        Debug.Log("Closing obj  UI" + SelectableManager.Instance.CurrentSelectedType);
        CleanupUI();
        this.gameObject.SetActive(false);
    }

    void CleanupUI()
    {
        for(int x=0;x< ButtonParent.transform.childCount;x++)
        {
            GameObject.Destroy(ButtonParent.transform.GetChild(x).gameObject);
        }
    }

    public override void RefreshUI()
    {
        CleanupUI();

        if (SelectableManager.Instance.CurrentSelectedType == SelectableType.Unit || SelectableManager.Instance.CurrentSelectedType == SelectableType.None || SelectableManager.Instance.CurrentlySelected.Count==0)
        {
            CloseUI();
            return;
        }
       


        RefreshBaseInfo();
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

      
    }
    Dictionary<string, SelectedObjectCategory> dataFromCurrent = new Dictionary<string, SelectedObjectCategory>();
    void RefreshBaseInfo()
    {
        dataFromCurrent.Clear();
        ObjectInfo oi = null;
        for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
        {
            oi = (ObjectInfo)SelectableManager.Instance.CurrentlySelected[x];
            if (oi != null)
            {
                if (dataFromCurrent.ContainsKey(oi.Name()) == false)
                {
                    dataFromCurrent.Add(oi.Name(), new SelectedObjectCategory(oi.Name()));
                }
                dataFromCurrent[oi.Name()].Increment(oi.Quantitiy());
            }
        }
    }
        void DrawButtons()
        {
            if (dataFromCurrent.Count == 0)
            {
                return;
            }
            if (dataFromCurrent.Count > 1)
            {
                foreach(KeyValuePair<string,SelectedObjectCategory> kvp in dataFromCurrent)
                {
                    GenerateFilterSelectedButton(kvp.Value);
                }
            }
            else
            {
                for(int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                {
                    GenerateSelectObjectButton(SelectableManager.Instance.CurrentlySelected[x] as ObjectInfo);
                }
            }
        }

        

        void GenerateFilterSelectedButton(SelectedObjectCategory category)
        {
            GameObject instance = Instantiate(ButtonPrefab, ButtonParent);
            instance.GetComponent<SelectedObjectButton>().InitAsFilter(category);
        }

       void GenerateSelectObjectButton(ObjectInfo oi)
        {
            GameObject instance = Instantiate(ButtonPrefab, ButtonParent);
            instance.GetComponent<SelectedObjectButton>().InitAsSelect(oi);
        }


        //Structures (walls & doors) that are built
        void DrawStructures()
        {
        DrawButtons();
        }

        //Objects that can be built that are built
        void DrawFurniture()
        {
        DrawButtons();

    }

    //objects & structures that are in the process of being built
    void DrawUnderConstructionObjects()
        {
        DrawButtons();

    }

    //resources in the map that are yet to be harvested
    void DrawResources()
        {
        DrawButtons();

    }

    //items that can be picked up
    void DrawItems()
        {
        DrawButtons();

    }
}


public class SelectedObjectCategory
{
    public string Key;
    public int Quantity;


    public SelectedObjectCategory(string key)
    {
        Key = key;
        Quantity = 0;
    }

    public void Increment(int val)
    {
        Quantity += val;
    }

}

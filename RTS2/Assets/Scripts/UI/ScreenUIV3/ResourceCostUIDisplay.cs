using UnityEngine;
using System.Collections.Generic;
public class ResourceCostUIDisplay : BaseUIElement
{
    const string ResourceRequiredDisplay = "ResourceRequired";
    public Transform resourcesRequiredParent;

    private void Awake()
    {
        UIEventManager.OnConstructableObjectSelected += OnConstructableSelectedToConstruct;
        UIEventManager.OnWallTileSelected += OnWallSelectedToConstruct;
    }

    void OnConstructableSelectedToConstruct(string selected)
    {
        if (ConstructableObjectManager.Instance.AllObjects.ContainsKey(selected))
        {
            Refresh(ConstructableObjectManager.Instance.AllObjects[selected].RequirementsToBuild);
        }
        else
        {
            Refresh(null);
        }
    }

    void OnWallSelectedToConstruct(WallTile selected)
    {
        if (selected != null)
        {
            Refresh(selected.RequirementsToBuild);
        }
        else
        {
            Refresh(null);
        }
    }
    List<GameObject> elements = new List<GameObject>();
    void Cleanup()
    {
        for(int x = 0; x < elements.Count; x++)
        {
            elements[x].transform.parent = null;
            elements[x].SetActive(false);
            GameObjectPoolManager.Instance.ReturnObjectToPool(elements[x], ResourceRequiredDisplay);
        }
        elements.Clear();
    }

    void Refresh(List<ResourceRequirement> RequirementsToBuild)
    {
        Cleanup();
        if (RequirementsToBuild == null)
        {
            return;
        }
        GameObject display = null;
        for (int x = 0; x < RequirementsToBuild.Count; x++)
        {
            display = GenerateResourceRequirementUI();
            display.GetComponent<ResourceCostUIDisplayElement>().Init(RequirementsToBuild[x].ResourceName, RequirementsToBuild[x].QuantityRequired);
        } 
    }

    GameObject GenerateResourceRequirementUI()
    {
        GameObject retVal = GameObjectPoolManager.Instance.GetObjectFromPool(ResourceRequiredDisplay);
        retVal.SetActive(true);
        retVal.transform.parent= resourcesRequiredParent;
        elements.Add(retVal);
        return retVal;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCostUI : BaseUIElement
{
    static ResourceCostUI instance;
    public static ResourceCostUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ResourceCostUI>(true);
            }
            return instance;
        }
    }

    void InitUI()
    {

    }
    public Transform ContentsParent;
    public GameObject ResourceUIPrefab;
    public Dictionary<string, ResourceUI> ResourceUIElements = new Dictionary<string, ResourceUI>();

    public void CreateUIElement(ResourceData toDraw)
    {
        if (ResourceUIElements.ContainsKey(toDraw.ResourceName))
        {
            return;
        }
        GameObject g = Instantiate(ResourceUIPrefab, ContentsParent);
        ResourceUI resource = g.GetComponent<ResourceUI>();
        resource.Init(toDraw.ResourceName, toDraw.Quantity, ResourceController.Instance.AllResources[toDraw.ResourceName].Item, ResourceManager.Instance.UserResources[toDraw.ResourceName].GetCapacity());
        ResourceUIElements.Add(toDraw.ResourceName, resource);
        g.SetActive(false);
    }

    public void UpdateUIElement(ResourceData toDraw)
    {
        if (!ResourceUIElements.ContainsKey(toDraw.ResourceName))
        {
            CreateUIElement(toDraw);
        }

        ResourceUIElements[toDraw.ResourceName].UpdateQuantity(toDraw.Quantity, ResourceManager.Instance.GetResourceCapacity(toDraw.ResourceName));
    }
    void HideAllUI()
    {
        foreach(KeyValuePair<string, ResourceUI> kvp in ResourceUIElements)
        {
            kvp.Value.gameObject.SetActive(false);
        }
    }
    public void UpdateUI(List<ResourceRequirement> reqs)
    {
        HideAllUI();
        for(int x=0;x< reqs.Count; x++)
        {
            Debug.Log("Updated requirements " + reqs[x].ResourceName + "," + reqs[x].QuantityRequired);
            ResourceUIElements[reqs[x].ResourceName].gameObject.SetActive(true);
            ResourceUIElements[reqs[x].ResourceName].UpdateRequirement(reqs[x].QuantityRequired);
        }
    }
}

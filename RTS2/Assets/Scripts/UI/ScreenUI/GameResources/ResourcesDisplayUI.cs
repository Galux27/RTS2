using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ResourcesDisplayUI : BaseUI
{
    static ResourcesDisplayUI instance;
    public static ResourcesDisplayUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ResourcesDisplayUI>(true);  
            }
            return instance;
        }
    }


    public Transform ContentsParent;
    public GameObject ResourceUIPrefab;

    public Dictionary<string, ResourceUI> ResourceUIElements=new Dictionary<string, ResourceUI>();
    public void CreateUIElement(ResourceData toDraw)
    {
        if (ResourceUIElements.ContainsKey(toDraw.ResourceName))
        {
            return;
        }
        GameObject g = Instantiate(ResourceUIPrefab,ContentsParent);
        ResourceUI resource=g.GetComponent<ResourceUI>();
        resource.Init(toDraw.ResourceName, toDraw.Quantity, ResourceController.Instance.AllResources[toDraw.ResourceName].Item);
        ResourceUIElements.Add(toDraw.ResourceName, resource);
    }

    public void UpdateUIElement(ResourceData toDraw)
    {
        ResourceUIElements[toDraw.ResourceName].UpdateQuantity(toDraw.Quantity);
    }
}

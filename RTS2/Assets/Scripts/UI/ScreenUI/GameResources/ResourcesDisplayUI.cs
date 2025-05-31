using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ResourcesDisplayUI : BaseUIElement
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
    private void Awake()
    {
        ResourceUIElements = new Dictionary<string, ResourceUI>();

    }

    public Transform ContentsParent;
    public GameObject ResourceUIPrefab;

    public Dictionary<string, ResourceUI> ResourceUIElements=new Dictionary<string, ResourceUI>();
    public void CreateUIElement(ResourceData toDraw)
    {
        if (ResourceUIElements.ContainsKey(toDraw.ResourceName))
        {
            Debug.Log("Resources: returning due to " + toDraw.ResourceName + " already existing");
            return;
        }
        GameObject g = Instantiate(ResourceUIPrefab,ContentsParent);
        ResourceUI resource=g.GetComponent<ResourceUI>();
        resource.Init(toDraw.ResourceName, toDraw.Quantity, ResourceController.Instance.AllResources[toDraw.ResourceName].Item);
        ResourceUIElements.Add(toDraw.ResourceName, resource);
        Debug.Log("Resources: created resource icon for "+  toDraw.ResourceName);

    }

    public void UpdateUIElement(ResourceData toDraw)
    {
        ResourceUIElements[toDraw.ResourceName].UpdateQuantity(toDraw.Quantity);
    }
}

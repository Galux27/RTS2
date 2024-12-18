using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSelectButtonManager : BaseUI
{
    static FurnitureSelectButtonManager instance;
    public static FurnitureSelectButtonManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FurnitureSelectButtonManager>(true);
            }
            return instance;
        }
    }
    public GameObject ButtonPrefab;
    public Transform ButtonParent;
    public override void RefreshUI()
    {
        for (int x = 0; x < ButtonParent.transform.childCount; x++)
        {
            Destroy(ButtonParent.transform.GetChild(x).gameObject);
        }
       foreach(KeyValuePair<string,ConstructableObject> kvp in ConstructableObjectManager.Instance.AllObjects)
        {
            GameObject button = GameObject.Instantiate(ButtonPrefab, ButtonParent);
            button.GetComponent<ConstructableSelectButton>().InitButton(kvp.Key);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUnitInit : MonoBehaviour
{

    public List<string> itemsToAdd;
    // Start is called before the first frame update
    void Start()
    {
        if(itemsToAdd!= null)
        {
            for(int x=0;x < itemsToAdd.Count;x++)
            {
                GetComponent<Human>().GetComponentInChildren<ItemHolder>().SetHolding(ItemInWorld.CreateItemInstanceInWorld(ItemController.Instance.AllItems[itemsToAdd[x]]));

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

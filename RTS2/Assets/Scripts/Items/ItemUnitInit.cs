using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to give items to a unit when its created
/// </summary>
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
                ItemInWorld iw = ItemInWorld.CreateItemInstanceInWorld(ItemController.Instance.AllItems[itemsToAdd[x]]);
                GetComponent<Inventory>().AddItemToInventory(iw);
                iw.EquipObject(this.GetComponent<Human>());
            }
        }

    }

   
}

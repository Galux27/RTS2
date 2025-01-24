using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryHelpers
{
    public static void TransferResourcesToContainer(Inventory toPutIn,Inventory toTransferFrom)
    {
        if (toPutIn.IsNotFull() == false)
        {
            return;
        }

        for (int x = 0; x < toTransferFrom.ObjectsInInventory.Count; x++)
        {
            if (ResourceController.Instance.AllResources.ContainsKey(toTransferFrom.ObjectsInInventory[x].Name()))
            {
                if (toPutIn.CanAddItemToInventory(toTransferFrom.ObjectsInInventory[x]))
                {
                    toPutIn.TransferItemBetweenInventory(toTransferFrom.ObjectsInInventory[x], toTransferFrom);
                }
            }
        }
    }

    public static bool DoesInventoryContainResource(Inventory toCheck)
    {
        for (int x = 0; x < toCheck.ObjectsInInventory.Count; x++)
        {
            if (ResourceController.Instance.AllResources.ContainsKey(toCheck.ObjectsInInventory[x].Name()))
            {
                return true;
            }
        }
        return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitTrainingHelpers
{
     public static void TurnUnitIntoOtherUnit(Unit existing,string typeToBecome)
    {
        UnitTypeSO toCreate = UnitTypesController.Instance.Units[typeToBecome];
        if(toCreate != null)
        {
            GameObject newUnit = GameObject.Instantiate(toCreate.Prefab,existing.transform.position,existing.transform.rotation);
            Unit u =newUnit.GetComponent<Unit>();
            CopyValuesToNewUnit(existing,ref u);
            existing.OnDeath();
        }
    }

    public static void CopyValuesToNewUnit(Unit old,ref Unit newUnit)
    {
        ObjectHealth health = newUnit.GetComponent<ObjectHealth>();
        health.MaxHealth = old.MaxHealth();
        health.CurrentHealth = old.Health();

        string equipedObject = "";
        if (old.GetComponent<ItemHolder>())
        {
            equipedObject = old.GetComponent<ItemHolder>().CurrentlyHolding.name;
        }
            Inventory i = newUnit.GetComponent<Inventory>();

        Inventory oldInv = old.GetComponent<Inventory>();
        i.InventoryCapacity = oldInv.InventoryCapacity;
        oldInv.CopyItemsIntoOtherInventory(ref i);

        UnitFaction faction = newUnit.GetComponent<UnitFaction>();
        faction.MyFactionID = old.MyFaction.MyFactionID;

        UnitSenses senses = newUnit.GetComponent<UnitSenses>();
        old.MySenses.CopyToNewSenses(ref senses);

        //copy behaviour?
    }
}

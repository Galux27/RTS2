using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InventoryObject
{
    public string Name();
    public float Weight();

    public int Quantity();

    public bool CanSplitStack();

    /// <summary>
    /// Splits the stack of items into 2, first contains the amount requested and second contains the remainder
    /// </summary>
    /// <param name="quantityWanted"></param>
    /// <returns></returns>
    public object[] SplitStack(int quantityWanted);
    public object[] SplitStack(float weightWanted);

    public void RepopulateData(InventoryObject toRepopulateWith);

    public void OnAddedToInventory();

    public void OnRemovedFromInventory();

    public bool CanObjectBeEquiped();

    public void EquipObject(Unit toEquipTo);
   

}

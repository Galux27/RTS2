using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUIElement :BaseUIElement
{
    public GameObject SelectedObjectsParent, SelectedObjectsInfoParent;

    public override void DrawUI()
    {
        base.DrawUI();
        RefreshUI();
    }


    public override void RefreshUI()
    {
        SelectedObjectsParent.SetActive(true);
    }
}

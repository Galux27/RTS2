using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsUIElement : BaseUIElement
{
    public GameObject RoomCreationControls, RoomInfoDisplay;

    public override void DrawUI()
    {
        base.DrawUI();
        RoomCreationControls.SetActive(true); 
        RoomInfoDisplay.SetActive(true);
    }

    public override void HideUI()
    {
        RoomCreationControls.SetActive(false);
        RoomInfoDisplay.SetActive(false);
        base.HideUI();
    }
}

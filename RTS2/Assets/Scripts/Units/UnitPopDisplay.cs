using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitPopDisplay :BaseUI
{
    public TextMeshProUGUI numberDisplay;
    public Image icon;

    public void InitUI(Color iconColour)
    {
        icon.color= iconColour;
        numberDisplay.text = "0/0";
    }

    public void UpdateValues(int val,int max)
    {
        numberDisplay.text = val + "/" + max;
    }
}

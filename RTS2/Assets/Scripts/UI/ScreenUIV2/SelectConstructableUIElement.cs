using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class SelectConstructableUIElement : BaseUIElement
{
    public Image Icon;
    public Button Button;

    public void SetupButton(string name, Action onClick,Sprite icon = null)
    {
        GetComponentInChildren<ButtonManagerBasic>().buttonText = name;
        Button.onClick.AddListener(() => onClick.Invoke()) ;
        Icon.sprite = icon;
    }
}

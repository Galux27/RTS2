using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class PauseMenuBarButtonUIelement : BaseUIElement
{
    public BaseUIElement PauseMenuElement;
    public ButtonManagerBasic OpenPauseMenu;

    private void Start()
    {
        OpenPauseMenu.gameObject.GetComponent<Button>().onClick.AddListener(() => { PauseMenuElement.DrawUI(); });
    }
}

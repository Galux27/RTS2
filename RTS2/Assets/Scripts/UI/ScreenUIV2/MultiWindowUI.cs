using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiWindowUI :  BaseUIElement
{
    public List<WindowButtonPair> buttons;

    public override void DrawUI()
    {
        Init();
        base.DrawUI();
    }

    bool init = false;
    void Init()
    {
        if(init)
        {
            return;
        }

        foreach(WindowButtonPair pair in buttons)
        {
            pair.button.onClick.AddListener(() => OnButtonClick(pair));
        }
        OnButtonClick(buttons[0]);

        init = true;
    }
    public Action<WindowButtonPair> OnSwitch;
    void OnButtonClick(WindowButtonPair window)
    {
        foreach (WindowButtonPair pair in buttons)
        {
            if (pair.window.GetComponent<BaseUIElement>() != null)
            {
                pair.window.GetComponent<BaseUIElement>().HideUI() ;
            }
            pair.window.SetActive(false);
            pair.OnIcon.SetActive(false);
            pair.OffIcon.SetActive(true);
            
        }

        window.window.SetActive(true);
        window.OffIcon.SetActive(false);
        window.OnIcon.SetActive(true);
        OnSwitch?.Invoke(window);
    }
}

[System.Serializable]
public class WindowButtonPair
{
    public Button button;
    public GameObject OnIcon, OffIcon;
    public GameObject window;
}

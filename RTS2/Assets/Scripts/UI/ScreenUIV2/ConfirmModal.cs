using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmModal : BaseUIElement
{
    static ConfirmModal instance;
    public static ConfirmModal Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<ConfirmModal>(true);
            }
            return instance;
        }
    }

    public ButtonManagerBasic Confirm,Cancel;

    public TextMeshProUGUI MessageDisplay, TitleDisplay;

    public void DisplayConfirmModal(string message,string title,Action onConfirm,Action onCancel)
    {
        TitleDisplay.text = title;
        MessageDisplay.text = message;
        Button button = Confirm.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(()=>onConfirm.Invoke());
        button.onClick.AddListener(HideUI);

        button = Cancel.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onCancel.Invoke());
        button.onClick.AddListener(HideUI);
        this.DrawUI();

    }



}

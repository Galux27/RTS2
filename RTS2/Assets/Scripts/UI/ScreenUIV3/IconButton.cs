using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
public class IconButton : MonoBehaviour
{
    public Button MyButton;
    public Image IconImage;

    public Action OnClick;

    public IconButtonState DefaultState;
    protected IconButtonState CurrentState;

    protected void Awake()
    {
        MyButton.onClick.AddListener(ButtonClick);
        SetState(DefaultState);
    }

    void ButtonClick()
    {
        OnClick?.Invoke();
    }

    public void SetState(IconButtonState state)
    {
        IconImage.sprite = state.Icon;
        CurrentState = state;
    }
}

[System.Serializable]
public class IconButtonState
{
    public string StateName;
    public Sprite Icon;
}

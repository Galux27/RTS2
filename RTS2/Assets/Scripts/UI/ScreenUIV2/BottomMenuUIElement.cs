using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
using System;
public class BottomMenuUIElement : BaseUIElement
{
    public Button Construction, Selection, Rooms, World;
    public BaseUIElement ConstructionElement, SelectionElement, RoomsElement, WorldElement;
    BaseUIElement currentlyDrawn;
    public BottomMenuMode Mode;

    public void Start()
    {
        Mode= BottomMenuMode.None;
        InitButtons();
        DrawUI();
    }

    void InitButtons()
    {
        Construction.onClick.AddListener(()=>OpenUI(BottomMenuMode.Construction));
        Selection.onClick.AddListener(() => OpenUI(BottomMenuMode.Selection));
        Rooms.onClick.AddListener(() => OpenUI(BottomMenuMode.Rooms));
        World.onClick.AddListener(() => OpenUI(BottomMenuMode.World));
        OnSwitchMode += TileValidityUI.Instance.Cleanup;
        OnSwitchMode +=()=> ConstructableObjectManager.Instance.GetCursor().SetActive(false);
    }

    void SelectButton(Button b)
    {
        b.transform.GetChild(3).GetComponent<CanvasGroup>().alpha = 1f;
    }
    void DeselectButton(Button b)
    {
        b.transform.GetChild(3).GetComponent<CanvasGroup>().alpha = 0f;
    }
    public Action OnSwitchMode;
    void OpenUI(BottomMenuMode modeToBe)
    {
        DeselectButton(Construction);
        DeselectButton(Selection); 
        DeselectButton(Rooms);
        DeselectButton(World);
        if (modeToBe == Mode)
        {
            if (currentlyDrawn != null)
            {
                currentlyDrawn.HideUI();
            }
            currentlyDrawn = null;
            Mode = BottomMenuMode.None;
        }
        else
        {
            if (currentlyDrawn != null)
            {
                currentlyDrawn.HideUI();
            }
            Mode = modeToBe;
            switch (modeToBe)
            {

                case BottomMenuMode.None:
                    break;
                case BottomMenuMode.Construction:
                    ConstructionElement.DrawUI();
                    SelectButton(Construction);
                    currentlyDrawn = ConstructionElement;
                    break;
                case BottomMenuMode.Selection:
                    SelectionElement.DrawUI();
                    SelectButton(Selection);
                    currentlyDrawn = SelectionElement;
                    break;
                case BottomMenuMode.Rooms:
                    RoomsElement.DrawUI();
                    SelectButton(Rooms);
                    currentlyDrawn = RoomsElement;
                    break;
                case BottomMenuMode.World:
                    WorldElement?.DrawUI();
                    SelectButton(World);
                    currentlyDrawn = WorldElement;
                    break;
            }
        }
        OnSwitchMode?.Invoke();
    }
}

public enum BottomMenuMode
{
    None,
    Construction,
    Selection,
    Rooms,
    World
}

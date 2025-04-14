using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class BottomMenuUIElement : BaseUIElement
{
    public ButtonManager Construction, Selection, Rooms, World;
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
        Construction.gameObject.GetComponent<Button>().onClick.AddListener(()=>OpenUI(BottomMenuMode.Construction));
        Selection.gameObject.GetComponent<Button>().onClick.AddListener(() => OpenUI(BottomMenuMode.Selection));
        Rooms.gameObject.GetComponent<Button>().onClick.AddListener(() => OpenUI(BottomMenuMode.Rooms));
        World.gameObject.GetComponent<Button>().onClick.AddListener(() => OpenUI(BottomMenuMode.World));

    }

    void OpenUI(BottomMenuMode modeToBe)
    {
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
                    currentlyDrawn = ConstructionElement;
                    break;
                case BottomMenuMode.Selection:
                    SelectionElement.DrawUI();
                    currentlyDrawn = SelectionElement;
                    break;
                case BottomMenuMode.Rooms:
                    RoomsElement.DrawUI();
                    currentlyDrawn = RoomsElement;
                    break;
                case BottomMenuMode.World:
                    WorldElement.DrawUI();
                    currentlyDrawn = WorldElement;
                    break;
            }
        }
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

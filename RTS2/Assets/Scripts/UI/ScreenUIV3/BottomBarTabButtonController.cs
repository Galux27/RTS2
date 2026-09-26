using UnityEngine;

public class BottomBarTabButtonController : TabButtonParent
{

    private void Awake()
    {
        InitTabs();
    }

    public override void DrawUI()
    {
        if (!AreTabsInit)
        {
            InitTabs();

        }
    }

    public override void InitTabs()
    {
        InitSelectionButton();
        InitConstructionButton();
        InitZonesButton();
        for (int x = 0; x < MyTabs.Count; x++)
        {
            MyTabs[x].Tab.SetParent(this);
            MyTabs[x].Tab.SetContent(MyTabs[x].TabContent);
        }

        base.InitTabs();
    }


    void InitSelectionButton()
    {
        MyTabs[0].Tab.SetText(MyTabs[0].Text);
        MyTabs[0].Tab.index = 0;
        MyTabs[0].Tab.AddAction(true, ResetSelectionMode);
        MyTabs[0].Tab.AddAction(false, ResetSelectionMode);

    }

    void OnZonesSelected()
    {
        if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Rooms)
        {
            SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Rooms);
        }

       
    }

    void ResetSelectionMode()
    {
        if (SelectionController.Instance.selectionMode != CurrentSelectionMode.None)
        {
            SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.None);
        }
    }
  
   

    void InitConstructionButton()
    {
        MyTabs[1].Tab.SetText(MyTabs[1].Text);
        MyTabs[1].Tab.index = 1;
        MyTabs[0].Tab.AddAction(true, ResetSelectionMode);
        MyTabs[0].Tab.AddAction(false, ResetSelectionMode);
    }

   

    void InitZonesButton()
    {
        MyTabs[2].Tab.SetText(MyTabs[2].Text);
        MyTabs[2].Tab.index = 2;
        MyTabs[2].Tab.AddAction(false, OnZonesSelected);
        MyTabs[2].Tab.AddAction(true, ResetSelectionMode);

    }
}

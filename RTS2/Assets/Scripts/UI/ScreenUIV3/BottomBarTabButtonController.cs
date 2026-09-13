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
        InitWorldButton();
        for (int x = 0; x < MyTabs.Count; x++)
        {
            MyTabs[x].Tab.SetParent(this);
        }

        base.InitTabs();
    }


    void InitSelectionButton()
    {
        MyTabs[0].Tab.SetText(MyTabs[0].Text);
        MyTabs[0].Tab.index = 0;
    }

    void InitConstructionButton()
    {
        MyTabs[1].Tab.SetText(MyTabs[1].Text);
        MyTabs[1].Tab.index = 1;

    }

    void InitWorldButton()
    {
        MyTabs[3].Tab.SetText(MyTabs[3].Text);
        MyTabs[3].Tab.index = 3;

    }

    void InitZonesButton()
    {
        MyTabs[2].Tab.SetText(MyTabs[2].Text);
        MyTabs[2].Tab.index = 2;

    }
}

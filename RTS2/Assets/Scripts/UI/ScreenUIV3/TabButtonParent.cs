using System.Collections.Generic;
using UnityEngine;

public class TabButtonParent : BaseUIElement
{
    public List<TabButtonInstance> MyTabs;
    protected bool AreTabsInit = false;
    public virtual void InitTabs()
    {
        AreTabsInit = true;
        
    }

    public void OnTabSelected(int index,bool wasActive)
    {
        if (wasActive)
        {
            for (int x = 0; x < MyTabs.Count; x++)
            {
                if (x != index)
                {

                    MyTabs[x].Tab.SetInactive();
                }
            }
        }
        else
        {
            for (int x = 0; x < MyTabs.Count; x++)
            { 
                 MyTabs[x].Tab.SetInactive();               
            }
        }

       
    }

}
[System.Serializable]
public class TabButtonInstance
{
    public TabButton Tab;
    public string Text;
}

using UnityEngine;
using System;
using System.Collections.Generic;
public static class UIEventManager
{
    public static Action<Selectable> OnObjectSelected,OnObjectDeselected;
    public static Action<List< Selectable>> OnObjectsSelected;
    
}

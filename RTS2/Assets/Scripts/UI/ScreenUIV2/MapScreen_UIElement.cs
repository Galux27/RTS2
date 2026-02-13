using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen_UIElement : BaseUIElement
{
    static MapScreen_UIElement instance;
    public static MapScreen_UIElement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapScreen_UIElement>(true); 
            }
            return instance;
        }
    }

    public RawImage MapImage;

    public void SetMapImage(Texture2D mapImage)
    {
        MapImage.texture= mapImage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBoundsGetter 
{
   public static Bounds GetBoundsOfObject(GameObject obj)
    {
        Bounds retVal = new Bounds();
        SpriteRenderer[] spriteRenderers = obj.transform.GetChild(0).GetComponentsInChildren<SpriteRenderer>();
        for(int x=0; x<spriteRenderers.Length;x++)
        {
            retVal.Encapsulate(spriteRenderers[x].bounds);
        }
       
        return retVal;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrderController : MonoBehaviour
{
    const int SortingLayersPerMeter = 10;
    SpriteRenderer sr;
    public bool IsStatic = false;
    const int MaxLayer = 32700;
    private void Awake()
    {
        sr= GetComponent<SpriteRenderer>();
    }

   
    


  
}

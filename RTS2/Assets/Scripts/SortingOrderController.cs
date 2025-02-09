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

    float lastY;
    private void Update()
    {
        if (IsStatic)
        {
            return;
        }
        if (lastY != this.transform.position.y)
        {
            OnPositionChange();
            lastY = this.transform.position.y;
        }
    }


    public void OnPositionChange()
    {
        int sortingOrder = Mathf.RoundToInt(this.transform.position.y * SortingLayersPerMeter);
        sr.sortingOrder= MaxLayer - sortingOrder;
    }

    private void OnEnable()
    {
        OnPositionChange();
    }
}

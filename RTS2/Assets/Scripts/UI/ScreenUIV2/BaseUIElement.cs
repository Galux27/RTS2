using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BaseUIElement : MonoBehaviour
{
    bool isDrawn = false;
    public Action OnDraw, OnHide;
    public virtual bool IsDrawn()
    {
        return isDrawn;
    }

    public virtual void DrawUI()
    {
        OnDraw?.Invoke();
        isDrawn = true;
        this.gameObject.SetActive(true);
    }

    public virtual void HideUI()
    {
        OnHide?.Invoke();
        isDrawn = false;
        this.gameObject.SetActive(false);
    }

    public virtual void RefreshUI()
    {

    }

}

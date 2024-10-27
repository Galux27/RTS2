using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    const float MaxValWidth = 100f;
    [SerializeField]GameObject Bar;
    ObjectHealth health;
    public void LinkToHealth(ObjectHealth toDisplay)
    {
        health = toDisplay;
        this.transform.parent = toDisplay.gameObject.transform;
        this.transform.localPosition = new Vector3(0, 1.6f, 0);
    }


    public void UpdateHealth()
    {
        RectTransform rectTransform = Bar.GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.InverseLerp(0, health.MaxHealth, health.CurrentHealth) * MaxValWidth);
    }
}

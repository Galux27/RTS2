using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    const float MaxValWidth = 100f;
    [SerializeField]GameObject Bar;
    ObjectHealth health;
    ObjectInfo objectHealth;

    public void LinkToHealth(ObjectHealth toDisplay)
    {
        if(toDisplay==null) return;
        health = toDisplay;
        this.transform.parent = toDisplay.gameObject.transform;
        this.transform.localPosition = new Vector3(0, 1.6f, 0);
    }

    public void LinkToObjectInfo(ObjectInfo objectInfo)
    {
        objectHealth= objectInfo;
        UpdateHealth();
    }

    float GetHealth()
    {
        if (health != null)
        {
            return health.CurrentHealth;
        }else if (objectHealth != null)
        {
            return objectHealth.Health();
        }
        return 0f;
    }

    float GetMaxHealth()
    {
        if (health != null)
        {
            return health.MaxHealth;
        }
        else if (objectHealth != null)
        {
            return objectHealth.MaxHealth();
        }
        return 0f;
    }

    public void UpdateHealth()
    {
        RectTransform rectTransform = Bar.GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.InverseLerp(0,GetMaxHealth(), GetHealth()) * MaxValWidth);

    }
}

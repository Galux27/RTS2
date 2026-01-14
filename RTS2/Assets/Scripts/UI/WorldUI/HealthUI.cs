using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    const float MaxValWidth = 100f;
    [SerializeField]GameObject Bar;
    EntityHealth health;
    ObjectInfo objectHealth;

    public void LinkToHealth(EntityHealth toDisplay,GameObject parent)
    {
        if(toDisplay==null) return;
        health = toDisplay;
        this.transform.parent = parent.transform;
        toDisplay.OnHealthUpdate += OnUpdateHealth;
        this.transform.localPosition = new Vector3(0, 2.1f, 0);
    }

    public void Cleanup()
    {
        if (health != null)
        {
            health.OnHealthUpdate -= OnUpdateHealth;
            health = null;
        }
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

    void OnUpdateHealth(float val)
    {
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        RectTransform rectTransform = Bar.GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.InverseLerp(0,GetMaxHealth(), GetHealth()) * MaxValWidth);

    }

    private void OnDestroy()
    {
        Debug.LogError("Destroying health ui...");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth
{
    public float CurrentHealth, MaxHealth;
    public Action<float> OnHealthIncrease, OnHealthDecrease, OnHealthUpdate;
    public Action OnDeath;
  
    public void ForceHealthValues(float health, float max)
    {
        CurrentHealth = health;
        MaxHealth = max;
        OnHealthUpdate?.Invoke(CurrentHealth);
    }

    public void IncreaseHealth(float val)
    {
        CurrentHealth += val;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        OnHealthIncrease?.Invoke(CurrentHealth);
        OnHealthUpdate?.Invoke(CurrentHealth);

    }

    public void DecreaseHealth(float val)
    {
        CurrentHealth -= val;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
        OnHealthDecrease?.Invoke(CurrentHealth);
        OnHealthUpdate?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    HealthUI healthUI;
    
    public void OnObjectRender(GameObject parent)
    {
        if (healthUI == null)
        {
            GameObject healthBar = GameObjectPoolManager.Instance.GetObjectFromPool("WorldspaceHealthBar");
            if (healthBar == null)
            {
                Debug.LogError("Got null health bar");
                return;
            }
            healthUI = healthBar.GetComponent<HealthUI>();
            healthUI.gameObject.SetActive(true);
            healthUI.LinkToHealth(this,parent);
        }
    }

    public void OnObjectHidden(GameObject parent)
    {
        if (healthUI != null)
        {
            GameObjectPoolManager.Instance.ReturnObjectToPool(healthUI.gameObject, "WorldspaceHealthBar");
            //GameObject.Destroy(healthUI.gameObject);
        }
    }
}

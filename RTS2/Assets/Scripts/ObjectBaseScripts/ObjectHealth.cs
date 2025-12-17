using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectHealth : MonoBehaviour
{
    public float CurrentHealth, MaxHealth;
    public Action<float> OnHealthIncrease, OnHealthDecrease,OnHealthUpdate;
    public Action OnDeath;
    HealthUI healthUI;

    private void Awake()
    {
      
    }

    public void OnObjectRender()
    {
        if (healthUI == null)
        {
            healthUI = GameObjectPoolManager.Instance.GetObjectFromPool("WorldspaceHealthBar").GetComponent<HealthUI>();
            healthUI.gameObject.SetActive(true);
           // healthUI.LinkToHealth(this);
        }
    }

    public void OnObjectHidden()
    {
        if (healthUI != null)
        {
            GameObjectPoolManager.Instance.ReturnObjectToPool(healthUI.gameObject, "WorldspaceHealthBar");
            //GameObject.Destroy(healthUI.gameObject);
        }
    }

    public void ForceHealthValues(float health,float max)
    {
        CurrentHealth = health;
        MaxHealth = max;
        healthUI?.UpdateHealth();
        OnHealthUpdate?.Invoke(CurrentHealth);
    }

    public void IncreaseHealth(float val)
    {
        CurrentHealth += val;
        CurrentHealth= Mathf.Min(CurrentHealth, MaxHealth);
        OnHealthIncrease?.Invoke(CurrentHealth);
        healthUI?.UpdateHealth();
        OnHealthUpdate?.Invoke(CurrentHealth);

    }

    public void DecreaseHealth(float val)
    {
        CurrentHealth -= val;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
        OnHealthDecrease?.Invoke(CurrentHealth);
        healthUI?.UpdateHealth();
        OnHealthUpdate?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}

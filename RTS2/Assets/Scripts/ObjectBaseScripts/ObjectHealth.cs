using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectHealth : MonoBehaviour
{
    public float CurrentHealth, MaxHealth;
    public Action<float> OnHealthIncrease, OnHealthDecrease;
    public Action OnDeath;
    HealthUI healthUI;

    private void Awake()
    {
        healthUI = Instantiate(WorldspaceUIManager.Instance.WorldspaceHealthBar).GetComponent<HealthUI>();
        healthUI.LinkToHealth(this);
    }

    public void IncreaseHealth(float val)
    {
        CurrentHealth += val;
        CurrentHealth= Mathf.Min(CurrentHealth, MaxHealth);
        OnHealthIncrease?.Invoke(CurrentHealth);
        healthUI?.UpdateHealth();
    }

    public void DecreaseHealth(float val)
    {
        CurrentHealth -= val;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
        OnHealthDecrease?.Invoke(CurrentHealth);
        healthUI?.UpdateHealth();
        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}

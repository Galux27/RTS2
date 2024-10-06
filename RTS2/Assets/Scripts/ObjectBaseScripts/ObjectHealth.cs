using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectHealth : MonoBehaviour
{
    public float CurrentHealth, MaxHealth;
    public Action<float> OnHealthIncrease, OnHealthDecrease;

    public void IncreaseHealth(float val)
    {
        CurrentHealth += val;
        CurrentHealth= Mathf.Min(CurrentHealth, MaxHealth);
        OnHealthIncrease?.Invoke(CurrentHealth);
        
    }

    public void DecreaseHealth(float val)
    {
        CurrentHealth -= val;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
        OnHealthDecrease?.Invoke(CurrentHealth);
    }
}

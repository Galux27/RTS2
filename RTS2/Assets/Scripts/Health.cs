using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Health
{
    public float Health();
    public float MaxHealth();

    public void AdjustHealth(float value);

    public void OnDeath();
}

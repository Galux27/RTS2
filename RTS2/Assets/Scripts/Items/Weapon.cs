using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class Weapon : Item
{
    //melee or using gun as club
    public float AttackRate;
    public float AttackDamage;
    public float AttackRange;

    
    public bool IsRanged;
    public float FireMinRange,FireMaxRange;
    public float FireRate;
    public GameObject RangedProjectile;

}

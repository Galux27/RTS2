using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackController : MonoBehaviour
{
    public float AttackDamage, AttackRate,AttackRange;

    bool HasRanged = false;
    public float MinRange, MaxRange, RangedFireRate,RangedDamage;
    GameObject RangedProjectile;

    public void OnNewItem(ItemInWorld itemHeld) 
    {
        if (itemHeld.MyItem.GetType() == typeof(Weapon))
        {
            Debug.Log("Holding Weapon, updating attack values");
        }
    }


    public void SetAttackValues(float damage,float rate,float range)
    {
        AttackDamage = damage;
        AttackRate = rate;
        AttackRange = range;
    }

    public void SetRangedValues(float damage,float rate,float minRange,float maxRange)
    {
        RangedFireRate = rate;
        RangedDamage = damage;
        MinRange = minRange;
        MaxRange= maxRange;
    }
    
    
    
    float attackTimer = 0f;
    float rangedTimer = 0f;

    bool CanRangedAttack(GameObject target)
    {
        if (HasRanged == false)
        {
            return false;

        }
        float distToTarget = Vector3.Distance(this.transform.position, target.transform.position);
        return distToTarget <= MaxRange && distToTarget>=MinRange;

    }


    bool CanMeleeAttack(GameObject target)
    {
        return Vector3.Distance(this.transform.position, target.transform.position) <= AttackRange;
    }

    Vector3 DirectionToTarget(GameObject target)
    {
        return (target.transform.position-this.transform.position).normalized;
    }

    public void AttemptAttack(Unit attacking)
    {
        if (HasRanged)
        {
            rangedTimer -= Mathf.Max(Time.deltaTime, 1f / 60f);
            if (CanRangedAttack(attacking.gameObject))
            {
                GameObject g = Instantiate(RangedProjectile, this.transform.position, Quaternion.identity);
                Projectile p = g.GetComponent<Projectile>();
                p.SetMomentum(DirectionToTarget(attacking.gameObject), 20f, this.GetComponent<Unit>(),5f);
            }
        }

        if (!CanMeleeAttack(attacking.gameObject))
        {
            attackTimer = AttackRate;
            return;
        }

        attackTimer -= Mathf.Max( Time.deltaTime,1f/60f);
        if (attackTimer <= 0)
        {
            attacking.AttackUnit(AttackDamage);
            attackTimer = AttackRate;
        }
    }
}

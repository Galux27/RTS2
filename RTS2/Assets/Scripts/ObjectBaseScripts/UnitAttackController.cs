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
           Weapon equiped = (Weapon)itemHeld.MyItem;
            SetAttackValues(equiped.AttackDamage,equiped.AttackRate,equiped.AttackRange);
            if (equiped.IsRanged)
            {
                SetRangedValues(5,equiped.FireRate,equiped.FireMinRange,equiped.FireMaxRange,equiped.RangedProjectile);
            }
            else
            {
                HasRanged = false;
            }
        }
    }


    public void SetAttackValues(float damage,float rate,float range)
    {
        AttackDamage = damage;
        AttackRate = rate;
        AttackRange = range;
    }

    public void SetRangedValues(float damage,float rate,float minRange,float maxRange,GameObject projectile)
    {
        HasRanged= true; 
        RangedFireRate = rate;
        RangedDamage = damage;
        MinRange = minRange;
        MaxRange= maxRange;
        this.RangedProjectile = projectile;
    }
    
    
    
    float attackTimer = 0f;
    float rangedTimer = 0f;


    public bool CanIAttackTarget(GameObject target)
    {
        return CanRangedAttack(target) || CanMeleeAttack(target);
    }

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
            rangedTimer -= Time.deltaTime;
            if (CanRangedAttack(attacking.gameObject) && rangedTimer<=0)
            {
                GameObject g = GameObjectPoolManager.Instance.GetObjectFromPool("Projectile");//Instantiate(RangedProjectile, this.transform.position, Quaternion.identity);
                g.transform.position = this.transform.position;
                g.transform.rotation = Quaternion.identity;
                Projectile p = g.GetComponent<Projectile>();
                g.SetActive(true);
                p.SetMomentum(DirectionToTarget(attacking.gameObject), 20f, this.GetComponent<Unit>(),5f);
                p.SetCreator(this.GetComponent<Unit>());
               
                rangedTimer = RangedFireRate;
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
            attacking.AttackUnit(AttackDamage,this.GetComponent<Unit>());
            attackTimer = AttackRate;
        }
    }
}

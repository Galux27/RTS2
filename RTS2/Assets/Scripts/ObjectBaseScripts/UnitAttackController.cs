using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Script to control units attacking other units in  a way that should work regardless of the weapon (ranged,melee etc...) they ahve
/// </summary>
public class UnitAttackController : MonoBehaviour
{
    public float AttackDamage, AttackRate,AttackRange;

    public bool HasRanged = false;
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

    public bool CanRangedAttack(Vector3 pos)
    {
        if (HasRanged)
        {
            return false;
        }
        float distToTarget = Vector3.Distance(this.transform.position, pos);
        return distToTarget <= MaxRange && distToTarget >= MinRange;
    }


    public bool CanRangedAttack(GameObject target)
    {
        return CanRangedAttack(target.transform.position);

    }

    public bool CanMeleeAttack(Vector3 pos)
    {
        return Vector3.Distance(pos, this.transform.position) <= AttackRange;
    }

   public bool CanMeleeAttack(GameObject target)
    {
        return CanMeleeAttack(target.transform.position);
    }

    public Vector3 DirectionToTarget(Vector3 pos)
    {
        return (pos - this.transform.position).normalized;
    }

    Vector3 DirectionToTarget(GameObject target)
    {
        return DirectionToTarget(target.transform.position);
    }

    public void AttemptAttack(ObjectInfo attacking)
    {
        if (HasRanged)
        {
            rangedTimer -= DeltaTimeWrapper.GameplayDelta;
            if (CanRangedAttack(attacking.Position()) && rangedTimer <= 0)
            {
                GameObject g = GameObjectPoolManager.Instance.GetObjectFromPool("Projectile");//Instantiate(RangedProjectile, this.transform.position, Quaternion.identity);
                g.transform.position = this.transform.position;
                g.transform.rotation = Quaternion.identity;
                Projectile p = g.GetComponent<Projectile>();
                g.SetActive(true);
                p.SetMomentum(DirectionToTarget(attacking.Position()), 20f, this.GetComponent<Unit>(), 5f);
                p.SetCreator(this.GetComponent<Unit>());

                rangedTimer = RangedFireRate;
            }
        }

        if (!CanMeleeAttack(attacking.Position()))
        {
            attackTimer = AttackRate;
            return;
        }

        attackTimer -= Mathf.Max(DeltaTimeWrapper.GameplayDelta, 1f / 60f);
        if (attackTimer <= 0)
        {
            attacking.AdjustHealth(-AttackDamage);
            attackTimer = AttackRate;
        }
    }

    public void AttemptAttack(Unit attacking)
    {
        if (HasRanged)
        {
            rangedTimer -= DeltaTimeWrapper.GameplayDelta;
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

        attackTimer -= Mathf.Max( DeltaTimeWrapper.GameplayDelta,1f/60f);
        if (attackTimer <= 0)
        {
            attacking.AttackUnit(AttackDamage,this.GetComponent<Unit>());
            attackTimer = AttackRate;
        }
    }
}

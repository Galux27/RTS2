using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackController : MonoBehaviour
{
    public float AttackDamage, AttackRate,AttackRange;

    float attackTimer = 0f;


    bool CanAttack(GameObject target)
    {
        return Vector3.Distance(this.transform.position, target.transform.position) <= AttackRange;
    }

    public void AttemptAttack(Unit attacking)
    {
        Debug.Log("Attempting attack");
        if (!CanAttack(attacking.gameObject))
        {
            Debug.Log("cant attack due to distance");

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

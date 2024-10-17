using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    UnitType ignoreHits;
    private void Awake()
    {
        rb= GetComponent<Rigidbody2D>();
    }
    float lifetime = 20f;
    float projectileDamage = 5f;
    public void SetMomentum(Vector3 direction,float force,Unit firing,float damage)
    {
        ignoreHits = firing.MyType;
        rb.AddForce(direction*force, ForceMode2D.Impulse);
        projectileDamage = damage;
    }
    private void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Unit>() != null)
        {
            Unit u = collision.gameObject.GetComponent<Unit>();
            if (u.MyType != ignoreHits)
            {
                OnHit(collision.gameObject);
            }
        }
        else
        {
            OnHit(collision.gameObject);
        }
    }

    void OnHit(GameObject hit)
    {
        if (hit.GetComponent<Unit>())
        {
            hit.GetComponent<Unit>().AttackUnit(projectileDamage);
        }
        Destroy(this.gameObject);
    }
}

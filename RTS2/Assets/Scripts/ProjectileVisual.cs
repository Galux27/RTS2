using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    LineRenderer lr;
    private void Awake()
    {
        lr = this.GetComponent<LineRenderer>();
    }
    Vector3 start, end,currentPos;
    float timer = 0f,timeLimit=0f;
    public void DisplayProjectileVisual(Vector3 startPos,Vector3 endPos,float duration)
    {

        start=startPos; end=endPos;
        currentPos = startPos;
        timer = duration;
        timeLimit = duration;
        lr.SetPosition(0,currentPos); 
        lr.SetPosition(1,currentPos);
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (timer < 0f)
        {
            return;
        }
        timer -= DeltaTimeWrapper.GameplayDelta;
        currentPos = Vector3.Lerp(start, end, Mathf.InverseLerp(timeLimit, 0f, timer));
        lr.SetPosition(0, currentPos);
        if (timer < 0f)
        {
            OnProjetileHitTarget();
        }
    }

    void OnProjetileHitTarget()
    {
        GameObjectPoolManager.Instance.ReturnObjectToPool(this.gameObject, "ProjectileVisual");
    }
}

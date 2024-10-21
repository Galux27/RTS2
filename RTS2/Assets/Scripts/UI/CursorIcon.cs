using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorIcon : MonoBehaviour
{
    static CursorIcon instance;
    public static CursorIcon Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<CursorIcon>();
            }
            return instance;
        }
    }


    public SpriteRenderer Icon;

    public Sprite Move, Attack, Build;


    public void SetMoveIcon()
    {
        Icon.sprite = Move;
    }

    public void SetAttackIcon()
    {
        Icon.sprite= Attack;
    }

    public void SetBuildIcon()
    {
        Icon.sprite= Build;
    }

    public void SetPosition(Vector3 pos)
    {
        this.transform.position= pos;
    }

    public void SetVisible(bool visible)
    {
        Icon.enabled = visible;
    }
}

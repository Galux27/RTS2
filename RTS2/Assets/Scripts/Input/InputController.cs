using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
   static InputController instance;
    public static InputController Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<InputController>();
            }
            return instance;
        }
    }
  public  float LastLeftClick=0f,LastRightClick=0f;
    public const float BlockInputLength = .02f;
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            LastLeftClick = Time.time;
        }

        if (Input.GetMouseButtonUp(1))
        {
            PriorRightClick = LastRightClick;
            LastRightClick = Time.time;
        }
    }

    public bool IsHoldingShortcutButton()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }

    public bool IsHoldingKey(KeyCode keyCode)
    {
        return Input.GetKey(keyCode);
    }

    public bool IsHoldingLeft()
    {
        return Input.GetKey(KeyCode.A);
    }

    public bool IsHoldingRight()
    {
        return Input.GetKey(KeyCode.D);
    }

    public bool IsHoldingUp()
    {
        return Input.GetKey(KeyCode.W);
    }
    public bool IsHoldingDown() 
    { 
        return Input.GetKey(KeyCode.S);
    }



    const float ClickLimit = .1f,DoubleClickLimit=.2f;
    public bool WasLastRightClickDoubleClick = false;
    public float PriorRightClick = 0f;
    public bool IsPressingRightMouse(out bool isDoubleClick)
    {
        isDoubleClick = false;
        if (Time.time-LastRightClick < ClickLimit)
        {
            Debug.Log("Actions: right click check" + (Time.time - LastRightClick));

            if (Time.time-PriorRightClick < DoubleClickLimit)
            {
                isDoubleClick = true;
            }
           
            WasLastRightClickDoubleClick = isDoubleClick;
            return true;
        }
        return false;
    }


    public KeyCode GetShortcutFromType(Type type)
    {
        if (type == typeof(HumanBehaviour_ConstructObject))
        {
            return KeyCode.C;
        }else if(type==typeof(HumanAttackUnit_Behaviour))
        {
            return KeyCode.A;
        }
        else if (type == typeof(GatherResources_Behaviour))
        {
            return KeyCode.G;
        }else if(type == typeof(HumanBehaviour_DeconstructObject))
        {
            return KeyCode.D;
        }
        else if (type == typeof(CollectResources_Behaviour))
        {
            return KeyCode.F;
        }

        return KeyCode.None;
    }

}

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

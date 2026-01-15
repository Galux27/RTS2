using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Unit Visual", menuName = "UnitVisuals/New Unit Visual", order = 1)]
public class UnitVisual : ScriptableObject
{
    public VisualType type;
    public string ID;
    public Sprite Front, Back, Side;
    public ColourPalleteCollection ColourPalletes;

    public Sprite GetDirectionalSprite(UnitVisualDirection direction)
    {
        switch(direction)
        {
            case UnitVisualDirection.Left:
                return Side;
                case UnitVisualDirection.Right:
                return Side;
            case UnitVisualDirection.Forward:
                return Back;
            case UnitVisualDirection.Backward:
                return Front;
        }
        return Front;
    }

    public static UnitVisualDirection CalculateDirection(Vector3 oldPos,Vector3 newPos)
    {
        float xTransformation = newPos.x - oldPos.x;
        float yTransformation = newPos.y - oldPos.y;
        if(Mathf.Abs(xTransformation) > Mathf.Abs(yTransformation))
        {
            if (xTransformation < 0)
            {
                return UnitVisualDirection.Left;
            }
            else
            {
                return UnitVisualDirection.Right;
            }
        }
        else
        {
            if(yTransformation > 0)
            {
                return UnitVisualDirection.Forward;

            }
            else
            {
                return UnitVisualDirection.Backward;

            }
        }
    }
}

public enum UnitVisualDirection{
    Forward,
    Backward,
    Left,
    Right
}

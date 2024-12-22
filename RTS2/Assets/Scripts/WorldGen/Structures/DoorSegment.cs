using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorSegment : WallSegment
{
    public DoorSegment(int x,int y,Tilemap toPlaceOn) : base(x, y)
    {
        WallType = WallType.Door;

        myAnim = WallHelpers.GetDoorVisual(this, WorldController.Instance.WallManager);
        DoorAnimator=new TilemapAnimator(myAnim,toPlaceOn,new Vector3Int(x,y,0));
        DoorAnimator.OnEnd += OnAnimEnd;
    }
    TilemapAnimation myAnim;
    public TilemapAnimator DoorAnimator;
    public int UnitsInTile = 0;
    public DoorState currentState=DoorState.Closed;

    public void UnitEnterDoor(Unit onTile)
    {
        UnitsInTile++;
        Debug.Log("Door: Unit " + onTile.gameObject.name + " entering door " + onTile.MySenses.Intelligence + " " + currentState.ToString() + " " + UnitsInTile);
        if (UnitCanUseDoor(onTile))
        {
            OpenDoor();
        }
    }

    public void UnitExitDoor(Unit onTile)
    {
        if (UnitsInTile > 0)
        {
            UnitsInTile--;
        }
        Debug.Log("Door: Unit " + onTile.gameObject.name + " xiting door " + onTile.MySenses.Intelligence + " " + currentState.ToString() + " " + UnitsInTile);

        if (UnitCanUseDoor(onTile)&& UnitsInTile==0)
        {
            CloseDoor();
        }
    }

    public bool UnitCanUseDoor(Unit toUse)
    {
        return toUse.MySenses.Intelligence >= 50;
    }

    public bool NeedToOpenDoor()
    {
        return currentState == DoorState.Closed || currentState == DoorState.Closing;
    }

    public bool NeedToCloseDoor()
    {
        return currentState == DoorState.Open || currentState == DoorState.Opening;
    }

    public void OpenDoor()
    {
        Debug.Log("Door: open door " + NeedToOpenDoor());

        if (NeedToOpenDoor()==false)
        {
            return;
        }
        currentState = DoorState.Opening;
        DoorAnimator.Reverse = false;
        DoorAnimator.StartAnimation();
        Collider.SetActive(false);
    }

    public void CloseDoor()
    {
        Debug.Log("Door: closing door " + NeedToCloseDoor());
        if (NeedToCloseDoor()==false)
        {
            return;
        }
        currentState = DoorState.Closing;
        DoorAnimator.Reverse = true;
        DoorAnimator.StartAnimation();
    }

    public override void DestroyWall()
    {
        base.DestroyWall();
        Pathfinding.RemovePathModifier(x, y, "Door");
    }

    void OnAnimEnd()
    {
        if(currentState == DoorState.Opening)
        {
            currentState = DoorState.Open;
        }else if (currentState == DoorState.Closing)
        {
            currentState = DoorState.Closed;
            Collider.SetActive(true);

        }
    }
}

public enum DoorState 
{ 
    Open,
    Closed,
    Opening,
    Closing
}


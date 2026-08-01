using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorSegment : WallSegment
{
    public DoorSegment(int x, int y, Tilemap toPlaceOn, WallTile wallType,int localX,int localY,bool render=true) : base(x, y,wallType,localX,localY)
    {
        WallType = WallType.Door;
        Debug.Log("Created door at " + x + "," + y + " render " + render);
        myAnim = WallHelpers.GetDoorVisual(this, WorldController.Instance.WallManager);
        DoorAnimator=new TilemapAnimator(myAnim,toPlaceOn,new Vector3Int(x,y,0),render);
        DoorAnimator.OnEnd += OnAnimEnd;
        SetTile(myAnim.AnimationFrames[0]);
    }
        TilemapAnimation myAnim;
    public TilemapAnimator DoorAnimator;
    public int UnitsInTile = 0;
    public DoorState currentState=DoorState.Closed;

    public void UnitEnterDoor(Unit onTile)
    {
        UnitsInTile++;
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
        if (UnitCanUseDoor(onTile)&& UnitsInTile==0)
        {
            CloseDoor();
        }
    }

    public bool UnitCanUseDoor(Unit toUse)
    {
        return true;
        return toUse.MySenses.Intelligence >= 50;
    }

    public override void UnRender()
    {
        Debug.Log("Un rendering wall at ");
        DoorAnimator.OnUnrender();
        base.UnRender();
    }
    public override void RenderWall()
    {
        Debug.Log("Rendering door at ");
        base.RenderWall();
        DoorAnimator.OnRender();
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
        if (NeedToOpenDoor()==false)
        {
            return;
        }
        currentState = DoorState.Opening;
        DoorAnimator.Reverse = false;
        DoorAnimator.StartAnimation();
     }

    public void CloseDoor()
    {
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


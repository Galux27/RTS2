using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapAnimator
{
    public TilemapAnimation Animation;
    public Tilemap ToEdit;
    public Vector3Int Coords;

    public float Timer = 0f;
    public bool IsAnimating = false,Reverse=false;
    int index = 0;
    


    public TilemapAnimator(TilemapAnimation animation,Tilemap toEdit, Vector3Int coords)
    {
        Animation = animation;
        ToEdit = toEdit;
        Timer = animation.TimePerFrame;
        Coords = coords;
        TilemapAnimationController.Instance.AddAnimator(this);
        UpdateTile();
    }

    public void Cleanup()
    {
        TilemapAnimationController.Instance.RemoveAnimatior(this);
    }


    public void StartAnimation()
    {
        IsAnimating = true;
        OnStart?.Invoke();
    }

    public void StopAnimation()
    {
        IsAnimating = false;
    }

    public void OnUpdate()
    {
        if (!IsAnimating)
        {
            return;
        }
        Timer -= Time.deltaTime;
        if (Timer <= 0f)
        {
            if (Reverse == false)
            {
                index++;
                if (index > Animation.AnimationFrames.Count - 2)
                {
                    StopAnimation();
                    OnEnd?.Invoke();
                }
            }
            else
            {
                index--;
                if (index < 1)
                {
                    StopAnimation();
                    OnEnd?.Invoke();
                }
            }
            UpdateTile();
            Timer = Animation.TimePerFrame;
        }
    }

    void UpdateTile()
    {
        index=Mathf.Clamp(index, 0, Animation.AnimationFrames.Count-1);
        ToEdit.SetTile(Coords, Animation.AnimationFrames[index]);
        OnFrameChange?.Invoke();
    }

    public Action OnStart, OnEnd, OnFrameChange;
}

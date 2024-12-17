using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapAnimationController : MonoBehaviour
{
    static TilemapAnimationController instance;
    public static TilemapAnimationController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<TilemapAnimationController>(true);
            }
            return instance;
        }
    }
    public List<TilemapAnimator> animators=new List<TilemapAnimator>();
    public void AddAnimator(TilemapAnimator animator)
    {
        animators.Add(animator);
    }

    public void RemoveAnimatior(TilemapAnimator animator)
    {
        animators.Remove(animator);
    }

    private void Awake()
    {
        LoadTilemapAnimations();
    }
  
    void Update()
    {
        for(int x=0; x<animators.Count; x++)
        {
            animators[x].OnUpdate();
        }   
    }


    public Dictionary<string, TilemapAnimation> Animations = new Dictionary<string, TilemapAnimation>();
    List<string> AnimationKeys;

    const string TilemapAnimDir = "TilemapAnimation";
    public void LoadTilemapAnimations()
    {
       
            Animations = new Dictionary<string, TilemapAnimation>();
            AnimationKeys = new List<string>();
            UnityEngine.Object[] items = Resources.LoadAll(TilemapAnimDir);
            for (int x = 0; x < items.Length; x++)
            {
                TilemapAnimation i = (TilemapAnimation)items[x];
                if (Animations.ContainsKey(i.AnimationID) == false)
                {
                    Animations.Add(i.AnimationID, i);
                    AnimationKeys.Add(i.AnimationID);
                }
            }
        

    }


}

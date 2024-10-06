using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemInWorld : MonoBehaviour
{
    public static ItemInWorld CreateItemInstanceInWorld(Item toCreate)
    {
        GameObject g = new GameObject();
        g.name = toCreate.name + " instance";
        ItemInWorld iw = g.AddComponent<ItemInWorld>();
        iw.SetItem(toCreate);
        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = toCreate.Sprite;
        iw.sr = sr;
        return iw;
    }
    private void Awake()
    {
        ItemController.Instance.AllItemsInWorld.Add(this);
    }

    public void SetItem(Item item)
    {
        MyItem = item;
    }

    public Item MyItem;
    public SpriteRenderer sr;
}

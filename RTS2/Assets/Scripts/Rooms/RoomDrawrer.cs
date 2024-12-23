using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDrawrer : MonoBehaviour
{
    static RoomDrawrer instance;

    public static RoomDrawrer Instance {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RoomDrawrer>();
            }
            return instance;
        }
    }

    public const string BaseTileKey="RoomUI";
    Transform drawParent;
    
    public Transform DrawingParent
    {
        get
        {
            if (drawParent == null)
            {
                drawParent = new GameObject().transform;
                drawParent.transform.parent = this.transform;
            }
            return drawParent;
        }
    }
    Dictionary<Room, GameObject> parentsOfRooms=new Dictionary<Room, GameObject>();
    public void RenderPoints(Transform parent, List<Vector2Int> coords, Color c =default)
    {
        CleanupRoom(parent);
        GameObject cur = null;
        for(int x=0; x<coords.Count; x++)
        {
            cur = GameObjectPoolManager.Instance.GetObjectFromPool(BaseTileKey);
            cur.transform.parent = parent;
            cur.SetActive(true);

            if (c != default)
            {
                cur.transform.position = new Vector3(coords[x].x + .5f, coords[x].y + .5f,0f);

                cur.GetComponent<SpriteRenderer>().color = c;
                cur.GetComponent<SpriteRenderer>().sortingOrder=98;
            }
            else
            {
                cur.transform.position = new Vector3(coords[x].x + .5f, coords[x].y + .5f, .1f);

                cur.GetComponent<SpriteRenderer>().color = new Color(0,1,1,.3f);
            }

        }
    }

    public void OnCreateRoom(Room r)
    {
        if(parentsOfRooms.ContainsKey(r)==false)
        {
            GameObject parent = new GameObject();
            parent.transform.parent = this.transform;
            parentsOfRooms[r] = parent;
        }
    }

    public void OnDestroyRoom(Room r)
    {
        if (parentsOfRooms.ContainsKey(r) )
        {
            CleanupRoom(parentsOfRooms[r].transform);
            Destroy(parentsOfRooms[r]);
            parentsOfRooms.Remove(r);
        }
    }

    public void RenderAllRooms()
    {
        for(int x = 0; x < RoomManager.Instance.roomList.Count; x++)
        {
            RenderRoom(RoomManager.Instance.roomList[x]);
        }
    }

    public void RenderRoom(Room r)
    {
        if (r==null)
        {
            return;
        }
        Debug.Log("Rendering room " + r.displayColour.ToString()+" tiles "+ r.tilesInRoom.Count);
        RenderPoints(parentsOfRooms[r].transform,r.tilesInRoom,r.displayColour);
    }

    public void CleanupAllRooms()
    {
        CleanupRoom(DrawingParent);
        foreach(KeyValuePair<Room,GameObject> kvp in parentsOfRooms)
        {
            CleanupRoom(kvp.Value.transform);
        }
    }

    public void CleanupRoom(Room r)
    {
        if (r == null)
        {
            return;
        }
        CleanupRoom(parentsOfRooms[r].transform);
    }

    public void CleanupRoom(Transform parent)
    {
        GameObject g = null;
        while(parent.childCount>0)
        {
            g = parent.GetChild(0).gameObject;
            g.transform.parent = null;
            GameObjectPoolManager.Instance.ReturnObjectToPool(g, BaseTileKey);
        }
    }
}

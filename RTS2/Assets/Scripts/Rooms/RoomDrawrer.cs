using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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
    public void RenderPoints(Transform parent,Room room, Color c = default)
    {
        CleanupRoom(parent);
        GameObject cur = null;
        if (room == RoomManager.Instance.SelectedRoom) { 
        for (int x = 0; x < room.tilesInRoom.Count; x++)
        {
            cur = GameObjectPoolManager.Instance.GetObjectFromPool(BaseTileKey);
            cur.transform.parent = parent;
            cur.SetActive(true);

          
            if (room.EdgeTiles != null && room.EdgeTiles.Contains(room.tilesInRoom[x]) )
            {
                cur.transform.position = new Vector3(room.tilesInRoom[x].x + .5f, room.tilesInRoom[x].y + .5f, 0f);

                if (room.InvalidEdge.Contains(room.tilesInRoom[x]))
                {
                    cur.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, .5f);

                }
                else
                {
                    cur.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, .5f);

                }
                cur.GetComponent<SpriteRenderer>().sortingOrder = 98;
            }
            else
            {
                if (c != default)
                {
                    cur.transform.position = new Vector3(room.tilesInRoom[x].x + .5f, room.tilesInRoom[x].y + .5f, 0f);

                    cur.GetComponent<SpriteRenderer>().color = c;
                    cur.GetComponent<SpriteRenderer>().sortingOrder = 98;
                }
                else
                {
                    cur.transform.position = new Vector3(room.tilesInRoom[x].x + .5f, room.tilesInRoom[x].y + .5f, .1f);

                    cur.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1, .3f);
                }
            }

            }
        }
        else
        {
            for (int x = 0; x < room.tilesInRoom.Count; x++)
            {
                cur = GameObjectPoolManager.Instance.GetObjectFromPool(BaseTileKey);
                cur.transform.parent = parent;
                cur.SetActive(true);


              
                    if (c != default)
                    {
                        cur.transform.position = new Vector3(room.tilesInRoom[x].x + .5f, room.tilesInRoom[x].y + .5f, 0f);

                        cur.GetComponent<SpriteRenderer>().color = new Color(c.r,c.g,c.b,c.a/2f);
                        cur.GetComponent<SpriteRenderer>().sortingOrder = 98;
                    }
                    else
                    {
                        cur.transform.position = new Vector3(room.tilesInRoom[x].x + .5f, room.tilesInRoom[x].y + .5f, .1f);

                        cur.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1, .3f);
                    }
                

            }
        }
    }



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
        if (r == null)
        {
            return;
        }
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
        Debug.Log("Rendering room " + r.displayColour.ToString() + " tiles " + r.tilesInRoom.Count);
        RenderPoints(parentsOfRooms[r].transform, r, r.displayColour);
        
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

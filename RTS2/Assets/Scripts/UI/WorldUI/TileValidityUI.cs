using UnityEngine;

public class TileValidityUI : MonoBehaviour
{
    static TileValidityUI instance;
    public static TileValidityUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TileValidityUI>();
                instance.Init();
            }
            return instance;
        }
    }


    void Init()
    {
        TilePool = new GameObjectPool(TileDisplayPrefab, 100);
    }
    public Color Valid, Invalid;
    public GameObject TileDisplayPrefab;
    GameObjectPool TilePool;
    public void Cleanup()
    {
        TilePool.ReturnAllObjectsToPool();
    }

    public void DrawTileValidity(Vector2Int coords,int width,int height)
    {
        Cleanup();
        Vector3 pos = new Vector3();
        GameObject tile = null;
        for (int x = coords.x; x < coords.x + width; x++)
        {
            for (int y = coords.y; y < coords.y + height; y++)
            {
                pos.x = x+.5f;
                pos.y = y+.5f;
                tile = TilePool.GetObjectFromPool();
                tile.SetActive(true);
                tile.transform.position = pos;
                if (WorldController.Instance.IsTraversible(x, y) == false)
                {
                    tile.GetComponent<SpriteRenderer>().color = Invalid;
                }
                else
                {
                    tile.GetComponent<SpriteRenderer>().color = Valid;

                }
            }

        }
    }
}

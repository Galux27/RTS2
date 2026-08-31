using UnityEngine;

public class RoomTileDebug : MonoBehaviour
{

    public static void DebugDrawAllTiles(GeneratedBuilding building)
    {
        GameObject BuildingParent = new GameObject();
        BuildingParent.name = "Building tile debug";
        Sprite sp = Sprite.Create(Resources.Load("DebugSprite") as Texture2D, new Rect(0, 0, 32, 32), new Vector2(.5f, .5f));
        for(int x = 0; x < building.Tiles.GetLength(0); x++)
        {
            for(int y = 0; y < building.Tiles.GetLength(1); y++)
            {
                if (building.Tiles[x, y] == null)
                {
                    continue;
                }
                GameObject tile = CreateRoomTileDebug(building.Tiles[x, y], building.Position + new Vector2Int(x, y),sp);
                tile.transform.parent = BuildingParent.transform;
            }
        }
    }

    public static GameObject CreateRoomTileDebug(RoomTile tile,Vector2Int position,Sprite debugSprite)
    {
        GameObject g = new GameObject() ;
        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 22222;
        sr.sprite = debugSprite;
        g.transform.localScale = Vector3.one * .5f;
        RoomTileDebug rtd=g.AddComponent<RoomTileDebug>();
        rtd.TileIRepresent=tile;
        rtd.transform.position = new Vector3(position.x+.5f, position.y+.5f, 1);
        return g;
    }

    public RoomTile TileIRepresent;
}

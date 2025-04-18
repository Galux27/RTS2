using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class DebugDrawing : MonoBehaviour
{
    static DebugDrawing instance;
    public static DebugDrawing Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<DebugDrawing>();
            }
            return instance;
        }
    }

    private void Update()
    {
        if (ConstructableObjectManager.Instance.selectedToConstruct != null)
        {
            DrawConstruction();
        }
    }

    public void DrawEnvironmentObjectInstance(EnvironmentObjectInstance toDraw)
    {
        EnvironmentObject data = EnvironmentObjectHelpers.GetEnvironmentObject(toDraw.ObjectKey);


      

        Vector3 cursorPos = toDraw.Position();
        Vector2Int coords = toDraw.coords;//WorldController.Instance.ConvertWorldToTileCoords(cursorPos);

        Color c = Color.green;

        for (int x = coords.x; x < coords.x + data.GetWidth; x++)
        {
            for (int y = coords.y; y < coords.y + data.GetHeight; y++)
            {
              
                if (WorldController.Instance.IsTraversible(x, y) == false)
                {
                    c = Color.red;
                }
                else if (FurnitureSelectionMode.AreAllTilesWalkable(coords,data.GetWidth,data.GetHeight) == false)
                {
                    c = Color.blue;
                }
                else if (FurnitureSelectionMode.DoBoundsIntersectExisting(coords, data.Size()))
                {
                    c = Color.cyan;
                    
                }


                Vector3 p = new Vector3(x, y);
              //  DrawSquare(p, c);
                Debug.DrawLine(p, cursorPos,c);
                // Debug.DrawLine(CursorSelect.Instance.GetMousePosition(), new Vector3(x, y), c);
            }

        }
    }

    void DrawConstruction()
    {
        Vector3 cursorPos = CursorSelect.Instance.GetMousePosition();
        Vector2Int coords = WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        for (int x = coords.x; x < coords.x + ConstructableObjectManager.Instance.selectedToConstruct.GetWidth; x++)
        {
            for (int y = coords.y ; y < coords.y + ConstructableObjectManager.Instance.selectedToConstruct.GetHeight; y++)
            {
                Color c = Color.green;
                if (WorldController.Instance.IsTraversible(x, y) == false)
                {
                    c = Color.red;
                }
                else if (FurnitureSelectionMode.AreAllTilesWalkable(coords, ConstructableObjectManager.Instance.selectedToConstruct.GetWidth, ConstructableObjectManager.Instance.selectedToConstruct.GetHeight) == false)
                {
                    c = Color.blue;
                }else if (FurnitureSelectionMode.DoBoundsIntersectExisting(coords, ConstructableObjectManager.Instance.selectedToConstruct.Size()))
                {
                    c = Color.cyan;
                }

                Vector3 p = new Vector3(x, y);
                    DrawSquare(p, c);
                Debug.DrawLine(p, cursorPos, Color.black);
               // Debug.DrawLine(CursorSelect.Instance.GetMousePosition(), new Vector3(x, y), c);
            }

        }
    }

    void DrawSquare(Vector3 pos,Color c)
    {
        Debug.DrawLine(pos, pos + new Vector3(1, 0), c);
        Debug.DrawLine(pos, pos + new Vector3(0, 1), c);

        Debug.DrawLine(pos + new Vector3(1, 0), pos + new Vector3(1, 1), c);
        Debug.DrawLine(pos + new Vector3(0, 1), pos + new Vector3(1,1), c);

    }
}

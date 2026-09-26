using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class ConstructableButton : MonoBehaviour
{
    public string ObjectKey;
    public Image Icon;
    public TextMeshProUGUI NameDisp;
    Button myButton;
    private void Awake()
    {
        myButton = this.GetComponent<Button>();
    }

    public void SetEnvObject(EnvironmentObject toSet,string key)
    {
        ObjectKey = key;
        Icon.sprite = toSet.ForwardsSprite;
        NameDisp.text = toSet.Name;
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClickEnvObject);

    }

    public void SetWallObject(WallTile tile)
    {
        ObjectKey = tile.WallName;
        NameDisp.text = tile.WallName;
        Icon.sprite = tile.LeftRight.sprite;
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClickWall);
    }

    public void SetDoorObject(WallTile tile)
    {
        ObjectKey = tile.WallName;
        NameDisp.text = tile.WallName;
        Icon.sprite = tile.LeftRight.sprite;
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClickDoor);
    }

    void OnClickDoor()
    {
        StructureSelectionMode.Mode = StructureSelectionType.Door;
        SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Structures);

        UIEventManager.OnConstructableObjectSelected?.Invoke(string.Empty);
        //add something to disable the cursor when its set to null for both walls and objects
        UIEventManager.OnWallTileSelected?.Invoke(WallTypeManager.Instance.AllObjects[ObjectKey]);

    }
    void OnClickWall()
    {
        StructureSelectionMode.Mode = StructureSelectionType.Walls;
        SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Structures);

        UIEventManager.OnConstructableObjectSelected?.Invoke(string.Empty);

        UIEventManager.OnWallTileSelected?.Invoke(WallTypeManager.Instance.AllObjects[ObjectKey]);


    }
    void OnClickEnvObject()
    {
        StructureSelectionMode.Mode = StructureSelectionType.None;
        SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Furniture);

        UIEventManager.OnWallTileSelected?.Invoke(null);
        UIEventManager.OnConstructableObjectSelected?.Invoke(ObjectKey);
        Debug.Log("Click Env: " + ObjectKey + " mode " + SelectionController.Instance.selectionMode);
    }
}

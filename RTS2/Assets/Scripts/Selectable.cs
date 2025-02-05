
using UnityEngine;
public interface Selectable 
{
    void OnObjectSelected();
    void OnObjectDeselected();

    public SelectableType GetSelectableType();

    public bool GetIsSelected();

    public bool IsSelectable();

    public void SetIsSelected(bool val);

    public Vector3 GetSize();
    public bool IsPointInBounds(Vector3 point);
}

public enum SelectableType
{
    None,
    Unit,
    Structure,
    ConstructableObject,
    Item,
    UnderConstructionObject,
    Resource



}

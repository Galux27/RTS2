
using UnityEngine;
public interface Selectable :ObjectBounds
{
    void OnObjectSelected();
    void OnObjectDeselected();

    public SelectableType GetSelectableType();

    public bool GetIsSelected();

    public bool IsSelectable();

    public void SetIsSelected(bool val);

 
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

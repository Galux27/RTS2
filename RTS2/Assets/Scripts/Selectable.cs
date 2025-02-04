

public interface Selectable 
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
    ConstructableObject,
    Item,
    UnderConstructionObject,
    Resource



}

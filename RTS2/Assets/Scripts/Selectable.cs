

public interface Selectable 
{
    void OnObjectSelected();
    void OnObjectDeselected();

    SelectableType GetSelectableType();

    public bool GetIsSelected();
    public void SetIsSelected(bool val);
}

public enum SelectableType
{
    Unit,
    Building
}

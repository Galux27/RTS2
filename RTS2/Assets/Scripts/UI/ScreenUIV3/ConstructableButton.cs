using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ConstructableButton : MonoBehaviour
{
    public string ObjectKey;
    public Image Icon;
    public TextMeshProUGUI NameDisp;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetButton(EnvironmentObject toSet)
    {
        ObjectKey = toSet.Name;
        Icon.sprite = toSet.ForwardsSprite;
        NameDisp.text = ObjectKey;
    }


    void OnClick()
    {
        UIEventManager.OnConstructableObjectSelected?.Invoke(ObjectKey);
    }
}

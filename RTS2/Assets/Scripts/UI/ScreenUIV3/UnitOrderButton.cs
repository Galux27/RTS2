using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitOrderButton : BaseUIElement
{
    public Image Icon;
    public TextMeshProUGUI True, False;
    public Button MyButton;

    public OrderCounter MyOrder;
    private void Awake()
    {
        MyButton.onClick.AddListener(ButtonPress);
    }
    public void SetOrderDetails(OrderCounter MyOrder)
    {
        this.MyOrder = MyOrder;
        Icon.sprite = IconManager.Instance.GetIcon(MyOrder.OrderKey);
        True.text = MyOrder.Following.ToString();
        False.text = MyOrder.NotFollowing.ToString();
    }

    void ButtonPress()
    {
        if (MyOrder.Following >= MyOrder.NotFollowing)
        {
            UIEventManager.SetUnitsToFollowOrder(MyOrder.OrderKey, false);
        }
        else
        {
            UIEventManager.SetUnitsToFollowOrder(MyOrder.OrderKey, true);

        }
    }
}

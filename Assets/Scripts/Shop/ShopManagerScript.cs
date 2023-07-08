using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShopManagerScript : MonoBehaviour
{
    [SerializeField]
    private int price = 1;
    [SerializeField]
    private GameObject itemPrefab = null;

    public void TryBuy(XRSimpleInteractable pile)
    {
        var player = PlayerManager.Instance();
        if (!player.Inventory().TryReduceMoney(price)) return; // 돈 부족

        var item = Instantiate(itemPrefab); // 물건을 새로 만들어서 쥐어줌
        var itemGrab = item.GetComponent<XRGrabInteractable>();
        itemGrab.interactionManager.SelectEnter(pile.firstInteractorSelecting, itemGrab);
    }

}

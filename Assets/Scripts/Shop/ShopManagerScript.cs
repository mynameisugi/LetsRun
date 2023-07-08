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
        if (!player.Inventory().TryReduceMoney(price)) return; // �� ����

        var item = Instantiate(itemPrefab); // ������ ���� ���� �����
        var itemGrab = item.GetComponent<XRGrabInteractable>();
        itemGrab.interactionManager.SelectEnter(pile.firstInteractorSelecting, itemGrab);
    }

}

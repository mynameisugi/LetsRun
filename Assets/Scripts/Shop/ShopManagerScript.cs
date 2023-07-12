using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShopManagerScript : MonoBehaviour
{
    [SerializeField]
    private int price = 1;
    [SerializeField]
    private GameObject itemPrefab = null;

    private IXRHoverInteractor interactor = null;

    private bool hovering = false;

    private float purchase = 0f;

    public void OnHoverStart(HoverEnterEventArgs eventArgs)
    {
        interactor = eventArgs.interactorObject;
        if (interactor != null) hovering = true;
    }

    public void OnHoverEnd(XRSimpleInteractable self)
    {
        interactor = null;
        hovering = false;
    }

    private void Update()
    {
        if (!hovering) return;

        if (Vector3.Dot(interactor.transform.up, interactor.transform.position - transform.position) > 0f)
        {
            purchase += Time.deltaTime;
            if (purchase > 1f)
            {
                hovering = false;
                TryBuy();
            }
        }
        else purchase = Mathf.Max(0f, purchase - Time.deltaTime * 2f);
    }

    public void TryBuy()
    {
        if (GameSettings.Values.rumble)
            PlayerManager.Instance().Action().GetDevice(0).SendHapticImpulse(0, 0.2f, 0.5f);
        
        var player = PlayerManager.Instance();
        if (!player.Inventory().TryReduceMoney(price)) return; // 돈 부족

        var item = Instantiate(itemPrefab); // 물건을 새로 만들어서 쥐어줌
        //var itemGrab = item.GetComponent<XRGrabInteractable>();
        //itemGrab.interactionManager.SelectEnter(pile.firstInteractorSelecting, itemGrab);
    }

}

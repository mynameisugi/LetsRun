using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShopManagerScript : MonoBehaviour
{
    [Header("Goods")]
    [SerializeField]
    private int price = 1;
    [SerializeField]
    private GameObject itemPrefab = null;
    [SerializeField]
    private Transform itemSpawn = null;

    [Header("NPC")]
    [SerializeField]
    private TextReader NPC = null;
    [SerializeField]
    private string purchaseDialogue = "FoodBuyMultteock";

    private IXRHoverInteractor interactor = null;

    private bool hovering = false;

    private float purchase = 0f;

    public void OnHoverStart(HoverEnterEventArgs eventArgs)
    {
        interactor = eventArgs.interactorObject;
        if (interactor != null) hovering = true;
    }

    public void OnHoverEnd()
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
        if (!player.Inventory().TryReduceMoney(price)) // µ∑ ∫Œ¡∑
        {
            if (NPC) NPC.PlayConversation("FoodBuyFail");
            return;
        }

        var item = Instantiate(itemPrefab); // π∞∞«¿ª ªı∑Œ ∏∏µÈæÓº≠ ¡„æÓ¡‹
        item.transform.SetPositionAndRotation(itemSpawn.position, itemSpawn.rotation);
        if (NPC) NPC.PlayConversation(purchaseDialogue);
        //var itemGrab = item.GetComponent<XRGrabInteractable>();
        //itemGrab.interactionManager.SelectEnter(pile.firstInteractorSelecting, itemGrab);
    }

}

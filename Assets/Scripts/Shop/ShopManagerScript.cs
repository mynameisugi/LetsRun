using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShopManagerScript : MonoBehaviour
{
    [SerializeField]
    private AudioSource aSource = null;

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

    [Header("Scanner")]
    [SerializeField]
    private TMP_Text textPrice;

    private void Start()
    {
        if (textPrice) textPrice.text = $"가격 {price}";
    }

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
        aSource.volume = 0.3f * GameSettings.Values.SE;
        aSource.Play();

        var player = PlayerManager.Instance();
        if (!player.Inventory().TryReduceMoney(price)) // 돈 부족
        {
            if (NPC) NPC.PlayConversation("FoodBuyFail");
            return;
        }

        var item = Instantiate(itemPrefab); // 물건을 새로 만들어서 쥐어줌
        item.transform.SetPositionAndRotation(itemSpawn.position, itemSpawn.rotation);
        if (NPC) NPC.PlayConversation(purchaseDialogue);
        //var itemGrab = item.GetComponent<XRGrabInteractable>();
        //itemGrab.interactionManager.SelectEnter(pile.firstInteractorSelecting, itemGrab);
    }

}

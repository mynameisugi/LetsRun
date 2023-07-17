using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FoodItem : MonoBehaviour
{
    [SerializeField]
    private HandAnimator.SpecialAnimation gripAnimation = HandAnimator.SpecialAnimation.GripRice;

    [Header("StatBoost")]
    [SerializeField] private float speedWalk = 0f;
    [SerializeField] private float speedTrot = 0f;
    [SerializeField] private float speedCanter = 0f;
    [SerializeField] private float speedGallop = 0f;
    [SerializeField] private float stamina = 0f;
    [SerializeField] private float steer = 0f;

    private PlayerInventory targetInventory = null;
    private HorseController targetHorse = null;

    private void OnTriggerEnter(Collider other)
    {
        var otherRoot = other.transform.root;
        if (otherRoot.CompareTag("Player"))
            targetInventory = otherRoot.GetComponent<PlayerManager>().Inventory();
        else if (other.gameObject.name == "FoodFeed")
            targetHorse = otherRoot.GetComponent<HorseController>();
    }

    private void OnTriggerExit(Collider other)
    {
        var otherRoot = other.transform.root;
        if (otherRoot.CompareTag("Player"))
            targetInventory = null;
        else if (other.gameObject.name == "FoodFeed")
            targetHorse = null;
    }

    public void OnUseAttempt()
    {
        if (targetHorse)
        {
            TryUseOnHorse();
            return;
        }
        if (targetInventory != null)
        {
            TryStore();
            return;
        }
    }

    private void TryUseOnHorse()
    {
        if (!targetHorse.playerRidable) return; // 플레이어가 탈 수 없는 말에는 못 먹임

        targetHorse.stats.SpeedWalk += speedWalk;
        targetHorse.stats.SpeedTrot += speedTrot;
        targetHorse.stats.SpeedCanter += speedCanter;
        targetHorse.stats.SpeedGallop += speedGallop;
        targetHorse.stats.GallopAmount += stamina;
        targetHorse.stats.SteerStrength += steer;
        Debug.Log($"{targetHorse.gameObject.name} ate {gameObject.name}");

        targetHorse.MySoundMaker.OnHorseNeigh();

        Destroy(gameObject);
    }

    private void TryStore()
    {
        // TODO: 인벤토리에 아이템 보관

        // Destroy(gameObject);
    }

    public void OnGrabbed(SelectEnterEventArgs eventArgs)
    {
        HandAnimator.RequestAnimation(eventArgs.interactorObject, gripAnimation);
    }

    public void OnGrabEnded(SelectExitEventArgs eventArgs)
    {
        HandAnimator.RequestAnimation(eventArgs.interactorObject, HandAnimator.SpecialAnimation.None);
    }
}

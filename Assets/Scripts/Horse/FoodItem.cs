using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("StatBoost")]
    [SerializeField] private float speedWalk = 0f;
    [SerializeField] private float speedTrot = 0f;
    [SerializeField] private float speedCanter = 0f;
    [SerializeField] private float speedGallop = 0f;
    [SerializeField] private float stamina = 0f;
    [SerializeField] private float steer = 0f;

    private void OnTriggerEnter(Collider other)
    {
        var horse = other.transform.root.GetComponent<HorseController>();
        if (!horse) return;
        if (!horse.playerRidable) return;

        horse.stats.SpeedWalk += speedWalk;
        horse.stats.SpeedTrot += speedTrot;
        horse.stats.SpeedCanter += speedCanter;
        horse.stats.SpeedGallop += speedGallop;
        horse.stats.GallopAmount += stamina;
        horse.stats.SteerStrength += steer;

        Destroy(gameObject);
    }
}

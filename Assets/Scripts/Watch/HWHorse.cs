using UnityEngine;

public class HWHorse : MonoBehaviour
{
    [SerializeField]
    private GameObject horseYes;
    [SerializeField]
    private GameObject horseNo;


    private HorseStats stats;

    private bool hasHorse = false;

    private void OnEnable()
    {
        horseYes.SetActive(false);
        horseNo.SetActive(false);
        var horse = PlayerManager.Instance().horse;
        if (!horse) { hasHorse = false; horseNo.SetActive(true); return; }
        stats = PlayerManager.Instance().horse.stats;
        hasHorse = true; horseYes.SetActive(true);
    }
}

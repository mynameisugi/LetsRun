using UnityEngine;

public class HWHorse : MonoBehaviour
{
    [SerializeField]
    private GameObject horseNo;

    [SerializeField]
    private GameObject horseYes;

    [SerializeField]
    private HWHorseStat[] statDisplays;

    private HorseStats stats;

    private void OnEnable()
    {
        horseYes.SetActive(false);
        horseNo.SetActive(false);
        var horse = PlayerManager.Instance().horse;
        if (!horse) { horseNo.SetActive(true); return; }

        stats = PlayerManager.Instance().horse.stats;
        horseYes.SetActive(true);
        float[] s = new float[] { stats.SpeedWalk, stats.SpeedTrot, stats.SpeedCanter, stats.SpeedGallop, stats.GallopAmount, stats.SteerStrength / 1.5f };
        for (int i = 0; i < 6; ++i)
            statDisplays[i].SetTarget((s[i] - HorseStats.MinStats[i]) / (HorseStats.MaxStats[i] - HorseStats.MinStats[i]));

    }
}

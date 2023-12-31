using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GUIRaceMap : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField]
    private TMP_Text textTime = null;
    [SerializeField]
    private TMP_Text textType = null;

    [Header("Dots")]
    [SerializeField]
    private float scale = 1f;
    [SerializeField]
    private Transform offset = null;
    [SerializeField]
    private RectTransform[] dots = new RectTransform[8];
    [SerializeField]
    private RectTransform start;

    public void AssignRace(Race newRace)
    {
        race = newRace;
        time = GameManager.Instance().Time;
        textType.text = race.info.type switch
        {
            RaceManager.RaceType.Easy => "500m",
            RaceManager.RaceType.Normal => "1000m",
            _ => "1500m"
        };
        textTime.text = "0:00:0";

        var o = race.info.start.transform.position - offset.position;
        start.SetLocalPositionAndRotation(new Vector3(scale * o.z, scale * -o.x, 0f),
            Quaternion.Euler(0f, 0f, -race.info.start.transform.rotation.eulerAngles.y));
        Update();
    }

    private Race race = null;
    private TimeManager time = null;
    private float curTime = 0f;

    private void Update()
    {
        if (!offset || !race) { gameObject.SetActive(false); return; }
        if (race.Status == Race.RaceStage.Racing) curTime = time.Now;
        textTime.text = $"{Mathf.FloorToInt(curTime) / 60:0}:{Mathf.FloorToInt(curTime) % 60:00}:{Mathf.RoundToInt(curTime * 10f) % 10}";

        for (int i = 0; i < 8; ++i)
        {
            var entry = race.GetEntry(i);
            if (!entry) { dots[i].gameObject.SetActive(false); continue; }
            dots[i].gameObject.SetActive(true);
            var o = entry.transform.position - offset.position;
            dots[i].localPosition = new Vector3(scale * o.z - 300f, scale * -o.x - 150f, 0f);
            if (entry.isPlayerRiding) dots[i].localScale = Vector3.one * 2f;
            else dots[i].localScale = Vector3.one;
        }
    }

}

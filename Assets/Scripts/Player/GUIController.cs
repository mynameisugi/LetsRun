using TMPro;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    [SerializeField] private GameObject panelTuto;
    [SerializeField] private TMP_Text textTuto;

    [SerializeField] private GUIRaceMap raceMap;

    public void SetMessageBoxText(string text)
    {
        if (string.IsNullOrEmpty(text)) { panelTuto.SetActive(false); return; }
        panelTuto.SetActive(true);
        textTuto.text = text;
    }

    public void ShowRaceMap(Race race)
    {
        raceMap.gameObject.SetActive(true);
        raceMap.AssignRace(race);
    }

    public void HideRaceMap() => raceMap.gameObject.SetActive(false);

}

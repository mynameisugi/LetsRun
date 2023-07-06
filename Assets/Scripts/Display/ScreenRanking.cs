using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenRanking : MonoBehaviour
{
    [SerializeField]
    private TMP_Text[] texts;

    private HorseController[] entries = new HorseController[8];

    private int[] entryScores = new int[8];

    private void CalculateEntryScore(int i)
    {
        var stats = entries[i].stats;
        // 스탯 4개를 최소~최대 사이로 수준 확인
        // + Mathf.Pow(Random.Range(-1f, 1f), 3f) 의 오차
    }

    private void Update()
    {
        string displayText;
        int time = Mathf.FloorToInt(GameManager.Instance().Time.Now);
        var race = GameManager.Instance().Race.CurrentRace;
        if (!race) displayText = NoRaceDisplay(time);
        else displayText = RaceDisplay(race, time);

        foreach (var tmp in texts) tmp.text = displayText;
    }

    private string NoRaceDisplay(int time)
    {
        


        return $"경주 {time - TimeManager.LOOP}초 전";
    }

    private string RaceDisplay(Race race, int time)
    {

        return $"경주 {time - TimeManager.LOOP}초 전";
    }

}

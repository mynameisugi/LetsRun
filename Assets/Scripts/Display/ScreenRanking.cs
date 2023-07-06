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
        // ���� 4���� �ּ�~�ִ� ���̷� ���� Ȯ��
        // + Mathf.Pow(Random.Range(-1f, 1f), 3f) �� ����
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
        


        return $"���� {time - TimeManager.LOOP}�� ��";
    }

    private string RaceDisplay(Race race, int time)
    {

        return $"���� {time - TimeManager.LOOP}�� ��";
    }

}

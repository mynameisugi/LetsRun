using System.Text;
using TMPro;
using UnityEngine;

public class ScreenRanking : MonoBehaviour
{
    [SerializeField]
    private TMP_Text[] bigTexts;
    [SerializeField]
    private TMP_Text[] texts;

    private HorseController[] entries = new HorseController[8];

    private int[] entryScores = new int[8];

    private void CalculateEntryScore(int i)
    {
        var stats = entries[i].stats;
        float[] s = new float[] { stats.SpeedTrot, stats.SpeedCanter, stats.SpeedGallop, stats.GallopAmount, stats.SteerStrength / 1.5f };
        float score = 0f; // Mathf.Pow(Random.Range(-1f, 1f), 3f); // 오차
        for (int t = 1; t < 6; ++t)
            score += (s[t - 1] - HorseStats.MinStats[t]) / (HorseStats.MaxStats[t] - HorseStats.MinStats[t]);
        score = Mathf.Clamp(score * 2f, 1f, 10f);
        entryScores[i] = Mathf.RoundToInt(score);
    }


    private void Update()
    {
        string displayBigText;
        string displayText = "";
        float time = GameManager.Instance().Time.Now;
        var race = GameManager.Instance().Race.CurrentRace;
        if (!race) displayBigText = NoRaceDisplay(Mathf.FloorToInt(time));
        else
        {
            displayText = RaceDisplay(race, time);
            displayBigText = displayText.Split('\n')[0];
            displayText = displayText[(displayText.IndexOf('\n') + 1)..];
        }

        foreach (var tmp in bigTexts) tmp.text = displayBigText;
        foreach (var tmp in texts) tmp.text = displayText;
    }

    private string NoRaceDisplay(int time)
    {
        int tMinus = TimeManager.LOOP - time;
        string display = $"경주 {tMinus / 60}분 ";
        if (tMinus % 60 > 0) display += $"{tMinus % 60}초 ";
        return display + "전";
    }

    private string RaceDisplay(Race race, float time)
    {
        if (race.Status == Race.RaceStage.Prepare)
        {
            string statText = "";
            for (int i = 0; i < 8; ++i)
            {
                var entry = race.GetEntry(i);
                if (!entry) { entries[i] = null; break; }
                if (entries[i] != entry) { entries[i] = entry; CalculateEntryScore(i); }
                statText += $"{i + 1}번마 {ScoreToStars(entryScores[i])}"
                    + (i % 2 == 0 ? "\t\t\t" : "\r\n");
            }

            return $"경주 {TimeManager.LOOP - time:0.0}초 전\r\n" + statText;

            static string ScoreToStars(int N)
            {
                StringBuilder SB = new();
                while (N > 0)
                {
                    if (N > 1) { SB.Append('★'); N -= 2; }
                    else { SB.Append('☆'); --N; }
                }
                while (SB.Length < 5) SB.Append("   ");
                return SB.ToString();
            }
        }
        if (race.Status == Race.RaceStage.Racing)
        {
            string resultText = "";
            for(int i = 0; i < race.goalInfos.Count; ++i)
            {
                var info = race.goalInfos[i];
                resultText += $"{i + 1}위 {info.index + 1}번마 {info.time:0.00}초\r\n";
            }

            return $"경주 시간: {Mathf.FloorToInt(time / 60f)}:{time % 60f:00.00}\r\n" + resultText;
        }

        return "경주 정리 중\n ";
    }


}

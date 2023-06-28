using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RankingUpdate : MonoBehaviour
{
    public List<RunnerTracker> runners;
    public Text rankingText;

    private StringBuilder rankingBuilder = new StringBuilder();

    private void Update()
    {
        for (int i = 1; i < runners.Count; i++)
        {
            RunnerTracker currentRunner = runners[i];
            float currentScore = currentRunner.GetRankingScore();
            int j = i - 1;

            while (j >= 0 && runners[j].GetRankingScore() < currentScore)
            {
                runners[j + 1] = runners[j];
                j--;
            }

            runners[j + 1] = currentRunner;
        }

        UpdateRankingText();
    }

    private void UpdateRankingText()
    {
        rankingBuilder.Clear();

        for (int i = 0; i < runners.Count; i++)
        {
            RunnerTracker runner = runners[i];
            float rankingScore = runner.GetRankingScore();
            float lapTime = runner.GetLapTime();

            rankingBuilder.Append($"Rank {i + 1}: {runner.name}           (Time: {lapTime.ToString("F2")}s)\n");
        }

        rankingText.text = rankingBuilder.ToString();
        rankingText.gameObject.SetActive(true);
    }
}
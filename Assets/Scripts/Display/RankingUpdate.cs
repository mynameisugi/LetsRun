using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RankingUpdate : MonoBehaviour
{
    public List<RunnerTracker> runners;
    public Text rankingText;

    private void Update()
    {
        List<RunnerTracker> sortedRunners = runners.OrderByDescending(runner => runner.GetRankingScore()).ToList();

        

        UpdateRankingText(sortedRunners);

    }

    private void UpdateRankingText(List<RunnerTracker> sortedRunners)
    {
        string ranking = "";
        for (int i = 0; i < sortedRunners.Count; i++)
        {
            RunnerTracker runner = sortedRunners[i];
            float rankingScore = runner.GetRankingScore();
            float lapTime = runner.GetLapTime();

            ranking += $"Rank {i + 1}: {runner.name} (Time: {lapTime:F2}s)\n";
        }

        rankingText.text = ranking;

        rankingText.gameObject.SetActive(true); // rankingText È°¼ºÈ­
    }


}

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
        List<RunnerTracker> sortedRunners = runners.OrderByDescending(runner => runner.GetDistanceTraveled()).ToList();

        UpdateRankingText(sortedRunners);
    }

    private void UpdateRankingText(List<RunnerTracker> sortedRunners)
    {
        string ranking = "";
        for (int i = 0; i < sortedRunners.Count; i++)
        {
            ranking += $"Rank {i + 1}: {sortedRunners[i].name}\n";
        }

        rankingText.text = ranking;
    }

}

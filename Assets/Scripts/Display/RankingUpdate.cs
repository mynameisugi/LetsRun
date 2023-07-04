using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RankingUpdate : MonoBehaviour
{
    public List<RunnerTracker> runners; // RunnerTracker 객체를 담는 리스트
    public Text rankingText; // 랭킹을 표시할 텍스트
    public Text rankingText2; // 랭킹을 표시할 텍스트

    private StringBuilder rankingBuilder = new StringBuilder(); // 랭킹 텍스트를 구성하기 위한 StringBuilder 객체

    private void Update()
    {
        // 삽입 정렬을 사용하여 랭킹을 업데이트
        for (int i = 1; i < runners.Count; i++)
        {
            RunnerTracker currentRunner = runners[i]; // 현재 RunnerTracker 객체
            float currentScore = currentRunner.GetRankingScore(); // 현재 랭킹 점수
            int j = i - 1;

            while (j >= 0 && runners[j].GetRankingScore() < currentScore)
            {
                runners[j + 1] = runners[j];
                j--;
            }

            runners[j + 1] = currentRunner;
        }

        UpdateRankingText(); // 랭킹 텍스트 업데이트
    }

    private void UpdateRankingText()
    {
        rankingBuilder.Clear(); // StringBuilder 초기화

        // 모든 러너에 대해 랭킹 텍스트 구성
        for (int i = 0; i < runners.Count; i++)
        {
            RunnerTracker runner = runners[i]; // 러너 객체
            float lapTime = runner.GetLapTime(); // 주행 시간
            if (lapTime == 0f) break;

            rankingBuilder.Append($"Rank {i + 1}: {runner.name}           (Time: {lapTime.ToString("F2")}s)\n");
        }

        rankingText.text = rankingBuilder.ToString(); // 텍스트 업데이트
        rankingText2.text = rankingBuilder.ToString(); // 텍스트 업데이트
    }
}

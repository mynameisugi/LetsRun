using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RankingUpdate : MonoBehaviour
{
    public List<RunnerTracker> runners; // RunnerTracker ��ü�� ��� ����Ʈ
    public Text rankingText; // ��ŷ�� ǥ���� �ؽ�Ʈ
    public Text rankingText2; // ��ŷ�� ǥ���� �ؽ�Ʈ

    private StringBuilder rankingBuilder = new StringBuilder(); // ��ŷ �ؽ�Ʈ�� �����ϱ� ���� StringBuilder ��ü

    private void Update()
    {
        // ���� ������ ����Ͽ� ��ŷ�� ������Ʈ
        for (int i = 1; i < runners.Count; i++)
        {
            RunnerTracker currentRunner = runners[i]; // ���� RunnerTracker ��ü
            float currentScore = currentRunner.GetRankingScore(); // ���� ��ŷ ����
            int j = i - 1;

            while (j >= 0 && runners[j].GetRankingScore() < currentScore)
            {
                runners[j + 1] = runners[j];
                j--;
            }

            runners[j + 1] = currentRunner;
        }

        UpdateRankingText(); // ��ŷ �ؽ�Ʈ ������Ʈ
    }

    private void UpdateRankingText()
    {
        rankingBuilder.Clear(); // StringBuilder �ʱ�ȭ

        // ��� ���ʿ� ���� ��ŷ �ؽ�Ʈ ����
        for (int i = 0; i < runners.Count; i++)
        {
            RunnerTracker runner = runners[i]; // ���� ��ü
            float lapTime = runner.GetLapTime(); // ���� �ð�
            if (lapTime == 0f) break;

            rankingBuilder.Append($"Rank {i + 1}: {runner.name}           (Time: {lapTime.ToString("F2")}s)\n");
        }

        rankingText.text = rankingBuilder.ToString(); // �ؽ�Ʈ ������Ʈ
        rankingText2.text = rankingBuilder.ToString(); // �ؽ�Ʈ ������Ʈ
    }
}

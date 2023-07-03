using UnityEngine;

public class RunnerTracker : MonoBehaviour
{
    private Transform runnerTransform;     // ������ Transform ������Ʈ
    private float lapDistance = 0f;         // ���� ������ �Ÿ�
    private int lapCount = 0;               // ������ �� ��
    private float lapStartTime = 0f;        // ���� ���� ���� �ð�
    private float lapTime = 0f;             // ���� ���� ���� �ð�
    public Animator animator;              // Animator ������Ʈ
    public Transform finishLine;           // ������� Transform

    private bool hasCrossedFinishLine = false;   // ��������� ����ߴ��� ����

    private void Start()
    {
        runnerTransform = transform;         // Transform ������Ʈ �Ҵ�
    }

    private void Update()
    {
        Vector3 currentPosition = runnerTransform.position;                        // ���� ��ġ
        float distanceTraveled = Vector3.Distance(currentPosition, runnerTransform.position);    // �̵��� �Ÿ� ���

        lapDistance += distanceTraveled;                     // ������ �Ÿ� ����

        if (!hasCrossedFinishLine && IsAtFinishLine())
        {
            hasCrossedFinishLine = true;
            animator.SetBool("Active", true);
        }
    }

    private bool IsAtFinishLine()
    {
        float distanceToFinishLine = Vector3.Distance(transform.position, finishLine.position);
        return distanceToFinishLine < 0.5f;
    }

    public float GetRankingScore()
    {
        return lapCount * 10000f - lapTime;                   // ��ŷ ���� ����Ͽ� ��ȯ
    }

    public void IncreaseLapCount()
    {
        lapCount++;                                           // �� �� ����
        lapTime = Time.time - lapStartTime;                   // ���� �ð� ���
        lapStartTime = Time.time;                             // ���� ���� ���� �ð� ����
    }

    public float GetLapTime()
    {
        return lapTime;                                       // ���� �ð� ��ȯ
    }
}

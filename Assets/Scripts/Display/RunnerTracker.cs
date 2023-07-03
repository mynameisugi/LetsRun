using UnityEngine;

public class RunnerTracker : MonoBehaviour
{
    private Transform runnerTransform;     // 러너의 Transform 컴포넌트
    private float lapDistance = 0f;         // 현재 주행한 거리
    private int lapCount = 0;               // 주행한 랩 수
    private float lapStartTime = 0f;        // 현재 랩의 시작 시간
    private float lapTime = 0f;             // 현재 랩의 주행 시간
    public Animator animator;              // Animator 컴포넌트
    public Transform finishLine;           // 결승지점 Transform

    private bool hasCrossedFinishLine = false;   // 결승지점을 통과했는지 여부

    private void Start()
    {
        runnerTransform = transform;         // Transform 컴포넌트 할당
    }

    private void Update()
    {
        Vector3 currentPosition = runnerTransform.position;                        // 현재 위치
        float distanceTraveled = Vector3.Distance(currentPosition, runnerTransform.position);    // 이동한 거리 계산

        lapDistance += distanceTraveled;                     // 주행한 거리 누적

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
        return lapCount * 10000f - lapTime;                   // 랭킹 점수 계산하여 반환
    }

    public void IncreaseLapCount()
    {
        lapCount++;                                           // 랩 수 증가
        lapTime = Time.time - lapStartTime;                   // 주행 시간 계산
        lapStartTime = Time.time;                             // 다음 랩의 시작 시간 설정
    }

    public float GetLapTime()
    {
        return lapTime;                                       // 주행 시간 반환
    }
}

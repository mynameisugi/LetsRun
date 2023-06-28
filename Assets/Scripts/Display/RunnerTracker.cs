using UnityEngine;

public class RunnerTracker : MonoBehaviour
{
    private Transform runnerTransform;
    private float lapDistance = 0f;
    private int lapCount = 0;
    private float lapStartTime = 0f;
    private float lapTime = 0f;
    private Vector3 previousPosition;

    private void Start()
    {
        runnerTransform = transform;
        previousPosition = runnerTransform.position;
    }

    private void Update()
    {
        Vector3 currentPosition = runnerTransform.position;
        float distanceTraveled = Vector3.Distance(currentPosition, previousPosition);

        Vector3 movementDirection = currentPosition - previousPosition;
        float dotProduct = Vector3.Dot(movementDirection.normalized, runnerTransform.forward);

        if (dotProduct > 0f)
        {
            lapDistance += distanceTraveled;
        }
        else
        {
            lapDistance -= distanceTraveled;
        }

        previousPosition = currentPosition;
    }

    public float GetRankingScore()
    {
        return lapCount * 10000f - lapTime;
    }

    public void IncreaseLapCount()
    {
        lapCount++;
        lapTime = Time.time - lapStartTime;
        lapStartTime = Time.time;
    }

    public float GetLapTime()
    {
        return lapTime;
    }
}
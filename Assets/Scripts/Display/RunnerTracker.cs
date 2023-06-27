using UnityEngine;

public class RunnerTracker : MonoBehaviour
{
    private Transform runnerTransform;
    private float lapDistance = 0f;
    private int lapCount = 0;
    private float lapStartTime = 0f; // New variable to track the lap start time
    private float lapTime = 0f; // New variable to track the lap time
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

        // Check if the runner is moving forward or backward
        Vector3 movementDirection = currentPosition - previousPosition;
        float dotProduct = Vector3.Dot(movementDirection.normalized, runnerTransform.forward);

        if (dotProduct > 0f)
        {
            // Moving forward
            lapDistance += distanceTraveled;
        }
        else
        {
            // Moving backward
            lapDistance -= distanceTraveled;
        }

        previousPosition = currentPosition;
    }

    public float GetRankingScore()
    {
        return lapCount * 10000f - lapTime; // Updated ranking score calculation
    }

    public void IncreaseLapCount()
    {
        lapCount++;
        lapTime = Time.time - lapStartTime; // Update lap time when lap count increases
        lapStartTime = Time.time; // Reset lap start time
    }

    public float GetLapTime()
    {
        return lapTime;
    }
}

using UnityEngine;

public class AIWaypointMovement : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private int lapsCompleted = 0;

    public float movementSpeed = 5f;

    private void Update()
    {
        if (lapsCompleted >= 1)
        {
            return;
        }

        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 moveDirection = targetPosition - transform.position;
        float distanceToWaypoint = moveDirection.magnitude;

        transform.Translate(moveDirection.normalized * movementSpeed * Time.deltaTime, Space.World);

        if (distanceToWaypoint < 0.5f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
                lapsCompleted++;

                if (lapsCompleted >= 1)
                {
                    Debug.Log("Race completed!");
                }

                RunnerTracker runnerTracker = GetComponent<RunnerTracker>();
                if (runnerTracker != null)
                {
                    runnerTracker.IncreaseLapCount();
                }
            }
        }
    }
}

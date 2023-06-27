using UnityEngine;

public class AIWaypointMovement : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private int lapsCompleted = 0;
    [SerializeField]
    private int totalLaps = 3;

    public float movementSpeed = 5f;
    private float raceStartTime = 0f; // New variable to track the race start time

    private void Start()
    {
        raceStartTime = Time.time; // Record the race start time
    }

    private void Update()
    {
        if (lapsCompleted >= totalLaps)
        {
            // Race completed
            Debug.Log("Race completed!");
            return;
        }

        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length)
        {
            // Player has completed all laps
            return;
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 moveDirection = targetPosition - transform.position;
        float distanceToWaypoint = moveDirection.magnitude;

        transform.Translate(moveDirection.normalized * movementSpeed * Time.deltaTime, Space.World);

        if (distanceToWaypoint < 0.5f)
        {
            // Reached the current waypoint
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                // Player has completed one lap
                currentWaypointIndex = 0;
                lapsCompleted++;

                if (lapsCompleted >= totalLaps)
                {
                    // Race completed
                    Debug.Log("Race completed!");
                }

                // Increase lap count for the runner
                RunnerTracker runnerTracker = GetComponent<RunnerTracker>();
                if (runnerTracker != null)
                {
                    runnerTracker.IncreaseLapCount();
                }
            }
        }
    }
}

using UnityEngine;

public class AIWaypointMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;

    private int currentWaypointIndex = 0;
    private bool isMovingForward = true;

    private void Update()
    {
        Transform currentWaypoint = waypoints[currentWaypointIndex];

        float distanceToWaypoint = Vector3.Distance(transform.position, currentWaypoint.position);

        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime);

        if (distanceToWaypoint <= 0.1f)
        {
            if (isMovingForward)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    isMovingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;

                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    isMovingForward = true;
                }
            }
        }
    }
}

using UnityEngine;

public class RunnerTracker : MonoBehaviour
{
    private Vector3 previousPosition;
    private float distanceTraveled;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        distanceTraveled += Vector3.Distance(currentPosition, previousPosition);
        previousPosition = currentPosition;
    }

    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }
}

using UnityEngine;

public class GoalPost : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<HorseSphere>();
        if (!sphere) return;

        if (GameManager.Instance().Race.CurrentRace)
            GameManager.Instance().Race.CurrentRace.HorseGoal(sphere.horse);
    }
}

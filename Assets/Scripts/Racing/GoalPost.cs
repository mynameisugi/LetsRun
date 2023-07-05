using UnityEngine;

public class GoalPost : MonoBehaviour
{
    private void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<HorseSphere>();
        if (!sphere) return;

        if (GameManager.Instance().Race.CurrentRace)
            GameManager.Instance().Race.CurrentRace.HorseGoal(sphere.horse);
    }
}

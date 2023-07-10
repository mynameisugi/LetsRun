using UnityEngine;

public class GoalPost : MonoBehaviour
{
    [SerializeField]
    private GameObject goalEffect;

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
        {
            int rating = GameManager.Instance().Race.CurrentRace.HorseGoal(sphere.horse);
            if (rating == 1)
            {
                goalEffect.SetActive(true);
                foreach(var ge in goalEffect.GetComponentsInChildren<ParticleSystem>())
                    ge.Play();
                Invoke(nameof(StopEffect), 30f);
            }
        }
    }

    private void StopEffect()
    {
        goalEffect.SetActive(false);
    }
}

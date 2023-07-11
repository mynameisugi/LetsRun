using UnityEngine;
using UnityEngine.AI;

public class HorseSphere : MonoBehaviour
{
    public HorseController horse = null;

    public void SlowPenalty()
    {
        var agent = GetComponent<NavMeshAgent>();
        if (agent.enabled)
        {
            agent.enabled = false;
            Invoke(nameof(ReenableAgent), 2f);
        }
        Vector3 rnd = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f).normalized;
        GetComponent<Rigidbody>().AddForce(rnd * agent.speed, ForceMode.Impulse);
        horse.Penalty(0);
        //Debug.Log($"{horse.gameObject.name} SlowPenalty");
    }

    private void ReenableAgent() => GetComponent<NavMeshAgent>().enabled = true;
}

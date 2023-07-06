using UnityEngine;

public class OJumpPenalty : MonoBehaviour
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
        if (sphere.horse.Jumping > 0f) return;

        // 점프하지 않는 말 페널티
        sphere.horse.Penalty(1);
    }
}

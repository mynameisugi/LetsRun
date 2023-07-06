using UnityEngine;

public class OJumpTrigger : MonoBehaviour
{
    private void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        var sphere = other.GetComponent<HorseSphere>();
        if (!sphere) return;
        sphere.horse.wantToJump = 1f;
    }
}

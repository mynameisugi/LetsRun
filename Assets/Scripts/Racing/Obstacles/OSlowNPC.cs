using UnityEngine;

public class OSlowNPC : MonoBehaviour
{
    private void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.gameObject.GetComponent<HorseSphere>();
        if (!sphere) return;

        // NPC �ӵ� ����
        sphere.horse.slowDown = true;
    }

    private void OnTriggerExit(Collider other)
    {
        var sphere = other.gameObject.GetComponent<HorseSphere>();
        if (!sphere) return;

        // NPC �ӵ� ���� ���
        sphere.horse.slowDown = false;
    }
}

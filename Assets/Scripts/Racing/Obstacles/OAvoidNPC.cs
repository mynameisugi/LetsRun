using UnityEngine;

public class OAvoidNPC : MonoBehaviour
{
    [SerializeField]
    private BoxCollider safeBox;

    private void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.gameObject.GetComponent<HorseSphere>();
        if (!sphere) return;

        if (sphere.horse.isPlayerRiding) return;
        sphere.horse.AddMidwaypoint(safeBox);
    }
}

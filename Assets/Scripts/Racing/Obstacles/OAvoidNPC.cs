using UnityEngine;

public class OAvoidNPC : MonoBehaviour
{
    [SerializeField]
    private BoxCollider[] safeBoxes = new BoxCollider[1];

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
        sphere.horse.AddMidwaypoint(safeBoxes[Random.Range(0, safeBoxes.Length)]);
    }
}

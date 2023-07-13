using UnityEngine;

public class OAvoidSafe : MonoBehaviour
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

        if (sphere.horse.isPlayerRiding && sphere.horse.CurMode >= 3f
            && GameManager.Instance().Race.CurrentRace)
            GameManager.Instance().Race.CurrentRace.AddWow(2f);
    }
}

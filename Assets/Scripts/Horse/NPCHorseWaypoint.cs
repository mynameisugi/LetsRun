using UnityEngine;

public class NPCHorseWaypoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<HorseSphere>();
        if (!sphere) return;
        if (sphere.horse.playerRidable) return;
        if (!sphere.horse.isRacing) return;

    }
}

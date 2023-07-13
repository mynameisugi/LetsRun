using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExit : MonoBehaviour
{
    [SerializeField]
    private IntroController owner;

    private void OnCollisionEnter(Collision collision)
    {
        var sphere = collision.gameObject.GetComponent<HorseSphere>();
        if (!sphere) return;
        if (!sphere.horse.isPlayerRiding) return;
        GetComponent<BoxCollider>().enabled = false;
        owner.OnSpawnExited();
    }
}

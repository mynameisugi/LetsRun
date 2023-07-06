using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OAvoidPenalty : MonoBehaviour
{
    private void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var sphere = collision.gameObject.GetComponent<HorseSphere>();
        if (!sphere) return;

        // 피하지 않은 말 페널티
        Vector3 normal = collision.contacts[0].normal;
        Vector3 vel = collision.rigidbody.velocity;
        if (Vector3.Angle(vel, -normal) > 60f)
            sphere.horse.Penalty(0);
    }
}

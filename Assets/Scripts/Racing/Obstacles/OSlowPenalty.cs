using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSlowPenalty : MonoBehaviour
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

        // ������ ���� �� ���Ƽ
        var horse = sphere.horse;
        if (horse.CurMode > 2.5f) sphere.SlowPenalty();
    }
}

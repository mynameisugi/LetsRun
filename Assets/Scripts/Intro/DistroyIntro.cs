using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistroyIntro : MonoBehaviour
{
    [SerializeField]
    private IntroController owner;

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponentInParent<PlayerManager>();
        if (!player) return;
        if (!player.IsRiding) return;

        owner.EndTutorial();
    }
}

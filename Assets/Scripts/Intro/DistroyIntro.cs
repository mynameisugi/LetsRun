using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistroyIntro : MonoBehaviour
{
    GameObject ForDestroy;
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerManager>();
        if (!player) return;

        if (GameSettings.Values.doAutoSave) GameManager.Instance().Save.SaveToPrefs(0);

        ForDestroy = GameObject.Find("Intro");

        Destroy(ForDestroy);
    }
}

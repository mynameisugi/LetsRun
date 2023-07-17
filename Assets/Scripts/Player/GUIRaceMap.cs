using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIRaceMap : MonoBehaviour
{
    [SerializeField]
    private float scale = 1f;

    [SerializeField]
    private Transform offset = null;

    [SerializeField]
    private RectTransform[] dots = new RectTransform[8];

    public void AssignRace(Race newRace)
    {
        race = newRace;
        // TODO: 시작선 켜고 끄기
        Update();
    }

    private Race race = null;

    private void Update()
    {
        if (!offset || !race) { gameObject.SetActive(false); return; }
        for (int i = 0; i < 8; ++i)
        {
            var entry = race.GetEntry(i);
            if (!entry) { dots[i].gameObject.SetActive(false); continue; }
            dots[i].gameObject.SetActive(true);
            var o = offset.position - entry.transform.position;
            dots[i].localPosition = new Vector3(scale * o.x, scale * o.z, 0f);
            if (entry.isPlayerRiding) dots[i].localScale = Vector3.one * 2f;
            else dots[i].localScale = Vector3.one;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HWClock : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textTime;

    [SerializeField]
    private RectTransform arm;

    private TimeManager time;

    private void Start()
    {
        time = GameManager.Instance().Time;
    }

    private void Update()
    {
        float t = TimeManager.LOOP - time.Now;
        int ti = Mathf.CeilToInt(t);
        textTime.text = $"{ti / 60:0}:{ti % 60:00}";
        arm.localRotation = Quaternion.Euler(0f, 0f, t * 360f / TimeManager.LOOP);
    }
}

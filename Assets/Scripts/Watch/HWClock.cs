using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HWClock : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textTime;

    [SerializeField]
    private RectTransform arm;

    [SerializeField]
    private HWPopup popup;

    private TimeManager time;

    private void Start()
    {
        time = GameManager.Instance().Time;
    }

    private void Update()
    {
        float t = TimeManager.LOOP - time.Now;
        float rot = t * 360f / TimeManager.LOOP;
        if (dragMode > 0)
        {
            if (dragMode == 1)
            {
                var dir = arm.parent.InverseTransformDirection(dragger.position - arm.position);
                dragAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                if (dragAngle < 0f) dragAngle += 360f;
                dragAngle = Mathf.Clamp(dragAngle, SKIPLIMIT * 360f / TimeManager.LOOP, rot); // 10초 제한
                arm.localRotation = Quaternion.Euler(0f, 0f, dragAngle);
            }
            return;
        }
        int ti = Mathf.CeilToInt(t);
        textTime.text = $"{ti / 60:0}:{ti % 60:00}";
        arm.localRotation = Quaternion.Euler(0f, 0f, rot);
    }

    private const int SKIPLIMIT = 10;
    private float dragAngle;
    private Transform dragger = null;
    private byte dragMode = 0;

    public void OnArmDragStart()
    {
        if (TimeManager.LOOP - time.Now < SKIPLIMIT) return; // 스킵 불가
        dragMode = 1;
        Time.timeScale = 0f;
        var pokers = PlayerManager.Instance().GetComponentsInChildren<XRPokeInteractor>();
        dragger = pokers[1].attachTransform;
    }

    public void OnArmDragEnd()
    {
        dragMode = 2;
        int offset = Mathf.FloorToInt(dragAngle / 360f * TimeManager.LOOP - (TimeManager.LOOP - time.Now));
        if (offset < 10) { dragMode = 0; return; }
        string offsetText = "";
        if (offset > 60) offsetText = $"{offset / 60}분 ";
        if (offset % 60 > 0) offsetText += $"{offset % 60}초";

        popup.OpenPopup("시간을 넘기겠습니까?",
            $"{offsetText} 뒤로\r\n시간을 넘깁니다.",
            () => { Time.timeScale = 1f; dragMode = 0; },
            () => { Time.timeScale = 1f; dragMode = 0; });
    }
}

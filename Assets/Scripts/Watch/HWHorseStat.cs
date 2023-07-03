using UnityEngine;

public class HWHorseStat : MonoBehaviour
{
    [SerializeField]
    private RectTransform bar;

    private float curDisplay = 0f;
    private float targetDisplay = 0f;

    public void SetTarget(float target)
    {
        curDisplay = 0f;
        targetDisplay = target * 300f;
        Debug.Log($"{gameObject.name} {targetDisplay}");
    }

    private void Update()
    {
        if (targetDisplay - curDisplay < 0.01f) { curDisplay = targetDisplay; return; }
        curDisplay = Mathf.MoveTowards(curDisplay, targetDisplay, Time.deltaTime * 120f);

        bar.localPosition = new Vector3(-100f + curDisplay * 0.5f, 0f, 0f);
        bar.sizeDelta = new Vector2(curDisplay, bar.sizeDelta.y);
    }

}

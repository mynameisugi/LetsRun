using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class HandWatchController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource = null;

    [SerializeField]
    private GameObject[] canvases = new GameObject[5];

    //private PlayerActionHandler action = null;

    private void Start()
    {
        PlayerManager.Instance().handWatch = this;
        //action = PlayerManager.Instance().Action();
        RequestModeSwitch(Mode.Main);
    }

    #region Mode

    public enum Mode : int
    {
        Main = -1,
        Clock = 0,
        Horse = 1,
        Inventory = 2,
        Setting = 3,
        Map = 4,
        Tutorial = 5
    }

    public Mode CurMode { get; private set; } = Mode.Main;

    public void RequestModeSwitch(Mode mode)
    {
        StopAllCoroutines();
        if (mode != Mode.Main)
            if (PlayerManager.Instance().IsRiding) return; // 말 타는 중에는 조작 금지
        else if (CurMode == mode) mode = Mode.Main; // 다시 클릭: 메뉴 끄기
        // Debug.Log($"RequestModSwitch {CurMode} > {mode}");
        CurMode = mode;
        ToggleCanvases(mode);

        void ToggleCanvases(Mode newMode)
        {
            foreach (var c in canvases) if (c) c.SetActive(false);
            if (newMode == Mode.Main) return;
            canvases[(int)newMode].SetActive(true);
            StartCoroutine(OpenMenu(canvases[(int)newMode].transform as RectTransform));
        }
    }

    private IEnumerator OpenMenu(RectTransform canvas)
    {
        float opened = 0f;
        canvas.localScale = new Vector3(0f, 0.001f, 0.001f);
        while (opened < 1f)
        {
            opened += Time.deltaTime * 3f;
            if (opened > 1f) opened = 1f;
            canvas.localScale = new Vector3(0.001f * Mathf.Pow(opened, 0.5f), 0.001f, 0.001f);
            yield return null;
        }
    }

    #endregion Mode

    /*
    private void Update()
    {
        if (CurMode == Mode.Main) return;
        var device = action.GetDevice(0);
        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out var pos))
            if (pos.y < -0.2f) RequestModeSwitch(Mode.Main); // 왼손이 너무 내려가면 시계 메뉴 끄기
    }
    */

    public void PlayUISound(AudioClip clip, float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = GameSettings.Values.SE * volume;
        audioSource.Play();
    }
}

using UnityEngine;
using UnityEngine.XR;

public class HandWatchController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] canvases = new GameObject[5];

    private PlayerActionHandler action = null;

    private void Start()
    {
        action = PlayerManager.Action();
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
        if (mode != Mode.Main)
        {
            if (PlayerManager.Instance().IsRiding) return; // 말 타는 중에는 조작 금지
            //if (CurMode != Mode.Main) return; // Switching to other mode directly
            CurMode = mode;
        }
        else CurMode = mode;
        ToggleCanvases(mode);

        void ToggleCanvases(Mode newMode)
        {
            foreach (var c in canvases) if (c) c.SetActive(false);
            if (newMode == Mode.Main) return;
            canvases[(int)newMode].SetActive(true);
        }
    }

    #endregion Mode

    private void Update()
    {
        if (CurMode == Mode.Main) return;
        var device = action.GetDevice(0);
        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out var pos))
            if (pos.y < -0.5f) RequestModeSwitch(Mode.Main); // 왼손이 너무 내려가면 시계 메뉴 끄기
    }

}

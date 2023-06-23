using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 플레이어 조작 처리
/// </summary>
public class PlayerActionHandler : MonoBehaviour
{
    [SerializeField]
    internal XRDirectInteractor[] directInteractors = new XRDirectInteractor[2];

    private HandAnimator GetHandAnimator(int grasp) =>
        directInteractors[grasp].transform.parent.GetComponentInChildren<HandAnimator>();

    [Header("Controller Settings")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float gripThreshold = 0.9f;

    private readonly InputDevice[] targetDevices = new InputDevice[2];

    private void Start()
    {
        InitDevices();
    }

    /// <summary>
    /// 추적할 VR 입력 장치를 가져옴
    /// </summary>
    private void InitDevices()
    {
        for (int i = 0; i < 2; ++i)
        {
            lastGrabs[i] = false;

            List<InputDevice> devices = new();
            InputDevices.GetDevicesWithCharacteristics(i == 0 ?
                InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right, devices);
            if (devices.Count < 1) continue;
            targetDevices[i] = devices[0];
        }
    }

    /// <summary>
    /// 이전 프레임에서 잡기 중이었는지 저장 (index 0: 왼쪽, 1: 오른쪽)
    /// </summary>
    private readonly bool[] lastGrabs = new bool[2];

    private void Update()
    {
        if (!targetDevices[0].isValid || !targetDevices[1].isValid)
        { InitDevices(); return; }

        for (int i = 0; i < 2; ++i)
        {
            bool grab = false;
            if (targetDevices[i].TryGetFeatureValue(CommonUsages.grip, out float grip))
                grab = grip > gripThreshold;
            if (grab && !lastGrabs[i] && !GrabOccupied(i)) OnGrabbed?.Invoke(i == 0, targetDevices[i]);
            else if (!grab && lastGrabs[i]) OnGrabReleased?.Invoke(i == 0, targetDevices[i]);

            lastGrabs[i] = grab;
        }
    }

    /// <summary>
    /// 손에 들고 있는게 있는지 확인
    /// </summary>
    /// <param name="grasp">왼손: 0, 오른손: 1</param>
    /// <returns>손에 뭔가 있으면 true, 아니면 false</returns>
    /// <exception cref="System.IndexOutOfRangeException">grasp에 0이나 1이 아닌 수를 집어넣은 경우</exception>
    public bool GrabOccupied(int grasp)
    {
        return directInteractors[grasp].interactablesHovered.Count > 0
            || directInteractors[grasp].interactablesSelected.Count > 0;
    }

    /// <summary>
    /// 손에 들고 있는게 있는지 확인
    /// </summary>
    /// <param name="left">왼손: true, 오른손: false</param>
    /// <returns>손에 뭔가 있으면 true, 아니면 false</returns>
    public bool GrabOccupied(bool left) => GrabOccupied(left ? 0 : 1);

    /// <summary>
    /// 잡기 행동의 이벤트
    /// </summary>
    /// <param name="left">왼쪽 손으로 잡았는지 오른손이었는지 여부</param>
    /// <param name="device">잡기를 한 입력 장치 정보</param>
    public delegate void GrabHandler(bool left, InputDevice device);

    /// <summary>
    /// 잡았을 때 발생하는 이벤트
    /// </summary>
    public GrabHandler OnGrabbed;

    /// <summary>
    /// 잡은 걸 놓았을 때 발생하는 이벤트
    /// </summary>
    public GrabHandler OnGrabReleased;

    /// <summary>
    /// 손 애니메이션을 설정
    /// </summary>
    public void RequestHandAnimation(int grasp, HandAnimator.SpecialAnimation anim)
        => GetHandAnimator(grasp).SetSpecialAnimation(anim);

    /// <summary>
    /// 손 애니메이션을 설정
    /// </summary>
    public void RequestHandAnimation(bool left, HandAnimator.SpecialAnimation anim)
        => RequestHandAnimation(left ? 0 : 1, anim);

    /// <summary>
    /// <see cref="InputDevice"/>가 어느 손인지 확인
    /// </summary>
    public static int GetHand(InputDevice device)
    {
        if (((uint)device.characteristics & (uint)InputDeviceCharacteristics.Left) > 0)
            return 0;
        if (((uint)device.characteristics & (uint)InputDeviceCharacteristics.Right) > 0)
            return 1;
        return -1;
    }
}

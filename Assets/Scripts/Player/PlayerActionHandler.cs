using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// �÷��̾� ���� ó��
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
    /// ������ VR �Է� ��ġ�� ������
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
    /// ���� �����ӿ��� ��� ���̾����� ���� (index 0: ����, 1: ������)
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
    /// �տ� ��� �ִ°� �ִ��� Ȯ��
    /// </summary>
    /// <param name="grasp">�޼�: 0, ������: 1</param>
    /// <returns>�տ� ���� ������ true, �ƴϸ� false</returns>
    /// <exception cref="System.IndexOutOfRangeException">grasp�� 0�̳� 1�� �ƴ� ���� ������� ���</exception>
    public bool GrabOccupied(int grasp)
    {
        return directInteractors[grasp].interactablesHovered.Count > 0
            || directInteractors[grasp].interactablesSelected.Count > 0;
    }

    /// <summary>
    /// �տ� ��� �ִ°� �ִ��� Ȯ��
    /// </summary>
    /// <param name="left">�޼�: true, ������: false</param>
    /// <returns>�տ� ���� ������ true, �ƴϸ� false</returns>
    public bool GrabOccupied(bool left) => GrabOccupied(left ? 0 : 1);

    /// <summary>
    /// ��� �ൿ�� �̺�Ʈ
    /// </summary>
    /// <param name="left">���� ������ ��Ҵ��� �������̾����� ����</param>
    /// <param name="device">��⸦ �� �Է� ��ġ ����</param>
    public delegate void GrabHandler(bool left, InputDevice device);

    /// <summary>
    /// ����� �� �߻��ϴ� �̺�Ʈ
    /// </summary>
    public GrabHandler OnGrabbed;

    /// <summary>
    /// ���� �� ������ �� �߻��ϴ� �̺�Ʈ
    /// </summary>
    public GrabHandler OnGrabReleased;

    /// <summary>
    /// �� �ִϸ��̼��� ����
    /// </summary>
    public void RequestHandAnimation(int grasp, HandAnimator.SpecialAnimation anim)
        => GetHandAnimator(grasp).SetSpecialAnimation(anim);

    /// <summary>
    /// �� �ִϸ��̼��� ����
    /// </summary>
    public void RequestHandAnimation(bool left, HandAnimator.SpecialAnimation anim)
        => RequestHandAnimation(left ? 0 : 1, anim);

    /// <summary>
    /// <see cref="InputDevice"/>�� ��� ������ Ȯ��
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

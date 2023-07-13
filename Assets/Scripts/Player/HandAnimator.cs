using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 컨트롤러 조작에 맞게 손의 애니메이션을 구현
/// </summary>
[RequireComponent(typeof(Animator))]
public class HandAnimator : MonoBehaviour
{
    [SerializeField]
    private InputDeviceCharacteristics inputDeviceCharacteristics;

    private Animator anim;
    private InputDevice targetDevice;

    private void Start()
    {
        anim = GetComponent<Animator>();

        InitDevice();
    }

    private void InitDevice()
    {
        List<InputDevice> devices = new();
        InputDevices.GetDevicesWithCharacteristics(inputDeviceCharacteristics, devices);

        if (devices.Count < 1) return;
        targetDevice = devices[0];
    }

    private void Update()
    {
        if (!targetDevice.isValid) { InitDevice(); return; }

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            anim.SetFloat("Trigger", triggerValue);
        else
            anim.SetFloat("Trigger", 0f);

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            anim.SetFloat("Grip", gripValue);
        else
            anim.SetFloat("Grip", 0f);
    }

    public void SetSpecialAnimation(SpecialAnimation newAnim)
    {
        if (newAnim == curAnim) return;
        if (curAnim != SpecialAnimation.None) anim.SetBool(curAnim.ToString(), false);
        if (newAnim != SpecialAnimation.None) anim.SetBool(newAnim.ToString(), true);
        curAnim = newAnim;
    }

    private SpecialAnimation curAnim = SpecialAnimation.None;

    public enum SpecialAnimation
    {
        None = -1,
        GripTicket,
        GripRice,
        GripHalter,
    }

    public static void RequestAnimation(IXRSelectInteractor interactor, SpecialAnimation anim)
        => interactor.transform.parent.GetComponentInChildren<HandAnimator>().SetSpecialAnimation(anim);
}

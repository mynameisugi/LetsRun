using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� ����ý��� �� ��ȣ�ۿ��� ���� Ȧ��
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public Transform xrOrigin = null;

    public bool IsRiding {
        get => isRiding;
        set
        {
            if (value) handWatch.RequestModeSwitch(HandWatchController.Mode.Main);
            isRiding = value;
        }
    }
    private bool isRiding = false;

    /// <summary>
    /// <see cref="PlayerManager"/> �ν��Ͻ�
    /// </summary>
    public static PlayerManager Instance()
        => GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerManager>();

    /// <summary>
    /// �÷��̾� ���� ��ġ
    /// </summary>
    public static Transform InstanceOrigin()
        => Instance().xrOrigin;

    [SerializeField]
    private PlayerActionHandler actionHandler = null;

    [SerializeField]
    private HandWatchController handWatch = null;

    /// <summary>
    /// �÷��̾� �׼� �ڵ鷯
    /// </summary>
    public static PlayerActionHandler Action() => Instance().actionHandler;

    
}

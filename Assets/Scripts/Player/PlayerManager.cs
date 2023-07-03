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
    /// ���� ���� ��
    /// </summary>
    public HorseController horse;

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
    public PlayerActionHandler Action() => actionHandler;

    /// <summary>
    /// �÷��̾� �κ��丮
    /// </summary>
    public PlayerInventory Inventory() => inventory;

    private PlayerInventory inventory = null;

    private void Start()
    {
        inventory = new PlayerInventory(this);
    }

}

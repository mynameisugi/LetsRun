using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 서브시스템 간 상호작용을 위한 홀더
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
    /// 소유 중인 말
    /// </summary>
    public HorseController horse;

    /// <summary>
    /// <see cref="PlayerManager"/> 인스턴스
    /// </summary>
    public static PlayerManager Instance()
        => GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerManager>();

    /// <summary>
    /// 플레이어 실제 위치
    /// </summary>
    public static Transform InstanceOrigin()
        => Instance().xrOrigin;

    [SerializeField]
    private PlayerActionHandler actionHandler = null;

    [SerializeField]
    private HandWatchController handWatch = null;

    /// <summary>
    /// 플레이어 액션 핸들러
    /// </summary>
    public PlayerActionHandler Action() => actionHandler;

    /// <summary>
    /// 플레이어 인벤토리
    /// </summary>
    public PlayerInventory Inventory() => inventory;

    private PlayerInventory inventory = null;

    private void Start()
    {
        inventory = new PlayerInventory(this);
    }

}

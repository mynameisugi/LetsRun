using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 서브시스템 간 상호작용을 위한 홀더
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public Transform xrOrigin = null;

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

    /// <summary>
    /// 플레이어 액션 핸들러
    /// </summary>
    public static PlayerActionHandler Action() => Instance().actionHandler;

    
}

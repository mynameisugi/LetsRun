using UnityEngine;

/// <summary>
/// 이 트랜스폼을 원하는 트랜스폼과 일치시킴
/// </summary>
public class SnapTransform : MonoBehaviour
{
    /// <summary>
    /// 참조하는 트랜스폼
    /// </summary>
    public Transform snapTransform = null;

    /// <summary>
    /// 위치를 일치시킬 것인지
    /// </summary>
    public bool snapPosition = true;

    /// <summary>
    /// 회전을 일치시킬 것인지
    /// </summary>
    public bool snapRotation = false;

    private void Update()
    {
        if (!snapTransform) return;
        if (snapPosition) transform.position = snapTransform.position;
        if (snapRotation) transform.rotation = snapTransform.rotation;
    }
}

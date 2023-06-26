using UnityEngine;

/// <summary>
/// �� Ʈ�������� ���ϴ� Ʈ�������� ��ġ��Ŵ
/// </summary>
public class SnapTransform : MonoBehaviour
{
    /// <summary>
    /// �����ϴ� Ʈ������
    /// </summary>
    public Transform snapTransform = null;

    /// <summary>
    /// ��ġ�� ��ġ��ų ������
    /// </summary>
    public bool snapPosition = true;

    /// <summary>
    /// ȸ���� ��ġ��ų ������
    /// </summary>
    public bool snapRotation = false;

    private void Update()
    {
        if (!snapTransform) return;
        if (snapPosition) transform.position = snapTransform.position;
        if (snapRotation) transform.rotation = snapTransform.rotation;
    }
}

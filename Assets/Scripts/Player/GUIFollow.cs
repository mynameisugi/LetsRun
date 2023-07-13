using UnityEngine;

public class GUIFollow : MonoBehaviour
{
    [SerializeField]
    private Transform offset;

    private void OnEnable()
    {
        var pos = transform.localPosition;
        pos.y = offset.localPosition.y;
        var rot = offset.localRotation.eulerAngles;
        rot.x = 0f; rot.z = 0f;
        transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(rot));
    }
}

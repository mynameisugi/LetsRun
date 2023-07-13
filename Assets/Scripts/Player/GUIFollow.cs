using UnityEngine;

public class GUIFollow : MonoBehaviour
{
    [SerializeField]
    private Transform offset;
    [SerializeField, Range(0f, 90f)]
    private float rotRange = 40f;

    private void Update()
    {
        var pos = transform.localPosition;
        pos.y = offset.localPosition.y;
        var rot = transform.localRotation.eulerAngles;
        rot.x = 0f; rot.z = 0f;
        var oRot = offset.localRotation.eulerAngles;
        if (Mathf.DeltaAngle(oRot.y, rot.y) > rotRange)
            rot.y = Mathf.MoveTowardsAngle(rot.y, oRot.y, Time.deltaTime * 60f);
        transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(rot));
    }
}

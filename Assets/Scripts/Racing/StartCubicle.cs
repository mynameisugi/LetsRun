using UnityEngine;

public class StartCubicle : MonoBehaviour
{
    [SerializeField]
    private Transform[] starts = new Transform[8];

    public Transform GetStartPos(int i) => starts[i];
}

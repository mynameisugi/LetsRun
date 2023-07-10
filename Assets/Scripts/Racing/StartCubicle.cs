using UnityEngine;

public class StartCubicle : MonoBehaviour
{
    [SerializeField]
    private Transform[] starts = new Transform[8];

    [SerializeField]
    private Animator cubicleAnim;

    [SerializeField]
    private GameObject startBlock;

    public Transform GetStartPos(int i) => starts[i];

    public void OpenGate()
    {
        cubicleAnim.SetBool("Open", true);
        startBlock.SetActive(false);
    }

    public void CloseGate()
    {
        if (!gameObject.activeSelf) return;
        cubicleAnim.SetBool("Open", false);
        startBlock.SetActive(true);
    }
}

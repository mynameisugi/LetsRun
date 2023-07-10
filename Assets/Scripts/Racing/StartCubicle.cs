using UnityEngine;

public class StartCubicle : MonoBehaviour
{
    [SerializeField]
    private Transform[] starts = new Transform[8];

    [SerializeField]
    private GameObject startBlock;

    public Transform GetStartPos(int i) => starts[i];

    private void Start()
    {
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP, () =>
        {
            GetComponent<Animator>().SetBool("Open", true);
            startBlock.SetActive(false);
        });
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP - 80, () =>
        {
            if (!gameObject.activeSelf) return;
            GetComponent<Animator>().SetBool("Open", false);
            startBlock.SetActive(true);
        });
    }
}

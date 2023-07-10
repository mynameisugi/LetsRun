using System.Collections;
using UnityEngine;

public class CountDown : MonoBehaviour{

    private Animator animator;
    [SerializeField] private Material matOn;
    [SerializeField] private Material matOff;
    [SerializeField] private MeshRenderer[] meshRen;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }
    private void Start()
    {
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP - 10, StartCountDown);
    }

    private void StartCountDown()
    {
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine() {
        //10�� ��
        const string UP = "up";
        // �����ִ� ���׸���� ����
        foreach (MeshRenderer mr in meshRen) mr.material = matOff;
        // ��ġ�� �ִϸ��̼� ����
        animator.SetBool(UP, true);
        yield return new WaitForSeconds (5f);
        //5�� ��
        // ī��Ʈ �ٿ� ����
        for ( int i = 0; i < 5; ++i) {
            //1�� ���
            yield return new WaitForSeconds(1f);
            //���� �������� �� ����
            meshRen[i].material = matOn;
            meshRen[^(i + 1)].material = matOn;
        }

        animator.SetBool(UP, false);
    }
}

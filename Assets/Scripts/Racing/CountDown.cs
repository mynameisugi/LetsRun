using System.Collections;
using UnityEngine;

public class CountDown : MonoBehaviour{

    private Animator animator;

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
        // ��ġ�� �ִϸ��̼� ����
        const string UP = "up";
        animator.SetBool(UP, true);
        yield return new WaitForSeconds (5f);
        //5�� ��
        // ī��Ʈ �ٿ� ����
        // 1�� ���
        // 4�� ��
        // ī��Ʈ �ٿ� �� �־� ����
    }
}

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
        //10초 전
        // 펼치는 애니메이션 실행
        const string UP = "up";
        animator.SetBool(UP, true);
        yield return new WaitForSeconds (5f);
        //5초 전
        // 카운트 다운 시작
        // 1초 대기
        // 4초 전
        // 카운트 다운 한 쌍씩 켜짐
    }
}

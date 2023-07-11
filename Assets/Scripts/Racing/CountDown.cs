using System.Collections;
using UnityEngine;

public class CountDown : MonoBehaviour{

    private Animator animator;
    [SerializeField] private Material matOn;
    [SerializeField] private Material matOff;
    [SerializeField] private MeshRenderer[] meshRen;

    [Header("SoundEffects")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clipCount;
    [SerializeField] private AudioClip clipEnd;
    [SerializeField] private AudioClip clipGun;
    [SerializeField] private AudioSource[] sourceGuns;


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
        const string UP = "up";
        // 꺼져있는 머테리얼로 변경
        foreach (MeshRenderer mr in meshRen) mr.material = matOff;
        // 펼치는 애니메이션 실행
        animator.SetBool(UP, true);
        yield return new WaitForSeconds (5f);
        //5초 전
        // 카운트 다운 시작
        for ( int i = 0; i < 5; ++i) {
            //1초 대기
            yield return new WaitForSeconds(1f);
            //불이 양쪽으로 불 켜짐
            meshRen[i].material = matOn;
            meshRen[^(i + 1)].material = matOn;
            //소리 재생
            if (i < 4)
            {
                source.clip = clipCount;
                source.volume = GameSettings.Values.SE;
                source.Play();
            }
        }
        source.clip = clipEnd;
        source.volume = GameSettings.Values.SE;
        source.Play();
        foreach(var sg in sourceGuns)
        {
            sg.clip = clipGun;
            sg.volume = GameSettings.Values.SE;
            sg.Play();
        }

        animator.SetBool(UP, false);
    }
}

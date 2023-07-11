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
            //�Ҹ� ���
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

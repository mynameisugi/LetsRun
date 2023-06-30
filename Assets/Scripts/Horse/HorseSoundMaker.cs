using UnityEngine;
using static HorseSoundMaker.Foot;

/// <summary>
/// 말의 소리를 재생
/// </summary>
public class HorseSoundMaker : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] feetAudios;

    [SerializeField]
    private AudioSource headAudio;

    public enum Foot : int
    {
        RearLeft = 0,
        RearRight = 1,
        FrontLeft = 2,
        FrontRight = 3
    }

    private AudioSource GetFootAudioSource(Foot foot) => feetAudios[(int)foot];

    // 예시. 이를 참고해 OnFootWalkStep, OnFootCanterStep, OnFootGallopStep을 만들면 됨.
    // 애니메이션은 Assets/Resources/Models/Horse에 H_로 시작하는 애니메이션 5개가 있고,
    // 그 중 H_Trot은 내가 예시로 Animation Event를 세팅해뒀음.
    public void OnFootTrotStep(int i)
    {
        // Trot 애니메이션은 발굽 3개가 동시에 닿아서 이렇게 배열을 만듦
        Foot[] feet = i switch
        {
            0 => new Foot[] { RearLeft, RearRight, FrontRight },
            1 => new Foot[] { RearLeft, RearRight, FrontLeft },
            2 => new Foot[] { RearLeft, RearRight, FrontRight },
            _ => new Foot[] { RearLeft, RearRight, FrontLeft }
        };
        Debug.Log($"{transform.root.gameObject.name} OnFootTrotStep {i}");
        // 바닥에 닿은 발굽마다 소리 재생
        foreach (var foot in feet)
        {
            AudioSource source = GetFootAudioSource(foot);
            AudioClip clip = GetFootsound(source);
            source.clip = clip;
            source.pitch = Random.Range(0.8f, 1.2f);
            source.Play();
        }
    }

    // 발소리를 Resources에 추가하고 이를 고쳐야 함.
    private static AudioClip GetFootsound(AudioSource source)
    {
        AudioClip clip;
        // 이건 땅 아래의 재질에 따라 다른 발소리를 내는 예시인데,
        // 이걸 빼고 그냥 랜덤한 발소리를 재생하는 걸로 일단 해도 됨.
        if (Physics.Raycast(source.transform.position + Vector3.up * 0.5f, Vector3.down,
                        out RaycastHit info, 1f, LayerMask.GetMask("Ground"))) // 발 아래의 땅을 확인
        {
            // info를 사용해 아래 땅의 재질에 맞는 발소리 재생
            clip = Resources.Load("땅에 맞는 발소리") as AudioClip;
        }
        else // 정보를 못 구하면 그냥 기본 발소리 재생
            clip = Resources.Load("기본 발소리") as AudioClip;
        
        return clip;
    }

    public void OnHorseNeigh()
    {
        // 말 히힝거리는 소리 재생
        headAudio.Play();
    }

}
using UnityEngine;
using static HorseSoundMaker.Foot;

/// <summary>
/// 말의 소리를 재생
/// </summary>
public class HorseSoundMaker : MonoBehaviour
{
    [SerializeField]
    private HorseController horse;

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

    private static bool IsLeft(Foot foot) => foot == FrontLeft || foot == RearLeft;

    private AudioSource GetFootAudioSource(Foot foot) => feetAudios[(int)foot];



    // 발소리를 Resources에 추가하고 이를 고쳐야 함.
    private static AudioClip GetFootsound(AudioSource source)
    {
        AudioClip clip;
        clip = Resources.Load("Sounds/Horse/Walksound" + Random.Range(1, 5).ToString()) as AudioClip;

        // 이건 땅 아래의 재질에 따라 다른 발소리를 내는 예시인데,
        // 이걸 빼고 그냥 랜덤한 발소리를 재생하는 걸로 일단 해도 됨.
        /*
        if (Physics.Raycast(source.transform.position + Vector3.up * 0.5f, Vector3.down,
                        out RaycastHit info, 1f, LayerMask.GetMask("Ground"))) // 발 아래의 땅을 확인
        {
            // info를 사용해 아래 땅의 재질에 맞는 발소리 재생
            clip = Resources.Load("땅에 맞는 발소리") as AudioClip;
        }
        else // 정보를 못 구하면 그냥 기본 발소리 재생
            clip = Resources.Load("기본 발소리") as AudioClip;
        */
        return clip;
    }

    public void OnHorseNeigh()
    {
        // 말 히힝거리는 소리 재생
        headAudio.Play();
    }

    public void OnFootWalkStep(int i)
    {
        Foot foot = i switch
        {
            0 => FrontLeft,
            1 => RearRight,
            2 => FrontRight,
            _ => RearLeft
        };

        AudioSource source = GetFootAudioSource(foot);
        AudioClip clip = GetFootsound(source);
        source.clip = clip;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.volume = 0.3f;
        if (!horse.isPlayerRiding) source.volume *= 0.5f;
        source.Play();

        bool left = IsLeft(foot);
        SendHapticFeedback(left ? 0.03f : 0f, left ? 0f : 0.03f);
    }

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

        // 바닥에 닿은 발굽마다 소리 재생
        int l = 0, r = 0;
        foreach (var foot in feet)
        {
            AudioSource source = GetFootAudioSource(foot);
            AudioClip clip = GetFootsound(source);
            source.clip = clip;
            source.pitch = Random.Range(0.9f, 1.1f);
            source.volume = 0.2f;
            if (!horse.isPlayerRiding) source.volume *= 0.5f;
            source.Play();
            if (IsLeft(foot)) ++l; else ++r;
        }

        SendHapticFeedback(0.05f * l, 0.05f * r);
    }

    public void OnFootCanterStep(int i)
    {
        Foot[] feet = i switch
        {
            0 => new Foot[] { FrontRight },
            1 => new Foot[] { FrontLeft },
            _ => new Foot[] { RearLeft, RearRight }
        };

        int l = 0, r = 0;
        foreach (var foot in feet)
        {
            AudioSource source = GetFootAudioSource(foot);
            AudioClip clip = GetFootsound(source);
            source.clip = clip;
            source.pitch = Random.Range(0.9f, 1.1f);
            source.volume = 0.5f;
            if (!horse.isPlayerRiding) source.volume *= 0.5f;
            source.Play();
            if (IsLeft(foot)) ++l; else ++r;
        }
        SendHapticFeedback(0.08f * l, 0.08f * r);
    }

    public void OnFootGallopStep(int i)
    {
        Foot foot = i switch
        {
            0 => FrontRight,
            1 => RearLeft,
            2 => FrontLeft,
            _ => RearRight
        };

        AudioSource source = GetFootAudioSource(foot);
        AudioClip clip = GetFootsound(source);
        source.clip = clip;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.volume = 0.8f;
        if (!horse.isPlayerRiding) source.volume *= 0.5f;
        source.Play();

        bool left = IsLeft(foot);
        SendHapticFeedback(left ? 0.2f : 0f, left ? 0f : 0.2f);
    }

    private void SendHapticFeedback(float leftAmplitude, float rightAmplitude)
    {
        if (!horse.isPlayerRiding) return;
        if (leftAmplitude > 0f) horse.playerAction.GetDevice(0).SendHapticImpulse(0, leftAmplitude, 0.05f);
        if (rightAmplitude > 0f) horse.playerAction.GetDevice(1).SendHapticImpulse(0, rightAmplitude, 0.05f);
    }
}
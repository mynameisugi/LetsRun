﻿using UnityEngine;
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

        if (Physics.Raycast(source.transform.position + Vector3.up * 0.5f, Vector3.down,
                        out RaycastHit info, 1f, LayerMask.GetMask("Ground")))
        {
            // info를 사용해 아래 땅의 재질에 맞는 발소리 재생
            if (info.collider.CompareTag("Stone"))
            {
                clip = Resources.Load("Sounds/Horse/StoneFootstep" + Random.Range(1, 3).ToString()) as AudioClip;
            }
            else if (info.collider.CompareTag("Grass"))
            {
                clip = Resources.Load("Sounds/Horse/GrassFootstep" + Random.Range(1, 3).ToString()) as AudioClip;
            }
            else if (info.collider.CompareTag("Dirt"))
            {
                clip = Resources.Load("Sounds/Horse/DirtFootstep" + Random.Range(1, 4).ToString()) as AudioClip;
            }
            else // 정보를 못 구하면 그냥 기본 발소리 재생
            {
                clip = Resources.Load("Sounds/Horse/Walksound" + Random.Range(1, 5).ToString()) as AudioClip;
            }
        }
        else
        {
            clip = Resources.Load("Sounds/Horse/Walksound" + Random.Range(1, 5).ToString()) as AudioClip;
        }
        return clip;
    }

    public void OnHorseJump()
    {
        headAudio.clip = Resources.Load("Sounds/Wind/windsound" + Random.Range(1, 4).ToString()) as AudioClip;
        headAudio.volume = GameSettings.Values.SE;
        headAudio.Play();
        SendHapticFeedback(0.7f, 0.7f, 1.0f);
    }

    public void OnHorseNeigh()
    {
        headAudio.clip = Resources.Load("Sounds/Horse/horsevoice3") as AudioClip;
        headAudio.volume = 0.7f * GameSettings.Values.SE;
        headAudio.Play();
        SendHapticFeedback(0.5f, 0.5f, 0.3f);
    }

    public void OnHorseDistress()
    {
        headAudio.clip = Resources.Load("Sounds/Horse/horsevoice" + Random.Range(1, 3).ToString()) as AudioClip;
        headAudio.volume = 0.8f * GameSettings.Values.SE;
        headAudio.Play();
        SendHapticFeedback(0.8f, 0.8f, 0.4f);
    }

    public void OnHorsePurr()
    {
        headAudio.clip = Resources.Load("Sounds/Horse/horsevoice4") as AudioClip;
        headAudio.volume = 0.9f * GameSettings.Values.SE;
        headAudio.Play();
        SendHapticFeedback(0.5f, 0.5f, 0.2f);
    }

    public void OnFootWalkStep(int i)
    {
        if (horse.CurMode < 0.5f) return; // 안 걷는 중
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
        source.volume = 0.6f;
        source.volume *= GameSettings.Values.SE;
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
            source.volume = 0.4f;
            source.volume *= GameSettings.Values.SE;
            source.Play();
            if (IsLeft(foot)) ++l; else ++r;
        }

        SendHapticFeedback(0.05f * l, 0.05f * r);
    }

    public void OnFootCanterStep(int i)
    {
        Foot[] feet = i switch
        {
            0 => new Foot[] { FrontRight, RearLeft },
            1 => new Foot[] { FrontLeft },
            _ => new Foot[] { RearRight }
        };

        int l = 0, r = 0;
        foreach (var foot in feet)
        {
            AudioSource source = GetFootAudioSource(foot);
            AudioClip clip = GetFootsound(source);
            source.clip = clip;
            source.pitch = Random.Range(0.9f, 1.1f);
            source.volume = 0.9f;
            source.volume *= GameSettings.Values.SE;
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
        source.volume = 1.2f;
        source.volume *= GameSettings.Values.SE;
        source.Play();

        bool left = IsLeft(foot);
        SendHapticFeedback(left ? 0.2f : 0f, left ? 0f : 0.2f);
    }

    public void OnFootJumpStep(int i)
    {
        Foot[] feet = i switch
        {
            0 => new Foot[] { FrontRight, FrontLeft },
            _ => new Foot[] { RearRight, RearLeft }
        };

        int l = 0, r = 0;
        foreach (var foot in feet)
        {
            AudioSource source = GetFootAudioSource(foot);
            AudioClip clip = GetFootsound(source);
            source.clip = clip;
            source.pitch = Random.Range(0.9f, 1.1f);
            source.volume = 0.9f;
            source.volume *= GameSettings.Values.SE;
            source.Play();
            if (IsLeft(foot)) ++l; else ++r;
        }
        SendHapticFeedback(0.08f * l, 0.08f * r);
    }

    private void SendHapticFeedback(float leftAmplitude, float rightAmplitude, float length = 0.05f)
    {
        if (!horse.isPlayerRiding || !GameSettings.Values.rumble) return;
        if (leftAmplitude > 0f) horse.playerAction.GetDevice(0).SendHapticImpulse(0, leftAmplitude, length);
        if (rightAmplitude > 0f) horse.playerAction.GetDevice(1).SendHapticImpulse(0, rightAmplitude, length);
    }
}
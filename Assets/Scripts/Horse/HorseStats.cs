using System;
using UnityEngine;

[Serializable]
public struct HorseStats
{
    public HorseStats(float stamina = 2.1f)
    {
        speeds = new float[] { 2f, 4f, 6f, 16f, 16f };
        gallopAmount = stamina;
    }

    /// <summary>
    /// 평보 속도 (1단계)
    /// </summary>
    public float SpeedWalk { get => speeds[0]; set => speeds[0] = value; }
    /// <summary>
    /// 속보 속도 (2단계)
    /// </summary>
    public float SpeedTrot { get => speeds[1]; set => speeds[1] = value; }
    /// <summary>
    /// 구보 속도 (3단계)
    /// </summary>
    public float SpeedCanter { get => speeds[2]; set => speeds[2] = value; }
    /// <summary>
    /// 습보 속도 (4단계 - 전력 질주)
    /// </summary>
    public float SpeedGallop { get => speeds[3]; set { speeds[3] = value; speeds[4] = value; } }

    [SerializeField]
    private float[] speeds;

    /// <summary>
    /// 현재 단계에 맞는 속도값
    /// </summary>
    /// <param name="mode">0단계: 정지 ~ 4단계: 습보</param>
    /// <returns></returns>
    public float GetSpeed(float mode)
    {
        if (mode < 1f) return Mathf.Lerp(0f, speeds[0], mode);
        mode = Mathf.Clamp(mode - 1f, 0f, 3f);
        int intMode = Mathf.FloorToInt(mode); float offset = mode - intMode;
        //if (offset == 0f) return speeds[intMode];
        return Mathf.Lerp(speeds[intMode], speeds[intMode + 1], offset);
    }

    /// <summary>
    /// 습보 개수
    /// </summary>
    public float gallopAmount;
}
using System;
using UnityEngine;

[Serializable]
public struct HorseStats
{
    public HorseStats(float stamina = 2.1f)
    {
        speeds = new float[] { 2f, 4f, 6f, 16f, 16f };
        gallopAmount = stamina;
        steerStrength = 30f;
        skin = 0;
    }

    /// <summary>
    /// 평보 속도 (1단계)
    /// </summary>
    public float SpeedWalk { get => speeds[0]; set => speeds[0] = Mathf.Clamp(value, MinStats[0], MaxStats[0]); }
    /// <summary>
    /// 속보 속도 (2단계)
    /// </summary>
    public float SpeedTrot { get => speeds[1]; set => speeds[1] = Mathf.Clamp(value, MinStats[1], MaxStats[1]); }
    /// <summary>
    /// 구보 속도 (3단계)
    /// </summary>
    public float SpeedCanter { get => speeds[2]; set => speeds[2] = Mathf.Clamp(value, MinStats[2], MaxStats[2]); }
    /// <summary>
    /// 습보 속도 (4단계 - 전력 질주)
    /// </summary>
    public float SpeedGallop
    {
        get => speeds[3];
        set
        {
            speeds[3] = Mathf.Clamp(value, MinStats[3], MaxStats[3]);
            speeds[4] = Mathf.Clamp(value, MinStats[3], MaxStats[3]);
        }
    }

    /// <summary>
    /// 습보 개수
    /// </summary>
    public float GallopAmount { get => gallopAmount; set => gallopAmount = Mathf.Clamp(value, MinStats[4], MaxStats[4]); }

    /// <summary>
    /// 커브 속도
    /// </summary>
    public float SteerStrength { get => steerStrength * 1.5f; set => steerStrength = Mathf.Clamp(value, MinStats[5], MaxStats[5]); }

    [SerializeField]
    private float[] speeds;

    public readonly static float[] MaxStats = new float[] { 3f, 5f, 9f, 24f, 10f, 50f };
    public readonly static float[] MinStats = new float[] { 1f, 3f, 4f, 12f, 1f, 20f };

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

    private float gallopAmount;

    private float steerStrength;

    /// <summary>
    /// 스킨 번호
    /// </summary>
    public int skin;


    public Material GetSkin()
    {
        if (skinLibrary == null)
        {
            skinLibrary = new Material[10];
            for (int i = 0; i < 10; ++i)
                skinLibrary[i] = Resources.Load($"Materials/Horse/PolygonHorse_{i + 1:00}") as Material;
        }
        return skinLibrary[skin];
    }

    private static Material[] skinLibrary;

}
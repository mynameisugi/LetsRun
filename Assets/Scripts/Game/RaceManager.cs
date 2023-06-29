using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject horsePrefab;

    public enum RaceType
    {
        Easy, // 500m
        Normal, // 1000m
        Hard // 1500m
    }

    private void Awake()
    {
        stage = 0; // 준비
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP, StartRace);
    }

    private void StartRace()
    {

    }

    public RaceType type;

    private int stage;

    private void Update()
    {
        switch (stage)
        {
            case 0: break;
        }
    }

    private HorseStats CreateRandomStats()
    {
        HorseStats stats = new(type switch
        {
            RaceType.Hard => Random.Range(5.6f, 9.1f),
            RaceType.Normal => Random.Range(2.6f, 5.1f),
            _ => Random.Range(1.6f, 3.1f)
        })
        {
            SpeedWalk = Random.Range(1.5f, 2.5f),
            SpeedTrot = Random.Range(3.1f, 4.9f),
            SpeedCanter = type switch
            {
                RaceType.Hard => Random.Range(6.5f, 8.5f),
                RaceType.Normal => Random.Range(5.5f, 7.5f),
                _ => Random.Range(4.5f, 6.5f)
            },
            SpeedGallop = type switch
            {
                RaceType.Hard => Random.Range(18f, 24f),
                RaceType.Normal => Random.Range(14f, 20f),
                _ => Random.Range(12f, 18f)
            },
            skin = Random.Range(0, 10),
            steerStrength = type switch { RaceType.Hard => 50f, RaceType.Normal => 40f, _ => 30f }
        };

        return stats;
    }

}
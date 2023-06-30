using System;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using static RaceManager;
using Random = UnityEngine.Random;

/// <summary>
/// 현재 경기를 관리하는 매니저
/// </summary>
public class Race : MonoBehaviour
{
    [SerializeField]
    private GameObject horsePrefab;

    private void Start()
    {
        Debug.Log($"Race Ready: {info.type}");

        stage = RaceStage.Prepare; // 준비

        // 참가자 생성
        entries = new HorseController[8];
        entryFilled = 0;
        Invoke(nameof(CreateEntry), 0.1f);

        // 경기 시작 대기
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP, StartRace);
    }

    private void CreateEntry()
    {
        // 새로운 참가자 말 생성
        var entryObj = Instantiate(horsePrefab);
        entryObj.name = $"Horse RaceEntry{entryFilled + 1}";
        entries[entryFilled] = entryObj.GetComponent<HorseController>();
        entries[entryFilled].stats = info.CreateRandomStats();
        entries[entryFilled].playerRidable = false;
        entryObj.transform.position = info.GetStartPos(entryFilled).position;
        entryObj.transform.rotation = info.GetStartPos(entryFilled).rotation;
        // 말에게 경기 정보 전달
        entries[entryFilled].NPCJoinRace(this);
        ++entryFilled;
        // 다음 참가자 0.1초 뒤 생성
        if (entryFilled < entries.Length) Invoke(nameof(CreateEntry), 0.1f);
    }

    private void StartRace()
    {
        Debug.Log($"Race Start! {info.type}");
        stage = RaceStage.Racing;
        foreach (var entry in entries) if (entry) entry.StartRace();
    }

    public RaceInfo info;

    private RaceStage stage;

    private enum RaceStage
    {
        Prepare, // 1분전. 경쟁자 말을 생성하고 케이지에 배치
        Racing, // 경주중/직후. 트랙 안의 말은 달리고, 도착한 말은 텐트로 이동.
        Clean // 플레이어가 경주를 안 했거나 텐트 밖으로 이동하면 경쟁자 말을 삭제하고 이 레이스 종료.
    }

    private HorseController[] entries;
    private int entryFilled;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("DEBUG Race Start");
            StartRace();
        }
    }

    [Serializable]
    public struct RaceInfo
    {
        /// <summary>
        /// 레이스 종류
        /// </summary>
        [SerializeField] public RaceType type;
        /// <summary>
        /// 레이스 시작점
        /// </summary>
        [SerializeField] public StartCubicle start;
        /// <summary>
        /// 레이스 경로
        /// </summary>
        [SerializeField] public BoxCollider[] trackNodes;

        public Transform GetStartPos(int i) => start.GetStartPos(i);

        public HorseStats CreateRandomStats()
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
}
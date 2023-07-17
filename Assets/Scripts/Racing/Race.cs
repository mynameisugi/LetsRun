using System;
using System.Collections.Generic;
using System.Linq;
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

    public RaceInfo info;

    [SerializeField]
    private AudioSource audienceBGM;
    [SerializeField]
    private AudioSource audienceSE;


    private void Start()
    {
        Debug.Log($"Race Ready: {info.type}");

        entries = new HorseController[8];
        Status = RaceStage.Prepare; // 준비

        info.end.playerRank = -1; // 플레이어 기록 리셋
        info.end.SetPrize(info.prize);

        info.start.gameObject.SetActive(true);
        info.start.CloseGate();

        // 참가자 생성
        entryFilled = 0;
        Invoke(nameof(CreateEntry), 0.1f);

        // 경기 시작 대기
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP, StartRace);
        audienceBGM.volume = GameSettings.Values.BGM * 0.1f;

        // 장애물 랜덤 켜기
        for (int i = 0; i < info.obstacles.Length; ++i)
        {
            if (info.obstacles[i] && Random.value < info.obstacleRate)
            {
                info.obstacles[i].SetActive(true);
                //++i;
            }
        }

        // PlayerManager.Instance().GUI.ShowRaceMap(this); // test
    }

    private void CreateEntry()
    {
        // 플레이어가 이미 차지한 자리면 스킵
        if (playerNum == entryFilled) goto Skip;
        // 새로운 참가자 말 생성
        var entryObj = Instantiate(horsePrefab);
        entryObj.name = $"Horse RaceEntry{entryFilled + 1}";
        entries[entryFilled] = entryObj.GetComponent<HorseController>();
        entries[entryFilled].stats = info.CreateRandomStats();
        entries[entryFilled].playerRidable = false;
        entryObj.transform.position = info.GetStartPos(entryFilled).position;
        entryObj.transform.rotation = info.GetStartPos(entryFilled).rotation;
        // 말에게 경기 정보 전달
        entries[entryFilled].NPCJoinRace(this, entryFilled + 1);
    Skip: ++entryFilled;
        // 다음 참가자 0.1초 뒤 생성
        if (entryFilled < entries.Length) Invoke(nameof(CreateEntry), 0.1f);
    }

    private void StartRace()
    {
        GameManager.Instance().Time.UnregisterEvent(TimeManager.LOOP, StartRace);
        info.start.OpenGate();
        Debug.Log($"Race Start! {info.type}");
        Status = RaceStage.Racing;
        foreach (var entry in entries) if (entry) entry.StartRace();
        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP - 90, DestroyRace);
    }

    private void DestroyRace()
    {
        Debug.Log($"Destroy Race! {info.type}");
        audienceBGM.volume = 0f;
        audienceSE.volume = 0f;

        if (GameManager.Instance().BGM.IsRaceBGM) GameManager.Instance().BGM.PlayNormalBGM();
        GameManager.Instance().Time.UnregisterEvent(TimeManager.LOOP - 90, DestroyRace);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        foreach (var entry in entries)
            if (entry) Destroy(entry.gameObject);
        foreach (var obst in info.obstacles)
            if (obst) obst.SetActive(false);
        if (info.start) info.start.gameObject.SetActive(false);
    }

    private int playerNum = -1;

    public void AddPlayer(int num)
    {
        playerNum = num;
        if (entries != null && entries[playerNum]) // 이미 NPC가 있으면 삭제
        {
            Destroy(entries[playerNum].gameObject);
            entries[playerNum] = null;
        }
    }

    public void JoinPlayer()
    {
        var playerHorse = PlayerManager.Instance().horse;
        playerHorse.OnPlayerRideRequest();
        var start = info.GetStartPos(playerNum);
        GameManager.Instance().BGM.PlayRaceBGM();
        GameManager.Instance().PlayFadeInAndOut(() =>
        {
            playerHorse.Teleport(start.position, start.rotation);
        }, null);

        PlayerManager.Instance().GUI.ShowRaceMap(this);
    }

    internal RaceStage Status { get; private set; }

    public enum RaceStage
    {
        Prepare, // 1분전. 경쟁자 말을 생성하고 케이지에 배치
        Racing, // 경주중/직후. 트랙 안의 말은 달리고, 도착한 말은 텐트로 이동.
        Clean // 플레이어가 경주를 안 했거나 텐트 밖으로 이동하면 경쟁자 말을 삭제하고 이 레이스 종료.
    }

    private HorseController[] entries;
    private int entryFilled;

    public HorseController GetEntry(int i)
    {
        if (playerNum == i) return PlayerManager.Instance().horse;
        return entries[i];
    }

    private float wow = 0f;
    private float curWow = 0f;

    public void AddWow(float wow) => this.wow += wow;

    private void Update()
    {
        if (Status == RaceStage.Clean)
        {
            audienceBGM.volume = Mathf.MoveTowards(audienceBGM.volume, 0f, Time.deltaTime * 0.1f);
            audienceSE.volume = Mathf.MoveTowards(audienceSE.volume, 0f, Time.deltaTime * 0.3f);
            int alive = entries.Count(x => x);
            if (alive == 0) DestroyRace();
            return;
        }
        if (Status == RaceStage.Racing)
        {
            if (wow == 0f && goalInfos.Count > 0)
            {
                audienceBGM.volume = Mathf.MoveTowards(audienceBGM.volume, 0f, Time.deltaTime * 0.1f);
                audienceSE.volume = Mathf.MoveTowards(audienceSE.volume, 0f, Time.deltaTime * 0.3f);
            }
            else
            {
                audienceBGM.volume = Mathf.MoveTowards(audienceBGM.volume, GameSettings.Values.BGM * 0.3f, Time.deltaTime);
                if (wow > 0f)
                {
                    curWow += Time.deltaTime;
                    wow -= Time.deltaTime;
                    if (wow < 0f) wow = 0f;
                }
                else
                {
                    curWow -= Time.deltaTime;
                    if (curWow < 0f) curWow = 0f;
                }
                audienceSE.volume = (Mathf.Clamp01(curWow) * 0.8f + 0.2f) * audienceSE.volume;
            }
        }

        if (Status == RaceStage.Prepare && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("DEBUG Race Start");
            StartRace();
        }
    }

    public int HorseGoal(HorseController horse)
    {
        int num = GetHorseEntryIndex(horse);
        if (num < 0) return -1; // 경주에 참가한 말이 아님
        foreach (var info in goalInfos) if (info.index == num) return -1; // 이미 도착한 말

        goalInfos.Add(new(horse, num, GameManager.Instance().Time.Now));
        if (goalInfos.Count == 1) AddWow(6f);
        if (horse.isPlayerRiding)
        {
            info.end.playerRank = goalInfos.Count; // 플레이어 등수 저장
            GameManager.Instance().BGM.PlayNormalBGM();
            PlayerManager.Instance().GUI.HideRaceMap();
        }
        if (goalInfos.Count == 8) Status = RaceStage.Clean;
        return goalInfos.Count;
        // Debug.Log($"Goal! {horse.gameObject.name} at {GameManager.Instance().Time.Now:0.00}");
    }

    private int GetHorseEntryIndex(HorseController horse)
    {
        int num = -1;
        if (horse.isPlayerRiding && playerNum >= 0)
        { num = playerNum; }
        else if (horse.isRacing)
        {
            for (int i = 0; i < entries.Length; ++i)
                if (entries[i] == horse)
                {
                    num = i;
                    // TODO: NPC 말을 도착지로 유도
                    break;
                }
        }

        return num;
    }

    public List<GoalInfo> goalInfos = new(8);

    public void NPCEntryEnd(HorseController horse)
    {
        int num = GetHorseEntryIndex(horse);
        if (num < 0) return; // 경주에 참가한 말이 아님

        Destroy(horse.gameObject);
        entries[num] = null;
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
        /// <summary>
        /// 레이스 끝
        /// </summary>
        [SerializeField] public EndTent end;
        /// <summary>
        /// 장애물들
        /// </summary>
        [SerializeField] public GameObject[] obstacles;
        /// <summary>
        /// 장애물 확률
        /// </summary>
        [SerializeField, Range(0f, 1f)] public float obstacleRate;

        /// <summary>
        /// 상금
        /// </summary>
        [SerializeField] public int[] prize;

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
                SteerStrength = type switch { RaceType.Hard => 50f, RaceType.Normal => 40f, _ => 30f }
            };

            return stats;
        }
    }

    public readonly struct GoalInfo
    {
        public readonly HorseController horse;
        public readonly int index;
        public readonly float time;

        public GoalInfo(HorseController horse, int index, float time)
        {
            this.horse = horse;
            this.index = index;
            this.time = time;
        }
    }
}
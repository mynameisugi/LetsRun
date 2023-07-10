using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 다음 경기를 시행하는 매니저
/// </summary>
public class RaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject racePrefab;

    [SerializeField]
    private Race.RaceInfo[] infos;

    public enum RaceType : int
    {
        Easy = 0, // 500m
        Normal = 1, // 1000m
        Hard = 2 // 1500m
    }

    private void Start()
    {
        foreach (var info in infos)
        {
            foreach (var box in info.trackNodes)
                if (box)
                {
                    var r = box.GetComponent<MeshRenderer>();
                    if (r) r.enabled = false;
                }
            foreach (var obst in info.obstacles)
                if (obst) obst.SetActive(false);
        }

        GameManager.Instance().Time.RegisterEvent(TimeManager.LOOP - 60, () =>
        {
            PrepareRace(nextObstacle); // 1분 전에 다음 경기 준비
        });

        if (GameManager.Instance().Time.Now > TimeManager.LOOP - 60 &&
            GameManager.Instance().Time.Now < TimeManager.LOOP - 10)
            PrepareRace(nextObstacle); // 이미 1분 전 이내면 바로 준비
    }

    private void PrepareRace(bool obstacle)
    {
        var raceObj = Instantiate(racePrefab, transform);
        CurrentRace = raceObj.GetComponent<Race>();
        CurrentRace.info = infos[(int)NextRace];
        if (!obstacle) CurrentRace.info.obstacleRate = 0f;
        if (playerNum > 0)
        {
            CurrentRace.AddPlayer(playerNum); playerNum = -1;
            CurrentRace.JoinPlayer();
        }

        NextRace = (RaceType)Random.Range(0, 3); // 그 다음 경기 랜덤 선택
        nextObstacle = Random.value > 0.3f;
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("DEBUG Race Ready");
            var raceObj = Instantiate(racePrefab, transform);
            CurrentRace = raceObj.GetComponent<Race>();
            CurrentRace.info = infos[(int)NextRace];
        }
    }

    /// <summary>
    /// 현재 경기
    /// </summary>
    public Race CurrentRace { get; private set; } = null;

    /// <summary>
    /// 다음 경기 종류
    /// </summary>
    public RaceType NextRace { get; private set; } = RaceType.Easy;

    private bool nextObstacle = true;

    public void RegisterPlayer(RaceType type, bool obstacle)
    {
        if (CurrentRace != null && CurrentRace.Status == Race.RaceStage.Prepare
            && GameManager.Instance().Time.Now < TimeManager.LOOP - 10f)
        { // 현재 경기가 준비중
            // 현재 경기에 참가
            if (CurrentRace.info.type != type)
            {
                Destroy(CurrentRace.gameObject);
                CurrentRace = null;
                NextRace = type;
                PrepareRace(obstacle);
            }

            CurrentRace.AddPlayer(Random.Range(0, 8));
            CurrentRace.JoinPlayer();
            return;
        }
        NextRace = type;
        nextObstacle = obstacle;
        playerNum = Random.Range(0, 8);
    }

    private int playerNum = -1;
}
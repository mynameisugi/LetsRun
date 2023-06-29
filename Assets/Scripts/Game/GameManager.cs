using UnityEngine;
using static RaceManager;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public static GameManager Instance() => instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else if (instance != this)
        { Destroy(gameObject); return; }

        Initiate();
    }

    [SerializeField]
    private GameObject raceManagerPrefab;

    /// <summary>
    /// 세이브를 관리
    /// </summary>
    public SaveManager Save { get; private set; }

    /// <summary>
    /// 시간과 이벤트 관리
    /// </summary>
    public TimeManager Time { get; private set; }

    private void Initiate()
    {
        Save = new SaveManager();
        Save.LoadFromPrefs(0); // 자동 세이브 불러오기

        // 시간 매니저 추가
        Time = new TimeManager(Save.LoadValue(TimeManager.SAVEKEY, 0));
        for (int i = 180; i < TimeManager.LOOP; i += 180)
            Time.RegisterEvent(i, AutoSave); // 3분마다 자동 저장

        Time.RegisterEvent(TimeManager.LOOP - 60, () => { // 1분 전에 다음 경기 준비
            var raceObj = Instantiate(raceManagerPrefab, transform);
            RaceManager race = raceObj.GetComponent<RaceManager>();
            race.type = NextRace;

            NextRace = (RaceType)Random.Range(0, 3); // 그 다음 경기 랜덤 선택
        });

        void AutoSave() { if (DoAutoSave) Save.SaveToPrefs(0); }
    }

    /// <summary>
    /// 자동저장 여부
    /// </summary>
    public bool DoAutoSave { get; set; } = true;

    private void Update()
    {
        Time.Update();
    }

    /// <summary>
    /// 다음 경기 종류
    /// </summary>
    public RaceType NextRace { get; private set; } = RaceType.Easy;

    /// <summary>
    /// 다음 경기 종류를 수정
    /// </summary>
    public void SetNextRace(RaceType type) => NextRace = type;

}

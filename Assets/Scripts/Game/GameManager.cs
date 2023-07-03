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

    /// <summary>
    /// 세이브를 관리
    /// </summary>
    public SaveManager Save { get; private set; }

    /// <summary>
    /// 시간과 이벤트 관리
    /// </summary>
    public TimeManager Time { get; private set; }

    /// <summary>
    /// 레이스 관리
    /// </summary>
    public RaceManager Race => GetComponent<RaceManager>();

    /// <summary>
    /// 설정 관리
    /// </summary>
    public GameSettings Settings { get; private set; }

    private void Initiate()
    {
        Save = new SaveManager();
        Settings = new GameSettings();
        Save.LoadFromPrefs(0); // 자동 세이브 불러오기

        // 시간 매니저 추가
        Time = new TimeManager(Save.LoadValue(TimeManager.SAVEKEY, 0));
        for (int i = 180; i < TimeManager.LOOP; i += 180)
            Time.RegisterEvent(i, AutoSave); // 3분마다 자동 저장

        void AutoSave() { if (Settings.DoAutoSave) Save.SaveToPrefs(0); }
    }


    private void Update()
    {
        Time.Update();
    }

}

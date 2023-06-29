using Unity.VisualScripting;
using UnityEngine;

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

    private void Initiate()
    {
        Save = new SaveManager();
        Save.LoadFromPrefs(0); // 자동 세이브 불러오기

        // 시간 매니저 추가
        Time = new TimeManager(Save.LoadValue(TimeManager.SAVEKEY, 0));
        Time.RegisterEvent(180, AutoSave); // 3분째 자동 저장
        Time.RegisterEvent(360, AutoSave); // 6분째 자동 저장
        Time.RegisterEvent(540, AutoSave); // 9분째 자동 저장

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

}

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
    /// ���̺긦 ����
    /// </summary>
    public SaveManager Save { get; private set; }

    /// <summary>
    /// �ð��� �̺�Ʈ ����
    /// </summary>
    public TimeManager Time { get; private set; }

    private void Initiate()
    {
        Save = new SaveManager();
        Save.LoadFromPrefs(0); // �ڵ� ���̺� �ҷ�����

        // �ð� �Ŵ��� �߰�
        Time = new TimeManager(Save.LoadValue(TimeManager.SAVEKEY, 0));
        for (int i = 180; i < TimeManager.LOOP; i += 180)
            Time.RegisterEvent(i, AutoSave); // 3�и��� �ڵ� ����

        Time.RegisterEvent(TimeManager.LOOP - 60, () => { // 1�� ���� ���� ��� �غ�
            var raceObj = Instantiate(raceManagerPrefab, transform);
            RaceManager race = raceObj.GetComponent<RaceManager>();
            race.type = NextRace;

            NextRace = (RaceType)Random.Range(0, 3); // �� ���� ��� ���� ����
        });

        void AutoSave() { if (DoAutoSave) Save.SaveToPrefs(0); }
    }

    /// <summary>
    /// �ڵ����� ����
    /// </summary>
    public bool DoAutoSave { get; set; } = true;

    private void Update()
    {
        Time.Update();
    }

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    public RaceType NextRace { get; private set; } = RaceType.Easy;

    /// <summary>
    /// ���� ��� ������ ����
    /// </summary>
    public void SetNextRace(RaceType type) => NextRace = type;

}

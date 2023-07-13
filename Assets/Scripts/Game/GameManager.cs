using UnityEngine;
using UnityEngine.SceneManagement;

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
    /// ���̺긦 ����
    /// </summary>
    public SaveManager Save { get; private set; }

    /// <summary>
    /// �ð��� �̺�Ʈ ����
    /// </summary>
    public TimeManager Time { get; private set; }

    /// <summary>
    /// ���̽� ����
    /// </summary>
    public RaceManager Race => GetComponent<RaceManager>();

    /// <summary>
    /// BGM ����
    /// </summary>
    public BGMManager BGM => GetComponent<BGMManager>();

    /// <summary>
    /// ���� ����
    /// </summary>
    public GameSettings Settings { get; private set; }

    private void Initiate()
    {
        Save = new SaveManager();
        Settings = new GameSettings();
        Save.LoadFromPrefs(0); // �ڵ� ���̺� �ҷ�����

        // �ð� �Ŵ��� �߰�
        Time = new TimeManager(Save.LoadValue(TimeManager.SAVEKEY, 0));
        for (int i = 180; i < TimeManager.LOOP; i += 180)
            Time.RegisterEvent(i, AutoSave); // 3�и��� �ڵ� ����

        // �÷��̾� ��ġ ����
        const string PLAYERPOSX = "PlayerPositionX",
            PLAYERPOSY = "PlayerPositionY", PLAYERPOSZ = "PlayerPositionZ";
        Save.OnSaveToPref += (SaveManager save) =>
        {
            if (PlayerManager.Instance().IsRiding) return; // ���� Ÿ�� �߿��� ��ġ ���� �� ��
            var player = PlayerManager.InstanceOrigin();
            save.SaveValue(PLAYERPOSX, player.localPosition.x);
            save.SaveValue(PLAYERPOSY, player.localPosition.y);
            save.SaveValue(PLAYERPOSZ, player.localPosition.z);
        };
        // �÷��̾� ��ġ �ҷ�����
        PlayerManager.InstanceOrigin().localPosition =
            new Vector3(Save.LoadValue(PLAYERPOSX, 0f),
                Save.LoadValue(PLAYERPOSY, 0f),
                Save.LoadValue(PLAYERPOSZ, 0f));

        void AutoSave() { if (GameSettings.Values.doAutoSave) Save.SaveToPrefs(0); }
    }


    private void Update()
    {
        Time.Update();
    }

    [SerializeField]
    private GameObject introPrefab;

    public void StartIntro()
    {
        SceneManager.LoadScene(0);
        Instantiate(introPrefab);
    }
}

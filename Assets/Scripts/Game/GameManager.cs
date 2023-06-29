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

    private void Initiate()
    {
        Save = new SaveManager();
        Save.LoadFromPrefs(0); // �ڵ� ���̺� �ҷ�����

        // �ð� �Ŵ��� �߰�
        Time = new TimeManager(Save.LoadValue(TimeManager.SAVEKEY, 0));
        for (int i = 180; i < TimeManager.LOOP; i += 180)
            Time.RegisterEvent(i, AutoSave); // 3�и��� �ڵ� ����

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

}

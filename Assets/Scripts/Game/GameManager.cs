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
        Time.RegisterEvent(180, AutoSave); // 3��° �ڵ� ����
        Time.RegisterEvent(360, AutoSave); // 6��° �ڵ� ����
        Time.RegisterEvent(540, AutoSave); // 9��° �ڵ� ����

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

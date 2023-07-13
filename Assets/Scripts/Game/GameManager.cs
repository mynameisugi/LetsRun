using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    #region ScreenEffects
    private LiftGammaGain gamma = null;
    private Vignette vignette = null;

    private void CollectEffects()
    {
        var volume = FindAnyObjectByType<Volume>();
        foreach (var c in volume.profile.components)
            if (c is LiftGammaGain lgg) gamma = lgg;
            else if (c is Vignette vgn) vignette = vgn;
    }

    /// <summary>
    /// ������ ��ο����� ������� �ִϸ��̼� ����
    /// </summary>
    public void PlayFadeInAndOut(Action OnFaded, Action OnFadeEnded)
    {
        if (gamma == null)
        {
            CollectEffects();
            if (gamma == null) return;
        }
        StartCoroutine(FadeCoroutine(OnFaded, OnFadeEnded));
    }

    private IEnumerator FadeCoroutine(Action OnFaded, Action OnFadeEnded)
    {
        float fade = 1f, delta;
        while (fade > 0f)
        {
            delta = UnityEngine.Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(UnityEngine.Time.unscaledDeltaTime);
            fade -= delta; if (fade < 0f) fade = 0f;
            gamma.gamma.value = Vector4.one * fade;
        }
        OnFaded?.Invoke();

        while (fade < 1f)
        {
            delta = UnityEngine.Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(UnityEngine.Time.unscaledDeltaTime);
            fade += delta; if (fade > 1f) fade = 1f;
            gamma.gamma.value = Vector4.one * fade;
        }
        OnFadeEnded?.Invoke();
    }

    public void PlayVignetteEffect(float intensity, Color color)
    {
        vignette.color.value = color;
        StartCoroutine(VignetteCoroutine(intensity));
    }

    private IEnumerator VignetteCoroutine(float intensity)
    {
        while (intensity > 0f)
        {
            vignette.intensity.value = intensity;
            yield return null;
            intensity -= UnityEngine.Time.deltaTime * 0.3f;
        }
        vignette.intensity.value = 0f;
    }


    #endregion ScreenEffects

}

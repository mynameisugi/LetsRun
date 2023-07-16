using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// NPC 컨트롤러
/// </summary>
public class TextReader : MonoBehaviour
{
    [Header("SpeechBubble")]
    [SerializeField]
    private TMP_Text textUI;
    [SerializeField]
    private string file = "TestText";
    [SerializeField]
    private GameObject textCanvas;

    [Header("SoundEffect")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] talkClips;

    private const string PATHFOLDER = "Texts/";

    private void Start()
    {
        textCanvas.SetActive(false);
    }

    private static string LoadLine(string fileName)
    {
        var asset = Resources.Load(PATHFOLDER + fileName) as TextAsset;
        var content = asset.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return content[Random.Range(0, content.Length)];
    }

    private void ShowDialogue(string text)
    {
        textCanvas.SetActive(true);
        textUI.text = text;
        CancelInvoke(nameof(HideDialogue));
        Invoke(nameof(HideDialogue), Mathf.Clamp(text.Length * 0.5f, 4f, 15f));

        audioSource.volume = GameSettings.Values.SE;
        audioSource.clip = talkClips[Random.Range(0, talkClips.Length)];
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }

    private void HideDialogue()
    {
        textCanvas.SetActive(false);
    }

    public void OnPlayerTrigger()
    {
        if (string.IsNullOrEmpty(file)) return;
        ShowDialogue(LoadLine(file));
    }

    /// <summary>
    /// 지정하는 파일에서 랜덤한 대사를 하나 출력
    /// </summary>
    /// <param name="customFile">파일명 (비우면 기본 대사)</param>
    public void PlayConversation(string customFile = "")
    {
        ShowDialogue(LoadLine(string.IsNullOrEmpty(customFile) ? file : customFile));
    }

    /// <summary>
    /// 원하는 대사를 출력
    /// </summary>
    /// <param name="customText">출력하고자하는 대사</param>
    public void PlayText(string customText)
    {
        ShowDialogue(customText);
    }
}

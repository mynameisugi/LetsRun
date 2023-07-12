using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TextReader : MonoBehaviour
{
    [SerializeField]
    private Text textComponent;
    [SerializeField]
    private string file = "TestText";
    [SerializeField]
    private GameObject conversationWindow;

    private const string PATHFOLDER = "Texts/";

    private static string LoadLine(string fileName)
    {
        var asset = Resources.Load(PATHFOLDER + fileName) as TextAsset;
        var content = asset.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return content[Random.Range(0, content.Length)];
    }

    private void ShowDialogue(string text)
    {
        conversationWindow.SetActive(true);
        textComponent.text = text;
        CancelInvoke(nameof(HideDialogue));
        Invoke(nameof(HideDialogue), Mathf.Clamp(text.Length * 0.5f, 4f, 15f));
    }

    private void HideDialogue()
    {
        conversationWindow.SetActive(false);
    }

    public void OnPlayerTrigger()
    {
        ShowDialogue(LoadLine(file));
    }

    public void PlayConversation(string customFile = "")
    {
        ShowDialogue(LoadLine(string.IsNullOrEmpty(customFile) ? file : customFile));
    }
}

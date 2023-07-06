using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class TextReader : MonoBehaviour
{
    public Text textComponent;
    public string filePath = "/Text/TestText.txt";
    public GameObject conversationWindow;
    private bool isPlayerInRange = false;
    private string[] words;

    void Start()
    {
        string fullPath = Application.dataPath + filePath;

        if (File.Exists(fullPath))
        {
            string fileContents = File.ReadAllText(fullPath);
            words = fileContents.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogError("파일을 찾을 수 없습니다: " + fullPath);
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetMouseButtonDown(0))
        {
            conversationWindow.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            conversationWindow.SetActive(true);
            UpdateConversationText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            conversationWindow.SetActive(false);
        }
    }

    private void UpdateConversationText()
    {
        if (words != null && words.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, words.Length);
            string randomWord = words[randomIndex];
            textComponent.text = randomWord;
        }
        else
        {
            Debug.LogError("대화 내용 배열이 비어 있습니다.");
        }
    }
}

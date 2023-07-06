using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TextReader : MonoBehaviour
{
    public Text textComponent;
    public string filePath = "/Text/TestText.txt";

    void Start()
    {
        string fullPath = Application.dataPath + filePath;

        if (File.Exists(fullPath))
        {
            string fileContents = File.ReadAllText(fullPath);
            textComponent.text = fileContents;
        }
        else
        {
            Debug.LogError("File not found: " + fullPath);
        }
    }
}

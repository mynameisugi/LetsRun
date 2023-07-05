using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextReader : MonoBehaviour
{
    public Text uiText;

    void Start()
    {
        string path = Application.dataPath;
        path += "/Text/TestText.txt";

        string[] contents = System.IO.File.ReadAllLines(path);
        if (contents.Length > 0)
        {
            List<string> textLines = new List<string>();
            List<string> numberLines = new List<string>();

            for (int i = 0; i < contents.Length; i++)
            {
                if (i < 1000)
                {
                    textLines.Add(contents[i]);
                }
                else
                {
                    numberLines.Add(contents[i]);
                }
            }

            // 텍스트 또는 숫자 중 랜덤하게 하나를 선택하여 출력
            if (Random.value < 0.5f && textLines.Count > 0)
            {
                int randomIndex = Random.Range(0, textLines.Count);
                string randomText = textLines[randomIndex];
                uiText.text = randomText;
            }
            else if (numberLines.Count > 0)
            {
                int randomIndex = Random.Range(0, numberLines.Count);
                string randomLine = numberLines[randomIndex];

                string[] txtArr = randomLine.Split(',');
                int[] numArr = new int[txtArr.Length];
                int total = 0;

                for (int j = 0; j < txtArr.Length; j++)
                {
                    numArr[j] = int.Parse(txtArr[j]);
                    total += numArr[j];
                }

                uiText.text = total.ToString();
            }
        }

        string content = System.IO.File.ReadAllText(path);
    }
}

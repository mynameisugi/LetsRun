using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoadURLImage : MonoBehaviour
{
    public RawImage rawImage;
    public InputField urlInputField;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        urlInputField.onEndEdit.AddListener(OnURLInputEndEdit);
    }

    void OnURLInputEndEdit(string url)
    {
        StartCoroutine(GetTexture(url));
    }

    IEnumerator GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.texture = myTexture;
        }
    }
}

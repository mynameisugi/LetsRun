using TMPro;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    [SerializeField] private GameObject panelTuto;
    [SerializeField] private TMP_Text textTuto;

    public void SetMessageBoxText(string text)
    {
        if (string.IsNullOrEmpty(text)) { panelTuto.SetActive(false); return; }
        panelTuto.SetActive(true);
        textTuto.text = text;
    }

}

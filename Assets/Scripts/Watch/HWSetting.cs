using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HWSetting : MonoBehaviour
{
    [SerializeField]
    private HWPopup popup;

    private void OnEnable()
    {
        popupEnabled = false;
        popup.gameObject.SetActive(false); // failsafe

        // LoadOptions

    }

    private void OnDisable()
    {
        SaveOptions();
        popupEnabled = false;
    }

    private void SaveOptions()
    {

    }

    #region Buttons
    private bool popupEnabled = false;

    public void OnClickedReset()
    {
        popup.OpenPopup("초기화하시겠습니까?", "모든 진행사항을\r\n잃게 됩니다.",
            () => {
                SaveOptions();
                //GameManager.Instance().Save.Reset();
                popupEnabled = false;
            },
            () => { popupEnabled = false; });
        popupEnabled = true;
    }

    public void OnClickedExit()
    {
        popup.OpenPopup("종료하시겠습니까?", "진행사항은\r\n자동으로 저장됩니다.",
            () => {
                SaveOptions(); GameManager.Instance().Save.SaveToPrefs();
                popupEnabled = false;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
            },
            () => { popupEnabled = false; });
        popupEnabled = true;
    }
#endregion Buttons
}

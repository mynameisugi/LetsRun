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
        popup.OpenPopup("�ʱ�ȭ�Ͻðڽ��ϱ�?", "��� ���������\r\n�Ұ� �˴ϴ�.",
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
        popup.OpenPopup("�����Ͻðڽ��ϱ�?", "���������\r\n�ڵ����� ����˴ϴ�.",
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

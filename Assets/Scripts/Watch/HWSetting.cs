using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HWSetting : MonoBehaviour
{
    [SerializeField]
    private HWPopup popup;

    [Header("SettingsGUI")]
    [SerializeField] private Toggle toggleAutoSave;
    [SerializeField] private Toggle toggleAntiAlias;
    [SerializeField] private Toggle toggleSoftTurn;
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSE;

    private void OnEnable()
    {
        PopupEnabled = false;
        popup.gameObject.SetActive(false); // failsafe

        // LoadOptions
        var S = GameSettings.Values;
        toggleAutoSave.isOn = S.doAutoSave;
        toggleAntiAlias.isOn = S.antiAlias;
        toggleSoftTurn.isOn = S.softTurn;
        sliderBGM.value = Mathf.RoundToInt(S.BGM * 10f);
        sliderSE.value = Mathf.RoundToInt(S.SE * 10f);
    }

    private void OnDisable()
    {
        SaveOptions();
        PopupEnabled = false;
    }

    private void SaveOptions()
    {
        var settings = new GameSettings.Settings(toggleAutoSave.isOn,
             toggleAntiAlias.isOn,
             toggleSoftTurn.isOn,
             true,
             Mathf.RoundToInt(sliderBGM.value),
             Mathf.RoundToInt(sliderSE.value));
        GameManager.Instance().Settings.ChangeSettings(settings);
    }

    #region Buttons
    private bool PopupEnabled
    {
        set
        {
            Selectable[] uis = new Selectable[] { toggleAutoSave, toggleAntiAlias, toggleSoftTurn, sliderBGM, sliderSE };
            foreach (var ui in uis) ui.interactable = !value;
        }
    }

    public void OnClickedReset()
    {
        popup.OpenPopup("�ʱ�ȭ�Ͻðڽ��ϱ�?", "��� ���������\r\n�Ұ� �˴ϴ�.",
            () =>
            {
                PopupEnabled = false;
                GameManager.Instance().Save.Reset();
                SaveOptions();
                SceneManager.LoadScene(0);
            },
            () => { PopupEnabled = false; });
        PopupEnabled = true;
    }

    public void OnClickedExit()
    {
        popup.OpenPopup("�����Ͻðڽ��ϱ�?", "���������\r\n�ڵ����� ����˴ϴ�.",
            () =>
            {
                SaveOptions(); GameManager.Instance().Save.SaveToPrefs();
                PopupEnabled = false;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
            },
            () => { PopupEnabled = false; });
        PopupEnabled = true;
    }
    #endregion Buttons
}

using System;
using TMPro;
using UnityEngine;

public class HWPopup : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textTitle;

    [SerializeField]
    private TMP_Text textDesc;

    /// <summary>
    /// 팝업을 열며 텍스트를 갱신
    /// </summary>
    /// <param name="title">위에 뜰 텍스트</param>
    /// <param name="desc">가운데 뜰 텍스트</param>
    /// <param name="yesAction">네를 눌렀을때 실행할 액션</param>
    /// <param name="noAction">아니오를 눌렀을때 실행할 액션</param>
    public void OpenPopup(string title, string desc, Action yesAction, Action noAction = null)
    {
        gameObject.SetActive(true);
        textTitle.text = title;
        textDesc.text = desc;
        this.yesAction = yesAction;
        this.noAction = noAction;
    }

    private Action yesAction = null;
    private Action noAction = null;

    public void OnClickedYes()
    {
        yesAction?.Invoke();
        gameObject.SetActive(false);
    }
    public void OnClickedNo()
    {
        noAction?.Invoke();
        gameObject.SetActive(false);
    }
}

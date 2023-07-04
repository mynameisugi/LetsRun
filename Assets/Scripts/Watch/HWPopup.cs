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
    /// �˾��� ���� �ؽ�Ʈ�� ����
    /// </summary>
    /// <param name="title">���� �� �ؽ�Ʈ</param>
    /// <param name="desc">��� �� �ؽ�Ʈ</param>
    /// <param name="yesAction">�׸� �������� ������ �׼�</param>
    /// <param name="noAction">�ƴϿ��� �������� ������ �׼�</param>
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

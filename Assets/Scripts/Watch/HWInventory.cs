using TMPro;
using UnityEngine;

public class HWInventory : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textMoney;

    private PlayerInventory inventory;

    private void OnEnable()
    {
        inventory = PlayerManager.Instance().Inventory();
        if (inventory == null) return;
        textMoney.text = inventory.GetMoney().ToString();
        // TODO: �κ��丮�� ������ �߰� �� ���⿡ ǥ��
        // TODO: �������� ��� �ո� ������ �������� �κ��丮�� ����
    }

    public void OnItemClicked(int i)
    {
        // TODO: ������ Ŭ���� �������� ���忡 ����

    }
}

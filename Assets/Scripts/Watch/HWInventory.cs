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
        // TODO: 인벤토리에 아이템 추가 후 여기에 표시
        // TODO: 아이템을 들고 손목에 가까이 가져가면 인벤토리에 보관
    }

    public void OnItemClicked(int i)
    {
        // TODO: 아이템 클릭시 아이템을 월드에 생성

    }
}

public class PlayerInventory
{
    public PlayerInventory(PlayerManager owner)
    {
        this.owner = owner;
        money = GameManager.Instance().Save.LoadValue(KEYMONEY, 0);
    }

    public readonly PlayerManager owner;

    #region Money

    private int money = 0;

    /// <summary>
    /// 현재 자금
    /// </summary>
    /// <returns></returns>
    public int GetMoney() => money;

    public void AddMoney(int add)
    {
        money += add; SaveMoney();
    }

    public bool TryReduceMoney(int reduce)
    {
        if (money < reduce) return false;
        money -= reduce; SaveMoney(); return true;
    }

    private void SaveMoney()
    {
        GameManager.Instance().Save.SaveValue(KEYMONEY, money);
    }

    public const string KEYMONEY = "PlayerMoney";

    #endregion Money

    // TODO: 아이템 추가

}
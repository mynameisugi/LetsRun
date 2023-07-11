using UnityEngine;

public class EndTent : MonoBehaviour
{
    public BoxCollider[] NPCWaypoints;

    public int playerRank = -1;

    public void SetPrize(int[] newPrice)
        => prize = newPrice;

    private int[] prize = new int[] { 0 };

    private void Start()
    {
        foreach (var box in NPCWaypoints)
            if (box)
            {
                var r = box.GetComponent<MeshRenderer>();
                if (r) r.enabled = false;
            }
    }

    public void OnPrizeAccept()
    {
        if (playerRank < 0) return; // ���� �� �߰ų� �̹� ����
        int reward = prize[Mathf.Clamp(playerRank - 1, 0, prize.Length - 1)];
        PlayerManager.Instance().Inventory().AddMoney(reward);
        Debug.Log($"�÷��̾� {playerRank}�� ��� [{reward}] ����");
        playerRank = -1;
    }
}

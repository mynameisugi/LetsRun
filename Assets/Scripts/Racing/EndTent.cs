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
        if (playerRank < 0) return; // 참가 안 했거나 이미 받음
        int reward = prize[Mathf.Clamp(playerRank - 1, 0, prize.Length - 1)];
        PlayerManager.Instance().Inventory().AddMoney(reward);
        Debug.Log($"플레이어 {playerRank}등 상금 [{reward}] 받음");
        playerRank = -1;
    }
}

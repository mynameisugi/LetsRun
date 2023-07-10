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


}

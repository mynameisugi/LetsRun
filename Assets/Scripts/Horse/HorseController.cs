using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class HorseController : MonoBehaviour
{
    [SerializeField]
    private Transform playerSnap = null;

    [SerializeField]
    private Transform[] ropeHinges = new Transform[2];

    /// <summary>
    /// 플레이어가 탈 수 있는지
    /// </summary>
    public bool playerRidable = false;

    internal bool isPlayerRiding = false;

    private Transform playerOrigin = null;

    private PlayerActionHandler playerAction = null;

    private void Start()
    {
        if (!playerRidable)
        {
            // 플레이어 상호작용 제거
            var interactables = GetComponentsInChildren<XRBaseInteractable>();
            foreach (var i in interactables) i.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isPlayerRiding || !playerOrigin || !playerAction) return;

        Transform lHand = playerAction.directInteractors[0].transform;
        Transform rHand = playerAction.directInteractors[1].transform;

        ropeHinges[0].position = lHand.position;
        ropeHinges[1].position = rHand.position;

    }

    #region XREvents
    public void OnPlayerRideRequest()
    {
        if (!playerRidable || isPlayerRiding) return;
        // 플레이어 추적
        playerOrigin = PlayerManager.InstanceOrigin();
        playerAction = PlayerManager.Action();
        // 플레이어 이동 중지
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = false;
        // 플레이어 말 위로 고정
        playerOrigin.SetParent(playerSnap);
        playerOrigin.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        isPlayerRiding = true;
    }

    public void OnPlayerLeaveRequest()
    {
        if (!isPlayerRiding) return;
        // 플레이어 말 오른쪽으로 이동
        playerOrigin.transform.position = playerSnap.position + playerSnap.right * 2f - playerSnap.up;
        // 플레이어 이동 허용
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
        // 플레이어 추적 중단
        playerOrigin.SetParent(PlayerManager.Instance().transform);
        playerOrigin = null;
        playerAction = null;
        isPlayerRiding = false;
        // 로프 리셋
        ropeHinges[0].localPosition = new Vector3(-0.4f, 1.6f, -0.14f);
        ropeHinges[1].localPosition = new Vector3(-0.4f, 1.6f, 0.14f);
    }
    #endregion XREvents
}

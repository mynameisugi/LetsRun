using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class HorseController : MonoBehaviour
{
    [SerializeField]
    private Transform playerSnap = null;

    [SerializeField]
    private Transform modelNormal = null;

    [SerializeField]
    private Transform[] ropeHinges = new Transform[2];

    public HorseStats stats = new HorseStats(2f);

    /// <summary>
    /// 플레이어가 탈 수 있는지
    /// </summary>
    public bool playerRidable = false;

    internal bool isPlayerRiding = false;

    private Transform playerOrigin = null;

    private PlayerActionHandler playerAction = null;

    private Rigidbody sphere;

    private void Awake()
    {
        var sphereObj = new GameObject($"{gameObject.name} Sphere") { layer = 7 };
        sphere = sphereObj.AddComponent<Rigidbody>();
        sphere.useGravity = false;
        var col = sphereObj.AddComponent<SphereCollider>();
        col.radius = 1f;
        sphereObj.transform.position = gameObject.transform.position + gameObject.transform.up;
    }

    private void Start()
    {
        if (!playerRidable)
        {
            // 플레이어 상호작용 제거
            var interactables = GetComponentsInChildren<XRBaseInteractable>();
            foreach (var i in interactables) i.gameObject.SetActive(false);
        }
    }

    private float curSpeed = 0f;
    private int targetMode = 0;
    private float curMode = 0f;
    private float curRotate = 0f;

    private void Update()
    {
        curRotate = 0f;
        curMode = Mathf.SmoothStep(curMode, targetMode, Time.deltaTime);
        curSpeed = stats.GetSpeed(curMode);

        transform.position = sphere.transform.position - transform.up;

        if (!isPlayerRiding || !playerOrigin || !playerAction) return;
        PlayerControlUpdate();

        transform.Rotate(transform.up, curRotate);
    }

    private void PlayerControlUpdate()
    {
        Transform lHand = playerAction.directInteractors[0].transform;
        Transform rHand = playerAction.directInteractors[1].transform;

        ropeHinges[0].position = lHand.position;
        ropeHinges[1].position = rHand.position;

        curRotate = 0f;
        float handOffset = Vector3.Dot(lHand.position - rHand.position, transform.forward);
        if (Mathf.Abs(handOffset) > 0.3f)
        {
            float rotate = Mathf.Abs(handOffset) - 0.2f;
            rotate = Mathf.Clamp01(rotate * 2.5f) * Mathf.Sign(handOffset);
            curRotate = rotate * 30f * Time.deltaTime;
            return; // 회전 조작이라면 다른 조작을 받지 않음
        }

        
    }

    private void FixedUpdate()
    {
        sphere.AddForce(transform.forward * curSpeed, ForceMode.Acceleration);

        sphere.AddForce(modelNormal.up * -10f, ForceMode.Acceleration);

        if (sphere.velocity.magnitude > curSpeed) sphere.angularDrag = Mathf.Infinity;
        else sphere.angularDrag = 0f;

        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out var hitNear, 2.0f, LayerMask.GetMask("Ground"));
        modelNormal.up = Vector3.Lerp(modelNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        modelNormal.Rotate(0, transform.eulerAngles.y, 0);
        //transform.Rotate(0, transform.eulerAngles.y, 0);
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
        // 말 서서히 정지
        targetMode = 0;
    }
    #endregion XREvents
}

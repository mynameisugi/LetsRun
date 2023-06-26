using TMPro;
using UnityEngine;
using UnityEngine.XR;
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

    public HorseStats stats;

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
        sphere.drag = 1f;
        var col = sphereObj.AddComponent<SphereCollider>();
        col.radius = 1f;
        sphereObj.transform.position = gameObject.transform.position + gameObject.transform.up;

        stats = new(2f);
        curStamina = stats.gallopAmount;
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
    private float gallopTimer = 0f;
    private float staminaRecoveryTimer = 0f;
    private float curStamina = 0f;
    private float displayStamina = 0f;

    private void Update()
    {
        curRotate = 0f;
        curMode = Mathf.SmoothStep(curMode, targetMode, Time.deltaTime * 12f);
        curSpeed = stats.GetSpeed(curMode);
        if (gallopTimer > 0f)
        {
            staminaRecoveryTimer = 2f;
            gallopTimer -= Time.deltaTime;
            if (gallopTimer <= 0f)
            {
                gallopTimer = 0f;
                if (targetMode == 4) targetMode = 3; // 습보에서 구보로 전환
            }
        }
        if (staminaRecoveryTimer > 0f) // 스태미너 회복 딜레이
        {
            staminaRecoveryTimer -= Time.deltaTime;
        }
        else
        {
            staminaRecoveryTimer = 0f;
            curStamina = Mathf.Min(curStamina + Time.deltaTime, stats.gallopAmount);
        }
        displayStamina = Mathf.SmoothStep(displayStamina, curStamina / stats.gallopAmount, Time.deltaTime * 12f); // 표시용 스태미너 퍼센트

        transform.position = sphere.transform.position - transform.up;

        if (!isPlayerRiding || !playerOrigin || !playerAction) return;
        PlayerControlUpdate();

        transform.Rotate(transform.up, curRotate);
    }

    private bool pulled = false, braked = false;
    private float pulledOffset = 0.1f, pulledTime = 0f, brakeTime = 0f;

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
            pulled = false;
            return; // 회전 조작이라면 다른 조작을 받지 않음
        }

        var center = Vector3.Lerp(lHand.position, rHand.position, 0.5f);
        handOffset = Vector3.Dot(center - transform.position, transform.forward);
        testText.text = $"Hand: {handOffset:0.0} Stamina: {curStamina:0.0}\nMode: {curMode:0.00} Speed: {curSpeed:0.00}";

        if (handOffset < 0.1f)
        {
            if (!pulled) pulledOffset = 0.1f;
            pulledOffset = Mathf.Min(pulledOffset, handOffset);
            pulled = true;
            pulledTime = Time.timeSinceLevelLoad;

            // 당긴 채로 양손을 쥐면 0.5초마다 브레이크
            playerAction.GetDevice(0).TryGetFeatureValue(CommonUsages.gripButton, out bool gripL);
            playerAction.GetDevice(1).TryGetFeatureValue(CommonUsages.gripButton, out bool gripR);
            if (gripL && gripR)
            {
                brakeTime += Time.deltaTime;
                if (brakeTime > 0.5f) { braked = true; brakeTime = 0.1f; SendHapticFeedback(0.7f, 0.3f); if (targetMode > 0) --targetMode; }
            }
            else brakeTime = 0f;
        }
        else if (handOffset > pulledOffset + 0.2f)
        {
            if (pulled && !braked) // 당겼고 브레이크한 적 없음
            {
                if (Time.timeSinceLevelLoad - pulledTime < 0.5f) // 빠른 채찍질
                {
                    if (targetMode < 3)
                    {
                        ++targetMode; // 가속
                        SendHapticFeedback(0.5f, 0.5f);
                    }
                    else // 습보
                    {
                        if (curStamina >= 1f) // 스태미너 확인
                        {
                            SendHapticFeedback(0.7f, 1f);
                            curStamina -= 1f; // 스태미너 소모
                            targetMode = 4; // 습보로 전환/유지
                            gallopTimer = 4f; // 습보 타이머 리셋
                        }
                        else // 스태미너 부족
                        {
                            SendHapticFeedback(0.7f, 1.5f);
                            targetMode = 1; // 말 저항, 속도 평보로 늦춤
                            // TODO: 말이 거부하는 애니메이션 플레이
                            staminaRecoveryTimer += 1f; // 스태미너 회복 딜레이 추가
                        }
                    }
                }
            }
            pulled = false;
            braked = false;
        }


        //temp
        /*
        bool tempA, tempB;
        if (playerAction.GetDevice(0).TryGetFeatureValue(CommonUsages.primaryButton, out tempA))
        { if (tempA) { if (!tmpBtnDown && targetMode < 4) { ++targetMode; } tmpBtnDown = true; } }
        if (playerAction.GetDevice(0).TryGetFeatureValue(CommonUsages.secondaryButton, out tempB))
        { if (tempB) { if (!tmpBtnDown && targetMode > 0) { --targetMode; } tmpBtnDown = true; } }
        if (!tempA && !tempB) tmpBtnDown = false; */
    }
    //private bool tmpBtnDown = false;

    private void FixedUpdate()
    {
        var vel = transform.forward * curSpeed;
        vel.y = sphere.velocity.y;
        sphere.velocity = vel;

        sphere.AddForce(modelNormal.up * -10f, ForceMode.Acceleration);

        //testText.text = $"Mode: {curMode:0.00} Spd: {curSpeed:0.00} Vel: {sphere.velocity.magnitude:0.00}";

        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out var hitNear, 2.0f, LayerMask.GetMask("Ground"));
        modelNormal.up = Vector3.Lerp(modelNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        modelNormal.Rotate(0, transform.eulerAngles.y, 0);
        //transform.Rotate(0, transform.eulerAngles.y, 0);
    }

    public TMP_Text testText;

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

    private void SendHapticFeedback(float amplitude, float duration = 0.5f)
    {
        playerAction.GetDevice(0).SendHapticImpulse(0, amplitude, duration);
        playerAction.GetDevice(1).SendHapticImpulse(0, amplitude, duration);
    }
}

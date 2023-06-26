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
    /// �÷��̾ Ż �� �ִ���
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
            // �÷��̾� ��ȣ�ۿ� ����
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
                if (targetMode == 4) targetMode = 3; // �������� ������ ��ȯ
            }
        }
        if (staminaRecoveryTimer > 0f) // ���¹̳� ȸ�� ������
        {
            staminaRecoveryTimer -= Time.deltaTime;
        }
        else
        {
            staminaRecoveryTimer = 0f;
            curStamina = Mathf.Min(curStamina + Time.deltaTime, stats.gallopAmount);
        }
        displayStamina = Mathf.SmoothStep(displayStamina, curStamina / stats.gallopAmount, Time.deltaTime * 12f); // ǥ�ÿ� ���¹̳� �ۼ�Ʈ

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
            return; // ȸ�� �����̶�� �ٸ� ������ ���� ����
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

            // ��� ä�� ����� ��� 0.5�ʸ��� �극��ũ
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
            if (pulled && !braked) // ���� �극��ũ�� �� ����
            {
                if (Time.timeSinceLevelLoad - pulledTime < 0.5f) // ���� ä����
                {
                    if (targetMode < 3)
                    {
                        ++targetMode; // ����
                        SendHapticFeedback(0.5f, 0.5f);
                    }
                    else // ����
                    {
                        if (curStamina >= 1f) // ���¹̳� Ȯ��
                        {
                            SendHapticFeedback(0.7f, 1f);
                            curStamina -= 1f; // ���¹̳� �Ҹ�
                            targetMode = 4; // ������ ��ȯ/����
                            gallopTimer = 4f; // ���� Ÿ�̸� ����
                        }
                        else // ���¹̳� ����
                        {
                            SendHapticFeedback(0.7f, 1.5f);
                            targetMode = 1; // �� ����, �ӵ� �򺸷� ����
                            // TODO: ���� �ź��ϴ� �ִϸ��̼� �÷���
                            staminaRecoveryTimer += 1f; // ���¹̳� ȸ�� ������ �߰�
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
        // �÷��̾� ����
        playerOrigin = PlayerManager.InstanceOrigin();
        playerAction = PlayerManager.Action();
        // �÷��̾� �̵� ����
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = false;
        // �÷��̾� �� ���� ����
        playerOrigin.SetParent(playerSnap);
        playerOrigin.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        isPlayerRiding = true;
    }

    public void OnPlayerLeaveRequest()
    {
        if (!isPlayerRiding) return;
        // �÷��̾� �� ���������� �̵�
        playerOrigin.transform.position = playerSnap.position + playerSnap.right * 2f - playerSnap.up;
        // �÷��̾� �̵� ���
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
        // �÷��̾� ���� �ߴ�
        playerOrigin.SetParent(PlayerManager.Instance().transform);
        playerOrigin = null;
        playerAction = null;
        isPlayerRiding = false;
        // ���� ����
        ropeHinges[0].localPosition = new Vector3(-0.4f, 1.6f, -0.14f);
        ropeHinges[1].localPosition = new Vector3(-0.4f, 1.6f, 0.14f);
        // �� ������ ����
        targetMode = 0;
    }
    #endregion XREvents

    private void SendHapticFeedback(float amplitude, float duration = 0.5f)
    {
        playerAction.GetDevice(0).SendHapticImpulse(0, amplitude, duration);
        playerAction.GetDevice(1).SendHapticImpulse(0, amplitude, duration);
    }
}

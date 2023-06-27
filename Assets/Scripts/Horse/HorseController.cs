using TMPro;
using UnityEngine;
using UnityEngine.AI;
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

    public HorseStats stats = new(2.1f);

    /// <summary>
    /// �÷��̾ Ż �� �ִ���
    /// </summary>
    public bool playerRidable = false;

    internal bool isPlayerRiding = false;

    private Transform playerOrigin = null;

    private PlayerActionHandler playerAction = null;

    private Rigidbody sphere;
    private NavMeshAgent agent;

    private HorseAnimator myAnimator;

    private void Awake()
    {
        var sphereObj = new GameObject($"{gameObject.name} Sphere") { layer = 7 };
        sphere = sphereObj.AddComponent<Rigidbody>();
        sphere.useGravity = false;
        sphere.drag = 1f;
        var col = sphereObj.AddComponent<SphereCollider>();
        col.radius = 1f;
        agent = sphereObj.AddComponent<NavMeshAgent>();
        agent.baseOffset = 1f;// agent.radius = 1f;
        sphereObj.transform.position = gameObject.transform.position + gameObject.transform.up;

        stats.skin = Random.Range(0, 10); // test
        myAnimator = GetComponent<HorseAnimator>();

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
        #region GenericHorseUpdate
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
        myAnimator.SetData(new(curMode, curRotate, displayStamina));
        
        #endregion GenericHorseUpdate

        transform.position = sphere.transform.position - transform.up;

        if (!isPlayerRiding || !playerOrigin || !playerAction) NPCControlUpdate();
        else PlayerControlUpdate();

    }

    private bool pulled = false, braked = false;
    private float pulledOffset = 0.1f, pulledTime = 0f, brakeTime = 0f;

    private void NPCControlUpdate()
    {
        agent.speed = curSpeed;

        // test
        if (!agent.hasPath)
        {
            brakeTime -= Time.deltaTime;
            targetMode = 0;
            if (brakeTime < 0f)
            {
                brakeTime = 0f;
                Vector3 offset = new(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
                //Debug.Log($"{gameObject.name} wanders off to {offset}");
                agent.SetDestination(transform.position + offset);
                targetMode = Random.value > 0.2f ? 1 : 2;
            }
        }
        else
        {
            Vector3 dest = agent.destination;
            float rotate = Mathf.Atan2(dest.x - transform.position.x, dest.z - transform.position.z) * Mathf.Rad2Deg;
            agent.isStopped = Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rotate)) > 30f;
            curRotate = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.y, rotate, (agent.isStopped ? 60f : 30f) * Time.deltaTime) - transform.rotation.eulerAngles.y;
            //Debug.Log($"{rotate:0.00} {curRotate:0.00}");
            //transform.rotation = Quaternion.Euler(0f, curRotate, 0f);
            transform.Rotate(transform.up, curRotate);
            brakeTime = 5f;
        }

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
            pulled = false;
            playerAction.GetDevice(handOffset < 0f ? 0 : 1).SendHapticImpulse(0, 0.1f + rotate * 0.1f, 0.1f);
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
                if (brakeTime > 0.5f)
                {
                    braked = true; brakeTime = 0.1f;
                    RequestModeDecrease();
                }
            }
            else brakeTime = 0f;
        }
        else if (handOffset > pulledOffset + 0.2f)
        {
            if (pulled && !braked) // ���� �극��ũ�� �� ����
            {
                if (Time.timeSinceLevelLoad - pulledTime < 0.5f) // ���� ä����
                {
                    RequestModeIncrease();
                }
            }
            pulled = false;
            braked = false;
        }

        transform.Rotate(transform.up, curRotate);

    }

    private void RequestModeIncrease()
    {
        if (targetMode < 3)
        {
            ++targetMode; // ����
            SendHapticFeedback(0.2f, 0.3f);
        }
        else // ����
        {
            if (curStamina >= 1f) // ���¹̳� Ȯ��
            {
                SendHapticFeedback(0.4f, 0.5f);
                curStamina -= 1f; // ���¹̳� �Ҹ�
                targetMode = 4; // ������ ��ȯ/����
                gallopTimer = 4f; // ���� Ÿ�̸� ����
            }
            else // ���¹̳� ����
            {
                SendHapticFeedback(0.7f, 0.8f);
                targetMode = 1; // �� ����, �ӵ� �򺸷� ����
                                // TODO: ���� �ź��ϴ� �ִϸ��̼� �÷���
                staminaRecoveryTimer += 1f; // ���¹̳� ȸ�� ������ �߰�
            }
        }
    }

    private void RequestModeDecrease()
    {
        if (targetMode > 0)
        {
            --targetMode;
            SendHapticFeedback(0.3f, 0.3f);
        }

    }

    private void FixedUpdate()
    {
        if (isPlayerRiding)
        {
            var vel = transform.forward * curSpeed;
            vel.y = sphere.velocity.y;
            sphere.velocity = vel;
        }

        // �߷�
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
        //character.enabled = false;
        agent.ResetPath();
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

        agent.ResetPath();
    }
    #endregion XREvents

    private void SendHapticFeedback(float amplitude, float duration = 0.5f)
    {
        if (!isPlayerRiding) return;
        playerAction.GetDevice(0).SendHapticImpulse(0, amplitude, duration);
        playerAction.GetDevice(1).SendHapticImpulse(0, amplitude, duration);
    }
}

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
    internal bool isRacing = false;

    private Transform playerOrigin = null;

    internal PlayerActionHandler playerAction = null;

    private Rigidbody sphere;
    private NavMeshAgent agent;

    internal HorseAnimator MyAnimator { get; private set; }
    internal HorseSoundMaker MySoundMaker { get; private set; }
    [SerializeField]
    public JockeyController myJockey;

    private void Awake()
    {
        stats.skin = Random.Range(0, 10);
    }

    private const float RAD = 0.5f;

    private void Start()
    {
        var sphereObj = new GameObject($"{gameObject.name} Sphere") { layer = 7 };
        sphereObj.transform.position = gameObject.transform.position + gameObject.transform.up * RAD;
        sphereObj.AddComponent<HorseSphere>().horse = this;
        sphere = sphereObj.AddComponent<Rigidbody>();
        sphere.useGravity = false;
        sphere.drag = 1f;
        var col = sphereObj.AddComponent<SphereCollider>();
        col.radius = RAD;
        agent = sphereObj.AddComponent<NavMeshAgent>();
        agent.baseOffset = RAD - 0.1f; agent.radius = RAD;
        //var obstacle = sphereObj.AddComponent<NavMeshObstacle>();
        //obstacle.shape = NavMeshObstacleShape.Capsule;
        //obstacle.radius = 1f; obstacle.height = 2f;


        MyAnimator = GetComponent<HorseAnimator>();
        MySoundMaker = GetComponentInChildren<HorseSoundMaker>();
        myJockey = GetComponentInChildren<JockeyController>();
        if (playerRidable) myJockey.SetPlayer();
        if (!isRacing) myJockey.gameObject.SetActive(false);

        curStamina = stats.GallopAmount;

        if (!playerRidable)
        {
            // �÷��̾� ��ȣ�ۿ� ����
            var interactables = GetComponentsInChildren<XRBaseInteractable>();
            foreach (var i in interactables) i.gameObject.SetActive(false);
        }
    }

    private float curSpeed = 0f;
    private int targetMode = 0;
    public float CurMode { get; private set; } = 0f;
    private float curRotate = 0f;
    private float gallopTimer = 0f;
    private float staminaRecoveryTimer = 0f;
    private float curStamina = 0f;
    private float displayStamina = 0f;

    [HideInInspector]
    public float wantToJump = 0f;
    [HideInInspector]
    public float Jumping { get; private set; } = 0f;

    private void Update()
    {
        #region GenericHorseUpdate
        curRotate = 0f;
        CurMode = Mathf.SmoothStep(CurMode, targetMode, Time.deltaTime * 12f);
        curSpeed = stats.GetSpeed(CurMode);

        const float JUMPTIME = 1.5f;
        if (wantToJump > 0f)
        {
            wantToJump -= Time.deltaTime;
            if (CurMode > 3.5f && Jumping <= 0f)
            {
                Jumping = JUMPTIME;
                MyAnimator.PlayJump();
            }
        }
        if (Jumping > 0f) Jumping -= Time.deltaTime;

        if (gallopTimer > 0f && Jumping <= 0f)
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
            curStamina = Mathf.Min(curStamina + Time.deltaTime, stats.GallopAmount);
        }

        displayStamina = Mathf.SmoothStep(displayStamina, curStamina / stats.GallopAmount, Time.deltaTime * 6f); // ǥ�ÿ� ���¹̳� �ۼ�Ʈ

        #endregion GenericHorseUpdate
        const float JUMPPI = Mathf.PI / JUMPTIME;
        transform.position = sphere.transform.position + transform.up * (Mathf.Sin(Jumping * JUMPPI) - RAD);

        if (!isPlayerRiding)
        {
            if (!isRacing) NPCWanderUpdate();
            else NPCRaceUpdate();
        }
        else if (playerOrigin && playerAction) PlayerControlUpdate();

        MyAnimator.SetData(new(CurMode, curRotate / Time.deltaTime, displayStamina, curStamina < 1f));

        transform.Rotate(transform.up, curRotate);
    }

    private bool pulled = false, braked = false;
    private float pulledOffset = 0.1f, pushedOffset = 0.3f, pulledTime = 0f, brakeTime = 2f;
    private const float PUSHPULL = 0.2f;

    #region NPCControl

    public void NPCJoinRace(Race race, int number)
    {
        isRacing = true; CurMode = 0f; targetMode = 0;
        this.race = race;
        raceInfo = this.race.info;
        nextNodeIndex = -1;
        pulledTime = 0f; brakeTime = 0f;
        myJockey.gameObject.SetActive(true);
        myJockey.SetNumber(number);
    }

    public void StartRace()
    {
        nextNodeIndex = 0;
        TargetNextNode();
    }

    private Race race = null;
    private Race.RaceInfo raceInfo;
    private int nextNodeIndex = -1;
    private bool RaceEnded => nextNodeIndex > raceInfo.trackNodes.Length;
    internal bool slowDown = false;

    private void TargetNextNode()
    {
        BoxCollider nextNode = nextNodeIndex < raceInfo.trackNodes.Length ? raceInfo.trackNodes[nextNodeIndex] :
            raceInfo.end.NPCWaypoints[^(nextNodeIndex - raceInfo.trackNodes.Length + 1)];
        Vector3 point = new(
            Random.Range(nextNode.bounds.min.x + 1f, nextNode.bounds.max.x - 1f),
            nextNode.bounds.max.y,
            Random.Range(nextNode.bounds.min.z + 1f, nextNode.bounds.max.z - 1f)
        );
        //point = nextNode.transform.TransformPoint(point);
        if (Physics.Raycast(point, Vector3.down, out var info, 10f, LayerMask.GetMask("Ground")))
            point = info.point;
        agent.SetDestination(point);
        //Debug.Log($"{gameObject.name} goes to node {nextNodeIndex} {point} (dist: {Vector3.Distance(point, sphere.position)})");

        ++nextNodeIndex;
        if (nextNodeIndex >= raceInfo.trackNodes.Length + raceInfo.end.NPCWaypoints.Length)
        {
            nextNodeIndex = -1;
            race.NPCEntryEnd(this);
        }
    }

    private void NPCRaceUpdate()
    {
        if (myJockey)
        {
            ropeHinges[0].transform.position = myJockey.hands[0].position;
            ropeHinges[1].transform.position = myJockey.hands[1].position;
        }

        if (nextNodeIndex < 0) return; // �����

        agent.speed = curSpeed;

        Vector3 next = agent.destination; next.y = 0f;
        Vector3 spherePos = sphere.position; spherePos.y = 0f;
        if (Vector3.Distance(next, spherePos) < (RaceEnded ? 0.2f : 3f))
        {
            //Debug.Log($"{gameObject.name}: {next} ~ {sphere.position} ({Vector3.Distance(next, sphere.position)})");
            TargetNextNode();
        }
        float rotate = Mathf.Atan2(next.x - transform.position.x, next.z - transform.position.z) * Mathf.Rad2Deg;
        curRotate = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.y, rotate, stats.SteerStrength * Time.deltaTime) - transform.rotation.eulerAngles.y;

        if (RaceEnded)
        {
            if (targetMode > 1 && Random.value < 0.04f * targetMode * targetMode) RequestModeDecrease();
            else if (targetMode < 1) RequestModeIncrease();
            return;
        }

        if (slowDown)
        {
            pulledTime -= Time.deltaTime; brakeTime = 0f;
            if (pulledTime < 0f)
            {
                if (targetMode > 2) RequestModeDecrease();
                else if (targetMode < 2) RequestModeIncrease();
                pulledTime = raceInfo.type switch
                {
                    RaceManager.RaceType.Easy => Random.Range(1f, 2f),
                    RaceManager.RaceType.Normal => Random.Range(0.5f, 1.5f),
                    _ => Random.Range(0.3f, 0.6f),
                };
            }
        }
        else if (targetMode < 3)
        {
            pulledTime -= Time.deltaTime; brakeTime = 0f;
            if (pulledTime < 0f)
            {
                RequestModeIncrease();
                pulledTime = raceInfo.type switch
                {
                    RaceManager.RaceType.Easy => Random.Range(1f, 2f),
                    RaceManager.RaceType.Normal => Random.Range(0.5f, 1.5f),
                    _ => Random.Range(0.3f, 0.6f),
                };
            }
        }
        else
        {
            brakeTime -= Time.deltaTime; pulledTime = 0f;
            if (brakeTime < 0f)
            {
                if (curStamina < 1f)
                {
                    float cancel = raceInfo.type switch
                    {
                        RaceManager.RaceType.Easy => 0.2f,
                        RaceManager.RaceType.Normal => 0.6f,
                        _ => 0.95f,
                    };
                    if (Random.value < cancel)
                    {
                        brakeTime = Mathf.Max(1f, stats.GallopAmount * Random.Range(0.3f, 1.1f));
                        return;
                    }
                }
                RequestModeIncrease();
                brakeTime = raceInfo.type switch
                {
                    RaceManager.RaceType.Easy => Random.Range(1f, 6f),
                    RaceManager.RaceType.Normal => Random.Range(3f, 5f),
                    _ => Random.Range(4f, 5f),
                };
            }
        }
    }

    private void NPCWanderUpdate()
    {
        agent.speed = curSpeed;

        if (!agent.hasPath)
        {
            brakeTime -= Time.deltaTime;
            targetMode = 0;
            if (brakeTime < 0f)
            {
                pulledTime = 5f; brakeTime = Random.Range(1f, 9f);
                Vector3 offset = new(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
                if (playerRidable) { brakeTime *= 4f; offset *= 0.3f; }
                //Debug.Log($"{gameObject.name} wanders off to {offset}");
                agent.SetDestination(transform.position + offset);
                targetMode = Random.value > 0.2f ? 1 : 2;
                if (targetMode == 1) MySoundMaker.OnHorsePurr();
                else MySoundMaker.OnHorseNeigh();
            }
        }
        else
        {
            Vector3 dest = agent.destination;
            float rotate = Mathf.Atan2(dest.x - transform.position.x, dest.z - transform.position.z) * Mathf.Rad2Deg;
            agent.isStopped = Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rotate)) > stats.SteerStrength;
            if (!agent.isStopped) pulledTime -= Time.deltaTime;
            curRotate = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.y, rotate, (agent.isStopped ? 2f : 1f) * stats.SteerStrength * Time.deltaTime) - transform.rotation.eulerAngles.y;
            //Debug.Log($"{rotate:0.00} {curRotate:0.00}");
            //transform.rotation = Quaternion.Euler(0f, curRotate, 0f);
            if (pulledTime < 0f)
            {
                brakeTime = Random.Range(1f, 9f);
                agent.ResetPath();
            }
        }

    }

    #endregion NPCControl

    #region PlayerControl

    private void PlayerControlUpdate()
    {
        Transform lHand = playerAction.directInteractors[0].transform;
        Transform rHand = playerAction.directInteractors[1].transform;

        ropeHinges[0].position = lHand.position + lHand.up * -0.3f;
        ropeHinges[1].position = rHand.position + lHand.up * -0.3f;

        curRotate = 0f;
        float handOffset = Vector3.Dot(lHand.position - rHand.position, transform.forward);
        // if (testText) testText.text = $"Stamina: {curStamina:0.0}\nMode: {curMode:0.00} Speed: {curSpeed:0.00}";
        if (Mathf.Abs(handOffset) > PUSHPULL * 1.2f)
        {
            float rotate = Mathf.Abs(handOffset) - 0.2f;
            rotate = Mathf.Clamp01(rotate * 2.5f) * Mathf.Sign(handOffset);
            curRotate = rotate * stats.SteerStrength * Time.deltaTime;
            pulled = false;
            playerAction.GetDevice(handOffset < 0f ? 0 : 1).SendHapticImpulse(0, 0.1f + rotate * 0.1f, 0.1f);
            return; // ȸ�� �����̶�� �ٸ� ������ ���� ����
        }

        handOffset = GetHandsOffset();

        if (handOffset < pulledOffset)
        {
            if (brakeTime < 0.1f) pulledOffset = Mathf.Min(pulledOffset, handOffset);
            pushedOffset = pulledOffset + PUSHPULL;
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
        else if (handOffset > pushedOffset)
        {
            pushedOffset = handOffset;
            pulledOffset = pushedOffset - PUSHPULL;
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

    }

    private float GetHandsOffset()
    {
        Transform lHand = playerAction.directInteractors[0].transform;
        Transform rHand = playerAction.directInteractors[1].transform;
        var center = Vector3.Lerp(lHand.position, rHand.position, 0.5f);
        return Vector3.Dot(center - transform.position, transform.forward);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        agent.enabled = false;
        transform.rotation = rotation;
        sphere.transform.SetPositionAndRotation(position, rotation);
        sphere.velocity = Vector3.zero;
        CurMode = 0f; targetMode = 0;
        curSpeed = 0f; curRotate = 0;
        Update();
        agent.enabled = true;
        agent.ResetPath();
    }

    #endregion PlayerControl

    private void RequestModeIncrease()
    {
        if (targetMode < 3)
        {
            ++targetMode; // ����
            SendHapticFeedback(0.2f, 0.3f);
            MySoundMaker.OnHorsePurr();
        }
        else // ����
        {
            if (curStamina >= 1f) // ���¹̳� Ȯ��
            {
                SendHapticFeedback(0.4f, 0.5f);
                MySoundMaker.OnHorsePurr();
                curStamina -= 1f; // ���¹̳� �Ҹ�
                targetMode = 4; // ������ ��ȯ/����
                gallopTimer = 4f; // ���� Ÿ�̸� ����
            }
            else // ���¹̳� ����
            {
                SendHapticFeedback(0.7f, 0.8f);
                targetMode = 1; // �� ����, �ӵ� �򺸷� ����
                MySoundMaker.OnHorseDistress();
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
            if (targetMode > 0) MySoundMaker.OnHorsePurr();
            else MySoundMaker.OnHorseNeigh();
            SendHapticFeedback(0.3f, 0.3f);
        }

    }

    public void Penalty(int type)
    {
        if (isPlayerRiding) GameManager.Instance().PlayVignetteEffect(0.5f, Color.red);
        CurMode = type;
        targetMode = type;
        MySoundMaker.OnHorseDistress();
    }

    private void FixedUpdate()
    {
        if (isPlayerRiding)
        {
            var vel = curSpeed * transform.forward;
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

    // public TMP_Text testText;

    private void OnDestroy()
    {
        if (sphere) Destroy(sphere.gameObject);
    }

    #region XREvents
    public void OnPlayerRideRequest()
    {
        if (!playerRidable || isPlayerRiding) return;
        // ī�޶�� ��¥ �÷��̾� ǥ��
        myJockey.gameObject.SetActive(true);
        // �� NPC AI ����
        pulledTime = 0f; brakeTime = 0f;
        agent.ResetPath();
        agent.enabled = false;
        // �÷��̾� ž�� ���� ����
        PlayerManager.Instance().IsRiding = true;
        // �÷��̾� ����
        playerOrigin = PlayerManager.InstanceOrigin();
        playerAction = PlayerManager.Instance().Action();
        // �÷��̾� �̵� ����
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = false;
        // �÷��̾� �� ���� ����
        playerOrigin.SetParent(playerSnap);
        playerOrigin.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        isPlayerRiding = true;
        // �� ������ ����
        float handOffset = GetHandsOffset();
        pulledOffset = handOffset - PUSHPULL * 0.5f;
        pushedOffset = handOffset + PUSHPULL * 0.5f;
        // �� �ִϸ��̼� ���
        playerAction.RequestHandAnimation(0, HandAnimator.SpecialAnimation.GripHalter);
        playerAction.RequestHandAnimation(1, HandAnimator.SpecialAnimation.GripHalter);
    }

    public void OnPlayerLeaveRequest()
    {
        if (!isPlayerRiding) return;
        // ī�޶�� ��¥ �÷��̾� ����
        myJockey.gameObject.SetActive(false);
        // �÷��̾ �� ���������� �̵�
        playerOrigin.transform.position = playerSnap.position + playerSnap.right * 2f - playerSnap.up;
        // �÷��̾� �̵� ���
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
        // �÷��̾� ž�� ���� ����
        PlayerManager.Instance().IsRiding = false;
        // �� �ִϸ��̼� �ߴ�
        playerAction.RequestHandAnimation(0, HandAnimator.SpecialAnimation.None);
        playerAction.RequestHandAnimation(1, HandAnimator.SpecialAnimation.None);
        // �÷��̾� ���� �ߴ�
        playerOrigin.SetParent(PlayerManager.Instance().transform);
        playerOrigin = null;
        playerAction = null;
        isPlayerRiding = false;
        // ���� ����
        ropeHinges[0].localPosition = new Vector3(-0.4f, 1.6f, -0.2f);
        ropeHinges[1].localPosition = new Vector3(-0.4f, 1.6f, 0.2f);
        // �� ������ ����
        targetMode = 0;
        // NPC AI �ʱ�ȭ
        agent.enabled = true;
        agent.ResetPath();
    }

    private void SendHapticFeedback(float amplitude, float duration = 0.5f)
    {
        if (!isPlayerRiding || !GameSettings.Values.rumble) return;
        playerAction.GetDevice(0).SendHapticImpulse(0, amplitude, duration);
        playerAction.GetDevice(1).SendHapticImpulse(0, amplitude, duration);
    }
    #endregion XREvents
}

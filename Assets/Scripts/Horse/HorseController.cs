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
    /// 플레이어가 탈 수 있는지
    /// </summary>
    public bool playerRidable = false;

    internal bool isPlayerRiding = false;
    internal bool isRacing = false;

    private Transform playerOrigin = null;

    internal PlayerActionHandler playerAction = null;

    private Rigidbody sphere;
    private NavMeshAgent agent;

    private HorseAnimator myAnimator;

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


        myAnimator = GetComponent<HorseAnimator>();

        curStamina = stats.GallopAmount;

        if (!playerRidable)
        {
            // 플레이어 상호작용 제거
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

    public float wantToJump = 0f;
    public float Jumping { get; private set; } = 0f;

    private void Update()
    {
        #region GenericHorseUpdate
        curRotate = 0f;
        CurMode = Mathf.SmoothStep(CurMode, targetMode, Time.deltaTime * 12f);
        curSpeed = stats.GetSpeed(CurMode);

        const float JUMPTIME = 2f;
        if (wantToJump > 0f)
        {
            wantToJump -= Time.deltaTime;
            if (CurMode > 3.5f && Jumping <= 0f)
            {
                Jumping = JUMPTIME;
                myAnimator.PlayJump();
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
            curStamina = Mathf.Min(curStamina + Time.deltaTime, stats.GallopAmount);
        }

        displayStamina = Mathf.SmoothStep(displayStamina, curStamina / stats.GallopAmount, Time.deltaTime * 6f); // 표시용 스태미너 퍼센트

        #endregion GenericHorseUpdate
        const float JUMPPI = Mathf.PI / JUMPTIME;
        transform.position = sphere.transform.position + transform.up * (Mathf.Sin(Jumping * JUMPPI) - RAD);

        if (!isPlayerRiding)
        {
            if (!isRacing) NPCWanderUpdate();
            else NPCRaceUpdate();
        }
        else if (playerOrigin && playerAction) PlayerControlUpdate();

        myAnimator.SetData(new(CurMode, curRotate / Time.deltaTime, displayStamina));

        transform.Rotate(transform.up, curRotate);
    }

    private bool pulled = false, braked = false;
    private float pulledOffset = 0.1f, pushedOffset = 0.3f, pulledTime = 0f, brakeTime = 2f;
    private const float PUSHPULL = 0.2f;

    #region NPCControl

    public void NPCJoinRace(Race race)
    {
        isRacing = true; CurMode = 0f; targetMode = 0;
        this.race = race;
        raceInfo = this.race.info;
        nextNodeIndex = -1;
        pulledTime = 0f; brakeTime = 0f;
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
        if (nextNodeIndex < 0) return; // 대기중

        agent.speed = curSpeed;

        Vector3 next = agent.destination;
        if (Vector3.Distance(next, sphere.position) < 2f)
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

        if (CurMode < 3)
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
                //Debug.Log($"{gameObject.name} wanders off to {offset}");
                agent.SetDestination(transform.position + offset);
                targetMode = Random.value > 0.2f ? 1 : 2;
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

        ropeHinges[0].position = lHand.position;
        ropeHinges[1].position = rHand.position;

        curRotate = 0f;
        float handOffset = Vector3.Dot(lHand.position - rHand.position, transform.forward);
        // if (testText) testText.text = $"Stamina: {curStamina:0.0}\nMode: {curMode:0.00} Speed: {curSpeed:0.00}";
        if (Mathf.Abs(handOffset) > PUSHPULL * 1.5f)
        {
            float rotate = Mathf.Abs(handOffset) - 0.2f;
            rotate = Mathf.Clamp01(rotate * 2.5f) * Mathf.Sign(handOffset);
            curRotate = rotate * stats.SteerStrength * Time.deltaTime;
            pulled = false;
            playerAction.GetDevice(handOffset < 0f ? 0 : 1).SendHapticImpulse(0, 0.1f + rotate * 0.1f, 0.1f);
            return; // 회전 조작이라면 다른 조작을 받지 않음
        }

        handOffset = GetHandsOffset();

        if (handOffset < pulledOffset)
        {
            if (brakeTime < 0.1f) pulledOffset = Mathf.Min(pulledOffset, handOffset);
            pushedOffset = pulledOffset + PUSHPULL;
            pulled = true;
            pulledTime = Time.timeSinceLevelLoad;

            // 당긴 채로 양손을 쥐면 0.5초마다 브레이크
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
            if (pulled && !braked) // 당겼고 브레이크한 적 없음
            {
                if (Time.timeSinceLevelLoad - pulledTime < 0.5f) // 빠른 채찍질
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
        transform.rotation = rotation;
        sphere.transform.position = position;
        sphere.transform.rotation = rotation;
        sphere.velocity = Vector3.zero;
        agent.ResetPath();
        CurMode = 0f; targetMode = 0;
        curSpeed = 0f; curRotate = 0;
        Update();
    }

    #endregion PlayerControl

    private void RequestModeIncrease()
    {
        if (targetMode < 3)
        {
            ++targetMode; // 가속
            SendHapticFeedback(0.2f, 0.3f);
        }
        else // 습보
        {
            if (curStamina >= 1f) // 스태미너 확인
            {
                SendHapticFeedback(0.4f, 0.5f);
                curStamina -= 1f; // 스태미너 소모
                targetMode = 4; // 습보로 전환/유지
                gallopTimer = 4f; // 습보 타이머 리셋
            }
            else // 스태미너 부족
            {
                SendHapticFeedback(0.7f, 0.8f);
                targetMode = 1; // 말 저항, 속도 평보로 늦춤
                                // TODO: 말이 거부하는 애니메이션 플레이
                staminaRecoveryTimer += 1f; // 스태미너 회복 딜레이 추가
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

    public void Penalty(int type)
    {
        CurMode = type;
        targetMode = type;
    }

    private void FixedUpdate()
    {
        if (isPlayerRiding)
        {
            var vel = curSpeed * transform.forward;
            vel.y = sphere.velocity.y;
            sphere.velocity = vel;
        }

        // 중력
        sphere.AddForce(modelNormal.up * -10f, ForceMode.Acceleration);

        //testText.text = $"Mode: {curMode:0.00} Spd: {curSpeed:0.00} Vel: {sphere.velocity.magnitude:0.00}";

        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out var hitNear, 2.0f, LayerMask.GetMask("Ground"));
        modelNormal.up = Vector3.Lerp(modelNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        modelNormal.Rotate(0, transform.eulerAngles.y, 0);
        //transform.Rotate(0, transform.eulerAngles.y, 0);
    }

    // public TMP_Text testText;

    #region XREvents
    public void OnPlayerRideRequest()
    {
        if (!playerRidable || isPlayerRiding) return;
        // 말 NPC AI 제거
        pulledTime = 0f; brakeTime = 0f;
        agent.ResetPath();
        // 플레이어 탑승 상태 변경
        PlayerManager.Instance().IsRiding = true;
        // 플레이어 추적
        playerOrigin = PlayerManager.InstanceOrigin();
        playerAction = PlayerManager.Instance().Action();
        // 플레이어 이동 중지
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = false;
        // 플레이어 말 위로 고정
        playerOrigin.SetParent(playerSnap);
        playerOrigin.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        isPlayerRiding = true;
        // 손 오프셋 보정
        float handOffset = GetHandsOffset();
        pulledOffset = handOffset - PUSHPULL * 0.5f;
        pushedOffset = handOffset + PUSHPULL * 0.5f;
    }

    public void OnPlayerLeaveRequest()
    {
        if (!isPlayerRiding) return;
        // 플레이어를 말 오른쪽으로 이동
        playerOrigin.transform.position = playerSnap.position + playerSnap.right * 2f - playerSnap.up;
        // 플레이어 이동 허용
        playerOrigin.GetComponent<DynamicMoveProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;
        playerOrigin.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
        // 플레이어 탑승 상태 변경
        PlayerManager.Instance().IsRiding = false;
        // 플레이어 추적 중단
        playerOrigin.SetParent(PlayerManager.Instance().transform);
        playerOrigin = null;
        playerAction = null;
        isPlayerRiding = false;
        // 로프 리셋
        ropeHinges[0].localPosition = new Vector3(-0.4f, 1.6f, -0.2f);
        ropeHinges[1].localPosition = new Vector3(-0.4f, 1.6f, 0.2f);
        // 말 서서히 정지
        targetMode = 0;
        // NPC AI 초기화
        agent.ResetPath();
    }

    private void SendHapticFeedback(float amplitude, float duration = 0.5f)
    {
        if (!isPlayerRiding || !GameSettings.Settings.Rumble) return;
        playerAction.GetDevice(0).SendHapticImpulse(0, amplitude, duration);
        playerAction.GetDevice(1).SendHapticImpulse(0, amplitude, duration);
    }
    #endregion XREvents
}

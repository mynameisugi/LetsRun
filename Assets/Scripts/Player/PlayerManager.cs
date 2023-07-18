using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �÷��̾��� ����ý��� �� ��ȣ�ۿ��� ���� Ȧ��
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public Transform xrOrigin = null;

    public bool IsRiding
    {
        get => isRiding;
        set
        {
            if (value) handWatch.RequestModeSwitch(HandWatchController.Mode.Main);
            isRiding = value;
        }
    }
    private bool isRiding = false;

    /// <summary>
    /// ���� ���� ��
    /// </summary>
    public HorseController horse;

    /// <summary>
    /// <see cref="PlayerManager"/> �ν��Ͻ�
    /// </summary>
    public static PlayerManager Instance()
        => GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerManager>();

    private static PlayerManager instance = null;

    private void Awake()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    /// <summary>
    /// �÷��̾� ���� ��ġ
    /// </summary>
    public static Transform InstanceOrigin()
        => Instance().xrOrigin;

    [SerializeField]
    private PlayerActionHandler actionHandler = null;

    internal HandWatchController handWatch = null;

    /// <summary>
    /// �÷��̾� �׼� �ڵ鷯
    /// </summary>
    public PlayerActionHandler Action() => actionHandler;

    /// <summary>
    /// �÷��̾� �κ��丮
    /// </summary>
    public PlayerInventory Inventory() => inventory;

    private PlayerInventory inventory = null;

    private void Start()
    {
        inventory = new PlayerInventory(this);
        
        // Save Horse
        GameManager.Instance().Save.OnSaveToPref += (save) =>
        {
            if (!horse) return;
            var horseData = horse.stats.ToSaveString();
            GameManager.Instance().Save.SaveValue(HorseKey, horseData);
        };

        GUI = GetComponentInChildren<GUIController>();
    }

    private const string HorseKey = "PlayerHorseStats";

    public void LoadHorse()
    {
        var data = GameManager.Instance().Save.LoadValue(HorseKey, "");
        if (!string.IsNullOrEmpty(data))
        {
            GameObject horseObj = Instantiate((GameObject)Resources.Load("Prefabs/Horse"));
            horseObj.name = $"PlayerHorse";
            horseObj.transform.position = transform.position - transform.right * 2f;
            horse = horseObj.GetComponent<HorseController>();
            horse.playerRidable = true;
            horse.stats = HorseStats.FromSaveString(data);
        }
    }


    public GameObject mapCamera;

    public GUIController GUI { get; private set; }
}

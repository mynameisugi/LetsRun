using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(HorseController))]
public class HorseAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animCtrler;

    [SerializeField]
    private ParticleSystem speedEffect = null;

    [Header("Bones")]
    [SerializeField]
    private Transform[] ears;
    [SerializeField]
    private Transform[] necks;

    [Header("Renderers")]
    [SerializeField]
    private SkinnedMeshRenderer skinRenderer;
    [SerializeField]
    private MeshRenderer[] renderers;

    [Header("Rope Physics")]
    [SerializeField]
    private Transform[] hinges = new Transform[4];
    [SerializeField, Range(0.5f, 1.5f)]
    private float loseness = 1f;
    [SerializeField, Range(0f, 1f)]
    private float tension = 1f;
    [SerializeField, Range(0f, 1f)]
    private float gravity = 1f;
    [SerializeField, Range(0f, 10f)]
    private float velClamp = 1f;

    private HorseController horse;

    private RopeMesh rope;

    private void Awake()
    {
        horse = GetComponent<HorseController>();
    }

    private void Start()
    {
        if (skinRenderer) skinRenderer.material = horse.stats.GetSkin();
        foreach (var r in renderers) if (r) r.material = horse.stats.GetSkin();
        rope = new RopeMesh(hinges, loseness, tension, gravity, velClamp, horse.stats.GetSkin());

        neckRotOrigins = new Vector3[necks.Length];
        for (int i = 0; i < necks.Length; ++i) neckRotOrigins[i] = necks[i].localRotation.eulerAngles;

        speedEffect.gameObject.SetActive(false);
    }

    public struct AnimData
    {
        public AnimData(float curMode, float curRotate, float displayStamina)
        {
            this.curMode = curMode;
            this.curRotate = curRotate;
            this.displayStamina = displayStamina;
        }

        public float curMode;
        public float curRotate;
        public float displayStamina;
    }

    private AnimData data = new(0f, 0f, 1f);

    public void SetData(AnimData data)
    {
        this.data = data;
    }

    private float breath = 0f;
    private Vector3[] neckRotOrigins;

    private float displayRot = 0f;

    private void Update()
    {
        if (!active) { animCtrler.SetFloat("mode", 0f); return; }

        breath += Time.deltaTime * (Mathf.Lerp(1f, 3f, data.curMode / 4f));
        float breathSin = Mathf.Sin(breath);
        displayRot = Mathf.MoveTowards(displayRot, Mathf.Clamp(-data.curRotate * 0.2f, -20f, 20f), Time.deltaTime * 9f);

        animCtrler.SetFloat("mode", data.curMode);

        float earRot = (1f - data.displayStamina) * 60f;
        for (int i = 0; i < 2; ++i)
        {
            var rot = ears[i].root.localRotation.eulerAngles;
            ears[i].localRotation = Quaternion.Euler(breathSin * 8f, earRot * (i == 0 ? 1f : -1f) + breathSin * 8f, rot.z);
        }

        for (int i = 0; i < necks.Length; ++i)
        {
            necks[i].localRotation = Quaternion.Euler(neckRotOrigins[i].x,
                neckRotOrigins[i].y + displayRot,
                neckRotOrigins[i].z + breathSin * 3f - data.curMode * 6f);
        }
        rope.Update();

        if (horse.isPlayerRiding && data.curMode > 3f)
        {
            speedEffect.gameObject.SetActive(true);
            var emit = speedEffect.emission;
            emit.rateOverTime = (data.curMode - 3f) * 20f;
            var main = speedEffect.main;
            main.startSpeed = Mathf.Lerp(20f, 80f, (horse.stats.SpeedGallop * 0.5f - HorseStats.MinStats[3]) / (HorseStats.MaxStats[3] - HorseStats.MinStats[3]));
        }
        else
            speedEffect.gameObject.SetActive(false);
    }

    private bool active = true;

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < 200f)
        {
            if (!active) rope.Reset();
            rope.FixedUpdate(); active = true;
        }
        else active = false;
    }

    public void PlayJump()
    {
        // Debug.Log($"{gameObject.name} Jump!");
        animCtrler.SetTrigger("jump");
    }
}
using UnityEngine;

[RequireComponent(typeof(HorseController))]
public class HorseAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animCtrler;

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
        if (!active) return;

        breath += Time.deltaTime * (Mathf.Lerp(1f, 3f, data.curMode / 4f));
        float breathSin = Mathf.Sin(breath);
        displayRot = Mathf.MoveTowards(displayRot, Mathf.Clamp(-data.curRotate * 2f, -20f, 20f), Time.deltaTime * 12f);

        animCtrler.SetFloat("mode", data.curMode);

        float earRot = (1f - data.displayStamina) * 60f;
        for (int i = 0; i < 2; ++i)
        {
            var rot = ears[i].localRotation.eulerAngles;
            ears[i].localRotation = Quaternion.Euler(rot.x + breathSin * 8f, earRot * (i == 0 ? 1f : -1f) + breathSin * 8f, rot.z);
        }

        for (int i = 0; i < necks.Length; ++i)
        {
            necks[i].localRotation = Quaternion.Euler(neckRotOrigins[i].x,
                neckRotOrigins[i].y + displayRot,
                neckRotOrigins[i].z + breathSin * 3f - data.curMode * 6f);
        }

        rope.Update();
    }

    private bool active = true;

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < 100f)
        {
            if (!active) rope.Reset();
            rope.FixedUpdate(); active = true;
        }
        else active = false;
    }
}
﻿using UnityEngine;

[RequireComponent(typeof(HorseController))]
public class HorseAnimator : MonoBehaviour
{
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
        for (int i = 0; i < necks.Length; ++i) neckRotOrigins[i] = necks[i].rotation.eulerAngles;
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

    private void Update()
    {
        breath += Time.deltaTime * (Mathf.Lerp(1f, 3f, data.curMode / 4f));
        float breathSin = Mathf.Sin(breath);

        float earRot = (1f - data.displayStamina) * 60f;
        for (int i = 0; i < 2; ++i)
        {
            var rot = ears[i].localRotation.eulerAngles;
            ears[i].localRotation = Quaternion.Euler(rot.x + breathSin * 10f, earRot * (i == 0 ? 1f : -1f) + breathSin * 10f, rot.z);
        }


        for (int i = 0; i < 4; ++i)
        {
            necks[i].rotation = Quaternion.Euler(neckRotOrigins[i].x, neckRotOrigins[i].y + Mathf.Clamp(data.curRotate / 3f, -10f, 10f), neckRotOrigins[i].z + breath * 10f - data.curMode * 10f);
        }

        rope.Update();
    }

    private void FixedUpdate()
    {
        // TODO: 플레이어와 너무 멀면 물리 시뮬레이션 중단하고, 가까워지면 리셋
        rope.FixedUpdate();
    }
}
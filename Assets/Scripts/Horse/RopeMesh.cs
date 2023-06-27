using System;
using UnityEngine;

public class RopeMesh
{
    public RopeMesh(Transform[] hinges, float loseness, float tension, float gravity, float velClamp,
        Material skin)
    {
        this.hinges = hinges;
        this.loseness = loseness;
        this.tension = tension;
        this.gravity = gravity;
        this.velClamp = velClamp;
        this.skin = skin;

        ropes = new Rope[3];
        Initialize();
    }

    private readonly Transform[] hinges;
    private readonly float loseness;
    private readonly float tension;
    private readonly float gravity;
    private readonly float velClamp;
    private Material skin;

    private readonly Rope[] ropes;

    private void Initialize()
    {
        for (int i = 0; i < 3; ++i)
            ropes[i] = new Rope(hinges[i].position, hinges[i + 1].position, i == 1 ? 5 : 6)
            {
                tension = tension,
                loseness = loseness + (i == 1 ? 0f : 0.2f),
                gravity = gravity / (i == 1 ? 5f : 6f),
                velClamp = velClamp
            };
    }

    public void Update()
    {
        for (int i = 0; i < 3; ++i)
        {
            var segs = ropes[i].GetSegmentsPos();
            var c = i switch
            {
                0 => Color.red,
                1 => Color.green,
                _ => Color.blue,
            };
            for (int j = 0; j < segs.Length; ++j)
                Debug.DrawLine(segs[j], j == segs.Length - 1 ? hinges[i + 1].position : segs[j + 1], c);
        }
    }

    public void FixedUpdate()
    {
        // TODO: �÷��̾�� �ʹ� �ָ� ���� �ùķ��̼� �ߴ��ϰ�, ��������� ����
        for (int i = 0; i < 3; ++i)
            ropes[i].Simulate(hinges[i].position, hinges[i + 1].position);
    }


    /// <summary>
    /// ���� �ùķ��̼�
    /// (����: https://youtu.be/FcnvwtyxLds)
    /// </summary>
    private class Rope
    {
        public Rope(Vector3 posLeft, Vector3 posRight, int segments)
        {
            segLength = Vector3.Distance(posLeft, posRight) / (segments - 1);
            this.segments = new Segment[segments];
            for (int i = 0; i < segments; ++i)
                this.segments[i] = new Segment(Vector3.Lerp(posLeft, posRight, (float)i / (segments - 1)));
        }

        private readonly Segment[] segments;
        private float segLength;
        public float loseness = 1f;
        public float tension = 1f;
        public float gravity = 1f;
        public float velClamp = 1f;

        /// <summary>
        /// ������ ��ġ�� ���� �������� ����
        /// </summary>
        public void Reset(Vector3 posLeft, Vector3 posRight)
        {
            segLength = loseness * Vector3.Distance(posLeft, posRight) / (segments.Length - 1);
            for (int i = 0; i < segments.Length; ++i)
            {
                segments[i].pos = Vector3.Lerp(posLeft, posRight, (float)i / (segments.Length - 1));
                segments[i].lastPos = segments[i].pos;
            }
        }

        /// <summary>
        /// ���� ���� ��� �ùķ��̼�
        /// </summary>
        public void Simulate(Vector3 posLeft, Vector3 posRight)
        {
            //Reset(posLeft, posRight); return;
            segLength = loseness * Vector3.Distance(posLeft, posRight) / (segments.Length - 1);
            var grav = gravity * Time.fixedDeltaTime * Physics.gravity;
            for (int i = 0; i < segments.Length; ++i)
            {
                var vel = segments[i].pos - segments[i].lastPos;
                segments[i].lastPos = segments[i].pos;
                vel = Vector3.ClampMagnitude(vel + grav, velClamp * segLength);
                segments[i].pos += vel;
            }

            // constraints
            segments[0].pos = posLeft;
            segments[^1].pos = posRight;
            for (int i = 0; i < segments.Length - 1; ++i)
            {
                float dist = (segments[i].pos - segments[i + 1].pos).magnitude;
                float error = Mathf.Abs(dist - segLength);

                var tensionDir = Vector3.zero;
                if (dist > segLength)
                    tensionDir = (segments[i].pos - segments[i + 1].pos).normalized;
                else if (dist < segLength)
                    tensionDir = (segments[i + 1].pos - segments[i].pos).normalized;

                var tensionVel = error * tension * tensionDir;
                if (i == 0)
                    segments[i + 1].pos += tensionVel;
                else if (i == segments.Length - 1)
                    segments[i].pos -= tensionVel;
                else
                {
                    tensionVel *= 0.8f;
                    segments[i].pos -= tensionVel;
                    segments[i + 1].pos += tensionVel;
                }
            }
        }

        /// <summary>
        /// ���� ������ �� ��ġ�� �迭�� ��ȯ
        /// </summary>
        public Vector3[] GetSegmentsPos()
            => Array.ConvertAll(segments, x => x.pos);

        private struct Segment
        {
            public Segment(Vector3 pos)
            {
                this.pos = pos;
                lastPos = pos;
            }

            public Vector3 pos, lastPos;
        }
    }
}

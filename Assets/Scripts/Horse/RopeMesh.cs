using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly Material skin;

    private readonly Rope[] ropes;

    private Mesh mesh;
    private GameObject meshObj;

    private void Initialize()
    {
        meshObj = new GameObject("RopeMesh");
        meshObj.transform.SetParent(hinges[0].parent);
        meshObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        for (int i = 0; i < 3; ++i)
            ropes[i] = new Rope(meshObj, hinges[i].position, hinges[i + 1].position, i == 1 ? 5 : 6)
            {
                tension = tension,
                loseness = loseness + (i == 1 ? 0f : 0.2f),
                gravity = gravity / (i == 1 ? 5f : 6f),
                velClamp = velClamp
            };

        mesh = new Mesh() { name = "RopeMesh" };
        Update();

        MeshFilter mf = meshObj.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
        mr.material = skin;
    }

    public void Update()
    {
        List<Vector3> segs = new();
        #region GetSegments
        for (int i = 0; i < 3; ++i)
        {
            var s = ropes[i].GetSegmentsPos();
            for (int j = 0; j < s.Length; ++j) segs.Add(s[j]);
        }
        segs.Add(hinges[3].position - meshObj.transform.position);
        segs = SmoothLine(segs, 0.5f);

        static List<Vector3> SmoothLine(List<Vector3> linePoints, float smoothingFactor)
        {
            if (linePoints.Count < 3)
                return linePoints;

            List<Vector3> smoothedLine = new(linePoints);

            for (int i = 1; i < linePoints.Count - 1; i++)
            {
                smoothedLine[i] = (1f - smoothingFactor) * linePoints[i] +
                                  0.5f * smoothingFactor * (linePoints[i - 1] + linePoints[i + 1]);
            }

            return smoothedLine;
        }
        #endregion GetSegments

        for (int j = 0; j < segs.Count - 1; ++j)
            Debug.DrawLine(segs[j], segs[j + 1]);

        #region UpdateMesh
        int segCount = segs.Count - 1;
        Vector3[] verts = new Vector3[segCount * 16];
        int[] tris = new int[segCount * 24];
        Vector3[] normals = new Vector3[verts.Length];
        Vector2[] uvs = Enumerable.Repeat(new Vector2(0.03f, 0.17f), verts.Length).ToArray();

        const float halfWidth = 0.01f;
        const float halfHeight = 0.03f;
        for (int i = 0; i < segCount; ++i)
        {
            Vector3 segStart = segs[i];
            Vector3 segEnd = segs[i + 1];
            Vector3 segDir = (segEnd - segStart).normalized;
            Vector3 segNormal = Vector3.Cross(segDir, Vector3.up).normalized;
            Vector3 segUp = Vector3.Cross(segDir, segNormal).normalized;

            int vertIdx = i * 16;
            #region Vertices
            if (i < 1)
            {
                verts[vertIdx] = segStart + segNormal * halfWidth + segUp * halfHeight;
                verts[vertIdx + 1] = segStart - segNormal * halfWidth + segUp * halfHeight;
                verts[vertIdx + 7] = segStart - segNormal * halfWidth - segUp * halfHeight;
                verts[vertIdx + 4] = segStart + segNormal * halfWidth - segUp * halfHeight;
            }
            else
            {
                verts[vertIdx] = verts[vertIdx - 16 + 3];
                verts[vertIdx + 1] = verts[vertIdx - 16 + 2];
                verts[vertIdx + 4] = verts[vertIdx - 16 + 5];
                verts[vertIdx + 7] = verts[vertIdx - 16 + 6];
            }

            verts[vertIdx + 2] = segEnd - segNormal * halfWidth + segUp * halfHeight;
            verts[vertIdx + 3] = segEnd + segNormal * halfWidth + segUp * halfHeight;
            verts[vertIdx + 5] = segEnd + segNormal * halfWidth - segUp * halfHeight;
            verts[vertIdx + 6] = segEnd - segNormal * halfWidth - segUp * halfHeight;

            verts[vertIdx + 8] = verts[vertIdx];
            verts[vertIdx + 9] = verts[vertIdx + 3];
            verts[vertIdx + 10] = verts[vertIdx + 5];
            verts[vertIdx + 11] = verts[vertIdx + 4];

            verts[vertIdx + 12] = verts[vertIdx + 2];
            verts[vertIdx + 13] = verts[vertIdx + 1];
            verts[vertIdx + 14] = verts[vertIdx + 7];
            verts[vertIdx + 15] = verts[vertIdx + 6];
            #endregion Vertices

            // 노말 계산
            for (int j = 0; j < 4; ++j)
            {
                Vector3 normal = j switch
                {
                    0 => segUp,
                    1 => -segUp,
                    2 => segNormal,
                    _ => -segNormal
                };
                for (int k = 0; k < 4; ++k)
                    normals[vertIdx + j * 4 + k] = normal;
            }

            for (int j = 0; j < 4; ++j)
            {
                tris[i * 24 + j * 6] = vertIdx + j * 4;
                tris[i * 24 + j * 6 + 1] = vertIdx + j * 4 + 1;
                tris[i * 24 + j * 6 + 2] = vertIdx + j * 4 + 2;
                tris[i * 24 + j * 6 + 3] = vertIdx + j * 4;
                tris[i * 24 + j * 6 + 4] = vertIdx + j * 4 + 2;
                tris[i * 24 + j * 6 + 5] = vertIdx + j * 4 + 3;
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.RecalculateBounds();

        #endregion UpdateMesh

        meshObj.transform.rotation = Quaternion.identity;
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < 3; ++i)
            ropes[i].Simulate(hinges[i].position, hinges[i + 1].position);
    }

    public void Reset()
    {
        for (int i = 0; i < 3; ++i)
            ropes[i].Reset(hinges[i].position, hinges[i + 1].position);
    }

    /// <summary>
    /// 로프 시뮬레이션
    /// (참조: https://youtu.be/FcnvwtyxLds)
    /// </summary>
    private class Rope
    {
        public Rope(GameObject parent, Vector3 posLeft, Vector3 posRight, int segments)
        {
            this.parent = parent.transform;
            posLeft -= this.parent.position;
            posRight -= this.parent.position;
            segLength = Vector3.Distance(posLeft, posRight) / (segments - 1);
            this.segments = new Segment[segments];
            for (int i = 0; i < segments; ++i)
                this.segments[i] = new Segment(Vector3.Lerp(posLeft, posRight, (float)i / (segments - 1)));
        }

        private readonly Transform parent;
        private readonly Segment[] segments;
        private float segLength;
        public float loseness = 1f;
        public float tension = 1f;
        public float gravity = 1f;
        public float velClamp = 1f;

        /// <summary>
        /// 로프의 위치를 선형 보간으로 리셋
        /// </summary>
        public void Reset(Vector3 posLeft, Vector3 posRight)
        {
            posLeft -= parent.position;
            posRight -= parent.position;
            segLength = loseness * Vector3.Distance(posLeft, posRight) / (segments.Length - 1);
            for (int i = 0; i < segments.Length; ++i)
            {
                segments[i].pos = Vector3.Lerp(posLeft, posRight, (float)i / (segments.Length - 1));
                segments[i].lastPos = segments[i].pos;
            }
        }

        /// <summary>
        /// 로프 물리 기반 시뮬레이션
        /// </summary>
        public void Simulate(Vector3 posLeft, Vector3 posRight)
        {
            posLeft -= parent.position;
            posRight -= parent.position;
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
        /// 로프 구간의 각 위치를 배열로 반환
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

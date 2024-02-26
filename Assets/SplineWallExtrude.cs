using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineWallExtrude : MonoBehaviour
{
    [SerializeField] private MeshFilter _filter;
    [SerializeField] public int resolution;
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private int splineIndex;

    [SerializeField] float _width, _heigh;

    float3 position;
    float3 forward;
    float3 upVector;

    Vector3 p1_low, p2_heigh;

    List<Vector3> _vertsP1_l, _vertsP2_l;

    public bool build;

    private void OnEnable()
    {
        Spline.Changed += UpdateMesh;
    }

    private void OnDisable()
    {
        Spline.Changed -= UpdateMesh;
    }

    private void SampleSpliteWidth(float t, out Vector3 p1_l, out Vector3 p2_l)
    {
        _splineContainer.Evaluate(splineIndex, t, out position, out forward, out upVector);
        float3 right = Vector3.Cross(forward, upVector).normalized;

        p1_low = position + (right * _width);
        p2_heigh = new Vector3(position.x, position.y + _heigh, position.z);

        // Convert positions to local coordinates
        p1_l = transform.InverseTransformPoint(p1_low);
        p2_l = transform.InverseTransformPoint(p2_heigh);
    }

    private void UpdateMesh(Spline arg1, int arg2, SplineModification arg3)
    {
        GetVerts();
        BuildMesh();
    }

    private void Update()
    {
        if (build)
        {
            GetVerts();
            BuildMesh();
            build = false;
        }
    }

    private void GetVerts()
    {
        _vertsP1_l = new List<Vector3>();
        _vertsP2_l = new List<Vector3>();

        float step = 1f / (float)resolution;
        for (int i = 0; i < resolution; i++)
        {
            float t = step * i;
            SampleSpliteWidth(t, out Vector3 p1_l, out Vector3 p2_l);

            // Do not transform to world space, use local coordinates
            _vertsP1_l.Add(p1_l);
            _vertsP2_l.Add(p2_l);
        }
    }

    private void BuildMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int length = _vertsP1_l.Count;

        // First loop for the front face
        for (int i = 0; i < length - 1; i++)
        {
            Vector3 p1 = _vertsP1_l[i];
            Vector3 p2 = _vertsP2_l[i];
            Vector3 p3 = _vertsP1_l[i + 1];
            Vector3 p4 = _vertsP2_l[i + 1];

            int startIndex = i * 4;

            vertices.AddRange(new List<Vector3> { p1, p2, p3, p4 });

            // Triangle 1
            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);

            // Triangle 2
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
            triangles.Add(startIndex + 1);
        }

        // Second loop for the back face
        for (int i = 0; i < length - 1; i++)
        {
            Vector3 p1 = _vertsP1_l[i];
            Vector3 p2 = _vertsP2_l[i];
            Vector3 p3 = _vertsP1_l[i + 1];
            Vector3 p4 = _vertsP2_l[i + 1];

            int startIndex = i * 4;

            // Add vertices in reverse order
            vertices.AddRange(new List<Vector3> { p4, p3, p2, p1 });

            // Triangle 1 (reversed order)
            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);

            // Triangle 2 (reversed order)
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 3);
        }

        Debug.Log($"Vertex Count: {vertices.Count}, Triangle Count: {triangles.Count / 3}");

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        _filter.mesh = mesh;
    }
}
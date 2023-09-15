using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math;
using UnityEngine;

public class ConvexHull : MonoBehaviour
{
    [SerializeField, Tooltip("凸包を作りたいModelのRenderer")] private SkinnedMeshRenderer _modelRenderer = default;
    
    /// <summary>このObjectのMeshFilter</summary>
    private MeshFilter _meshFilter = default;

    private static Mesh _modelMesh = default;
    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _modelMesh = new Mesh();
    }

    void Start()
    {
        _modelRenderer.BakeMesh(_modelMesh);

        _meshFilter.mesh = GetConvexHull(_modelMesh);
    }

    private Mesh GetConvexHull(Mesh mesh)
    {
        List<Vector3> vertices = new List<Vector3>(mesh.vertices);

        Debug.Log(vertices.Count);

        Mesh retMesh = new Mesh();
        
        retMesh.SetVertices(GetConvexHull(vertices));
        
        retMesh.RecalculateBounds();
        retMesh.RecalculateNormals();
        
        return retMesh;
    }

    private List<Vector3> GetConvexHull(List<Vector3> vertices)
    {
        List<Vector3> convexHullvertices = new List<Vector3>();

        IEnumerable<Vector3> vertEnum = vertices.OrderBy(x => x.x);

        // X座標が最大の点と最小の点をつなぐ線分を出す
        LineSegment xMin2XMax = new LineSegment(vertEnum.First(), vertEnum.Last());

        // 上の線分から一番遠い点を出す
        Vector3 farPoint = default;
        float farDistance = float.MinValue;
        
        foreach (var n in vertEnum)
        {
            var temp = xMin2XMax.SprtDistance(n);

            if (temp > farDistance)
            {
                farDistance = temp;
                farPoint = n;
            }
        }

        convexHullvertices.Add(xMin2XMax.Point1);
        convexHullvertices.Add(xMin2XMax.Point2);
        convexHullvertices.Add(farPoint);
        
        return convexHullvertices;
    }
}

public struct LineSegment
{
    private Vector3 point1;
    private Vector3 point2;
    private Vector3 normalizedP1ToP2Vec;
        
    /// <summary>コンストラクタ</summary>
    /// <param name="point1">point1の値</param>
    /// <param name="point2">point2の値</param>
    public LineSegment(Vector3 point1, Vector3 point2)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.normalizedP1ToP2Vec = Vector3.zero;

        UpdateNormalVec();
    }
        
    /// <summary>線分の端</summary>
    public Vector3 Point1
    {
        get => point1;
        set => point1 = value;
    }
    /// <summary>線分の端</summary>
    public Vector3 Point2
    {
        get => point2;
        set => point2 = value;
    }

    /// <summary>normalizedP1ToP2Vecを更新します</summary>
    public void UpdateNormalVec()
    {
        normalizedP1ToP2Vec = (this.point1 - this.point2).normalized;
    }

    public float SprtDistance(Vector3 point)
    {
        Vector3 tempPoint = point1 + Vector3.Dot(point - point1, normalizedP1ToP2Vec) * normalizedP1ToP2Vec;
        
        return (tempPoint - point).sqrMagnitude;
    }
}

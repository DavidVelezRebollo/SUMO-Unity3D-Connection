using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Foundation
{
    public List<Vector2> Positions;
    public string Type;

    public Foundation(List<Vector2> pos, string type)
    {
        Positions = pos;
        Type = type;
    }
}

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private Transform Parent;
    [SerializeField] private Material BuildingMaterial;
    [SerializeField] private Material ParkMaterial;
    [SerializeField] private Material AmenityMaterial;
    [SerializeField] private Material RoofMaterial;
    [SerializeField] private Material FloorMaterial;
    [SerializeField] private float BuildingHeight;
    
    private Queue<Foundation> _foundationsToPlace = new();
    private bool _showCeilings = true;

    private void Update()
    {
        if (_foundationsToPlace.Count <= 0) return;
        
        PlaceFoundation();
    }

    public void ChangeCeilingState(bool state) => _showCeilings = state;

    public void AddBuildings(List<Foundation> foundations)
    {
        if (foundations.Count <= 0) return;

        _foundationsToPlace = new Queue<Foundation>(foundations);
    }

    private void PlaceFoundation()
    {
        while (_foundationsToPlace.Count > 0)
        {
            Foundation foundation = _foundationsToPlace.Dequeue();
            List<Vector3> points = new();
            foreach(Vector2 v in foundation.Positions) points.Add(new Vector3(v.x, 0, v.y));
            if (foundation.Type.Contains("building"))
            {
                GenerateBuilding(points);
                if (_showCeilings) GeneratePlane(points, false);
                GeneratePlane(points, true);
            }
            else if (foundation.Type.Contains("park")) GeneratePark(points);
        }
    }

    private void GenerateBuilding(List<Vector3> points)
    {
        GameObject building = new("Building");
        building.transform.position = new Vector3(0, -1, 0);
        building.transform.SetParent(Parent);
        
        MeshFilter meshFilter = building.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = building.AddComponent<MeshRenderer>();
        meshRenderer.material = BuildingMaterial;
        
        Mesh mesh = new ();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[points.Count * 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i];
            vertices[i + points.Count] = points[i] + BuildingHeight * Vector3.up;

            uvs[i] = new Vector2(i / (float)points.Count, 0);
            uvs[i + points.Count] = new Vector2(i / (float)points.Count, 1);
        }
        
        List<int> triangles = new();

        for (int i = 0; i < points.Count; i++)
        {
            triangles.Add(i);
            triangles.Add((i + 1) % points.Count);
            triangles.Add(i + points.Count);
            
            triangles.Add((i + 1) % points.Count);
            triangles.Add((i + 1) % points.Count + points.Count);
            triangles.Add(i + points.Count);
        }
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetUVs(0, uvs);
        mesh.Optimize();
        mesh.RecalculateNormals();
        
    }

    private readonly List<List<Vector3>> _generatedRoofs = new();
    private readonly List<List<Vector3>> _generatedFloors = new();

    private void GeneratePlane(List<Vector3> points, bool isFloor)
    {
        float diff = 1;
        
        var collidingRoofs = PolygonIntersecting(points, isFloor ? _generatedFloors : _generatedRoofs);
        if (collidingRoofs.Count > 0) diff = isFloor ? -1 : 2;
        
        GameObject roof = new GameObject("Roof");
        roof.transform.position = new Vector3(0, isFloor ? 0 + diff : BuildingHeight - diff, 0);
        roof.transform.SetParent(Parent);
        
        MeshFilter meshFilter = roof.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = roof.AddComponent<MeshRenderer>();
        meshRenderer.material = isFloor ? FloorMaterial : RoofMaterial;
        
        Mesh mesh = new ();
        meshFilter.mesh = mesh;

        Vector3[] vertices = points.ToArray();

        // Triángulos
        List<int> triangles = new();
        for (int i = 0; i < points.Count; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add((i + 1) % points.Count);
        }

        // UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z); 

        }
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetUVs(0, uvs);
        mesh.Optimize();
        mesh.RecalculateNormals();
        
        if (!isFloor) _generatedRoofs.Add(points);
    }

    private readonly List<List<Vector3>> _generatedParks = new();

    private void GeneratePark(List<Vector3> points)
    {
        float diff = PolygonIntersecting(points, _generatedParks).Count <= 0 ? -0.5f : -0.1f;
        
        GameObject park = new ("Park", typeof(MeshRenderer), typeof(MeshFilter));
        park.transform.position = new Vector3(0, diff, 0);
        park.transform.SetParent(Parent);
        
        MeshFilter meshFilter = park.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = park.GetComponent<MeshRenderer>();
        meshRenderer.material = ParkMaterial;
        
        Mesh mesh = new ();
        meshFilter.mesh = mesh;

        Vector3[] vertices = points.ToArray();

        // Triángulos
        List<int> triangles = new();
        for (int i = 0; i < points.Count; i++)
        {
            triangles.Add(i);
            triangles.Add((i + 1) % points.Count);
            triangles.Add(0);
        }

        // UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z); 

        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetUVs(0, uvs);
        mesh.Optimize();
        mesh.RecalculateNormals();
        
        _generatedParks.Add(points);
    }
    
    // Polygon intersection verification
    private List<List<Vector3>> PolygonIntersecting(List<Vector3> points, List<List<Vector3>> generatedPolygons)
    {
        List<List<Vector3>> enclosedPolygons = new();
        
        foreach (var polygon in generatedPolygons)
        {
            bool enclosed = polygon.Any(v => IsPointInPolygon(v, points));
            
            if (enclosed) enclosedPolygons.Add(polygon);
        }

        return enclosedPolygons;
    }

    private bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
    {
        int intersections = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector3 vx1 = polygon[i];
            Vector3 vx2 = polygon[(i + 1) % polygon.Count];

            if (RayIntersectsSegment(point, vx1, vx2)) intersections++;
        }

        return intersections % 2 != 0;
    }

    private bool RayIntersectsSegment(Vector3 point, Vector3 vertex1, Vector3 vertex2)
    {
        return vertex1.z > point.z != vertex2.z > point.z &&
               point.x < (vertex2.x - vertex1.x) * (point.z - vertex1.z) / (vertex2.z - vertex1.z) + vertex1.z;
    }

    public void ClearBuildings()
    {
        int count = Parent.childCount;
        for (int i = 0; i < count; i++) Destroy(Parent.GetChild(i).gameObject);
        
        _foundationsToPlace.Clear();
    }
}

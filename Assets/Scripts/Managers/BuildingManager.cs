using System;
using System.Collections.Generic;
using System.Globalization;
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
    [SerializeField] private float BuildingHeight;
    
    private readonly Queue<Foundation> _foundationsToPlace = new();

    private void Update()
    {
        if (_foundationsToPlace.Count <= 0) return;
        
        PlaceFoundation();
    }

    public void AddBuilding(string data)
    {
        string[] splitData = data.Split('*');
        List<Vector2> vectors = ParseVector2(splitData[0]);
        string type = splitData[1];
        
        _foundationsToPlace.Enqueue(new Foundation(vectors, type));
    }

    private void PlaceFoundation()
    {
        while (_foundationsToPlace.Count > 0)
        {
            Foundation foundation = _foundationsToPlace.Dequeue();
            List<Vector3> points = new();
            foreach(Vector2 v in foundation.Positions) points.Add(new Vector3(v.x, 0, v.y));
            if (foundation.Type.Contains("building")) GenerateBuilding(points);
            else GeneratePark(points, foundation.Type.Contains("amenity") ? AmenityMaterial : ParkMaterial);
        }
    }

    private List<Vector2> ParseVector2(string data)
    {
        data = data.Trim('(', ')');
        string[] pairs = data.Split(new[] { "), (" }, StringSplitOptions.RemoveEmptyEntries);

        List<Vector2> vectors = new();

        for (int i = 0; i < pairs.Length; i++)
        {
            string cleanedPair = pairs[i].Trim('(', ')');
            string[] coordinates = cleanedPair.Split(',');
            
            if (coordinates.Length < 2) continue;

            float x = ParseFloat(coordinates[0]);
            float y = ParseFloat(coordinates[1]);
            vectors.Add(new Vector2(x, y));
        }

        return vectors;
    }

    private float ParseFloat(string data)
    {
        CultureInfo info = CultureInfo.InvariantCulture;
        return float.Parse(data, info);
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
            vertices[i + points.Count] = points[i] + Vector3.up * BuildingHeight;

            // uvs[i] = new Vector2(points[i].x, points[i].z);
        }
        
        List<int> triangles = new();
        for (int i = 0; i < points.Count; i++)
        {
            int next = (i + 1) % points.Count;
            
            triangles.Add(i);
            triangles.Add(next);
            triangles.Add(i + points.Count);

            triangles.Add(next);
            triangles.Add(next + points.Count);
            triangles.Add(i + points.Count);
        }
        
        AddBase(points, triangles, ref vertices);
        // AddBase(points, triangles, ref vertices, 2);
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        // mesh.SetUVs(0, uvs);
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    private void GeneratePark(List<Vector3> points, Material material)
    {
        GameObject park = new ("Park", typeof(MeshRenderer), typeof(MeshFilter));
        park.transform.position = new Vector3(0, material == ParkMaterial ? -3 : -2, 0);
        park.transform.SetParent(Parent);
        
        MeshFilter meshFilter = park.GetComponent<MeshFilter>();
        MeshRenderer renderer = park.GetComponent<MeshRenderer>();
        renderer.material = material;
        
        Mesh mesh = new();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[points.Count * 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i];
            uvs[i] = new Vector2(points[i].x, points[i].z);
        }
        
        List<int> triangles = new();
        for (int i = 1; i < points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetUVs(0, uvs);
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    private void AddBase(List<Vector3> points, List<int> triangles, ref Vector3[] vertices)
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < points.Count; i++) center += vertices[i];
        center /= points.Count;

        int centerIdx = vertices.Length;
        Array.Resize(ref vertices, vertices.Length + 1);
        vertices[centerIdx] = center;

        for (int i = 0; i < points.Count; i++)
        {
            int next = (i + 1) % points.Count;
            
            triangles.Add(centerIdx);
            triangles.Add(i);
            triangles.Add(next);
            
        }
    }

    public void ClearBuildings()
    {
        int count = Parent.childCount;
        for (int i = 0; i < count; i++) Destroy(Parent.GetChild(i).gameObject);
        
        _foundationsToPlace.Clear();
    }
}

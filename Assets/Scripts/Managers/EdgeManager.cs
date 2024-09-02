using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Edge
{
    public string ID;
    public int NumLanes;

    public Edge(string id, int numLanes)
    {
        ID = id;
        NumLanes = numLanes;
    }

    public override string ToString()
    {
        return $"Edge {ID}: Number of lanes: {NumLanes}";
    }
}

public class EdgeManager : MonoBehaviour
{
    private JunctionManager _junctions;
    private List<Edge> _edges = new();

    private void Awake()
    {
        _junctions = GetComponent<JunctionManager>();
    }

    public void AddEdges(List<Edge> edges) => _edges = edges;
    

    public Edge GetEdge(string id) => _edges.Find(x => x.ID.Equals(id));

    public void ClearEdges() => _edges.Clear();
    
}

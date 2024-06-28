using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Edge
{
    public string ID;
    public Junction From;
    public Junction To;
    public int NumLanes;

    public Edge(string id, Junction from, Junction to, int numLanes)
    {
        ID = id;
        From = from;
        To = to;
        NumLanes = numLanes;
    }

    public override string ToString()
    {
        return $"Edge {ID}: From {From} to {To}. Number of lanes: {NumLanes}";
    }
}

public class EdgeManager : MonoBehaviour
{
    private JunctionManager _junctions;
    private readonly List<Edge> _edges = new();

    private void Awake()
    {
        _junctions = GetComponent<JunctionManager>();
    }

    public void AddEdge(string info)
    {
        if (string.IsNullOrEmpty(info)) return;

        string[] e = info.Split(',');
        string id = e[0];
        int numLanes = int.Parse(e[3]);
        Junction fromJunction = _junctions.GetJunctionByID(e[1]);
        Junction toJunction = _junctions.GetJunctionByID(e[2]);

        Edge edge = new(id, fromJunction, toJunction, numLanes);
        _edges.Add(edge);
    }

    public Edge GetEdge(string id) => _edges.Find(x => x.ID.Equals(id));

    public void ClearEdges() => _edges.Clear();
    
}

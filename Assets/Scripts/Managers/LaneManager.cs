using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Lane
{
    public Edge ParentEdge;
    public List<Vector2> Shape;
    public bool PedestrianRoad;

    public Lane(Edge edge, List<Vector2> shape, bool pedestrian)
    {
        ParentEdge = edge;
        Shape = shape;
        PedestrianRoad = pedestrian;
    }

    public override string ToString()
    {
        string shape = "";
        
        foreach (Vector2 pos in Shape)
        {
            shape += $"({pos.x}, {pos.y}), ";
        }
        
        return $"Edge {ParentEdge.ID}: {shape}";
    }
}

public class LaneManager : MonoBehaviour
{
    [SerializeField] private GameObject TwoWayRoadPrefab;
    [SerializeField] private GameObject OneWayRoadPrefab;
    [SerializeField] private GameObject PedestrianRoadPrefab;
    [SerializeField] private Transform Parent;
    
    private EdgeManager _edgeManager;
    private Queue<Lane> _lanesToPlace = new();

    private void Awake()
    {
        _edgeManager = GetComponent<EdgeManager>();
    }

    private void Update()
    {
        if (_lanesToPlace.Count <= 0) return;
        
        PlaceLane();
    }

    public void AddLanes(List<Lane> lanes)
    {
        if (lanes.Count <= 0) return;

        _lanesToPlace = new Queue<Lane>(lanes);
    }
    
    private List<Vector2> ParseVector2(string data)
    {
        data = data.Trim('(', ')');
        string[] pairs = data.Split(new[] { "), (" }, StringSplitOptions.RemoveEmptyEntries);

        List<Vector2> vectors = new();

        foreach (string p in pairs)
        {
            string cleanedPair = p.Trim('(', ')');
            string[] coordinates = cleanedPair.Split(',');
            
            if (coordinates.Length < 2) continue;

            float x = Utils.ParseFloat(coordinates[0]);
            float y = Utils.ParseFloat(coordinates[1]);
            vectors.Add(new Vector2(x, y));
        }
        
        return vectors;
    }

    private void PlaceLane()
    {
        while (_lanesToPlace.Count > 0)
        {
            Lane lane = _lanesToPlace.Dequeue();
            // if (lane.Shape.Count <= 2)
            // {
            //     InstantiateRoad(lane, lane.FirstJunction.Position, lane.LastJunction.Position);
            //     return;
            // }

            for (int i = 0; i < lane.Shape.Count; i++)
            {
                if (i < lane.Shape.Count - 1)
                {
                    Vector3 from = new(lane.Shape[i].x, 0, lane.Shape[i].y);
                    Vector3 to = new(lane.Shape[i + 1].x, 0, lane.Shape[i + 1].y);
                    InstantiateRoad(lane, from, to);
                }

                /*if(i == 0)
                    InstantiateRoad(lane, lane.FirstJunction.Position, new Vector3(lane.Shape[i].x, 0, lane.Shape[i].y));
                else if (i > 0 && i < lane.Shape.Count - 1)
                {
                    Vector3 from = new(lane.Shape[i].x, 0, lane.Shape[i].y);
                    Vector3 to = new(lane.Shape[i + 1].x, 0, lane.Shape[i + 1].y);
                    InstantiateRoad(lane, from, to);
                }
                else if (i == lane.Shape.Count - 1)
                {
                    InstantiateRoad(lane, new Vector3(lane.Shape[i].x, 0, lane.Shape[i].y), lane.LastJunction.Position);
                }
                */
            }
        }
    }

    private void InstantiateRoad(Lane lane, Vector3 from, Vector3 to)
    {
        GameObject prefab;

        if (lane.PedestrianRoad) prefab = PedestrianRoadPrefab;
        else prefab = lane.ParentEdge.NumLanes == 1 ? OneWayRoadPrefab : TwoWayRoadPrefab;
        
        GameObject go = Instantiate(prefab, Parent);
        go.name = lane.ParentEdge.ID;
        Transform aux = go.transform;
            
        aux.position = (from + to) / 2;
        aux.rotation = Quaternion.LookRotation((to - from).normalized);

        Vector3 scale = aux.localScale;
        scale.z = Vector3.Distance(from, to) / 10;
        aux.localScale = scale;
    }

    public void ClearLanes()
    {
        int count = Parent.childCount;
        for (int i = 0; i < count; i++) Destroy(Parent.GetChild(i).gameObject);
        _lanesToPlace.Clear();
    }
}

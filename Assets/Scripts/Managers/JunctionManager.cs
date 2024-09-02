using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class Junction
{
    public string ID;
    public Vector3 Position;

    public Junction(string id, Vector3 pos)
    {
        ID = id;
        Position = pos;
    }

    public override string ToString()
    {
        return $"Junction {ID}: {Position}";
    }
}

public class JunctionManager : MonoBehaviour
{
    [SerializeField] private Transform Parent;
    [SerializeField] private GameObject Junction;
    [SerializeField] private bool ShowJunctions;

    private readonly Queue<Junction> _junctionsToPlace = new();
    private readonly List<Junction> _junctions = new ();
    
    private void Update()
    {
        if (_junctionsToPlace.Count <= 0) return;
        
        PlaceJunctions();
    }

    public void SaveJunction(List<Junction> junctions)
    {
        if (junctions.Count <= 0) return;

        foreach (Junction junction in junctions)
        {
            _junctionsToPlace.Enqueue(junction);
            _junctions.Add(junction);
        }
    }

    private void PlaceJunctions()
    {
        while (_junctionsToPlace.Count > 0)
        {
            Junction j = _junctionsToPlace.Dequeue();
            
            if (!ShowJunctions) continue;
            Instantiate(Junction, j.Position, Quaternion.identity, Parent).name = j.ID;
        }
    }

    public Junction GetJunctionByID(string id)
    {
        Junction j = _junctions.Find(x => x.ID == id);
        if(j == null) Debug.LogError($"No junction found with the ID {id}");

        return j;
    }

    public void ClearJunctions()
    {
        if (ShowJunctions)
        {
            int count = Parent.childCount;
            for (int i = 0; i < count; i++) Destroy(Parent.GetChild(i).gameObject);
        }
        
        _junctions.Clear();
        _junctionsToPlace.Clear();
    }
}

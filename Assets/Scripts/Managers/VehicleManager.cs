using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Vehicles;
    [SerializeField] private Transform VehiclesParent;
    private readonly Dictionary<string, Vehicle> _vehicles = new();

    private readonly Queue<Vehicle> _queue = new();

    private void Update()
    {
        foreach (Vehicle v in _vehicles.Values) v.Update();
        
        if (_queue.Count <= 0) return;
        
        _queue.Dequeue().Instantiate();
    }

    public void AddVehicle(string info)
    {
        string[] data = info.Split(',');
        if (data.Length < 3) return;
        
        string id = data[0];

        float x = Mathf.Ceil(Utils.ParseFloat(data[1]));
        float z = Mathf.Ceil(Utils.ParseFloat(data[2]));
        Vector3 pos = new (x, 0, z);
        
        
        if (_vehicles.TryGetValue(id, out Vehicle vehicle))
        {
            vehicle.UpdatePosition(pos);
            return;
        }

        Vehicle v = new (id, pos, Vehicles, VehiclesParent);
        _vehicles.TryAdd(id, v);
        _queue.Enqueue(v);
    }

    public void CleanVehicles()
    {
        int count = VehiclesParent.childCount;
        for (int i = 0; i < count; i++)
            Destroy(VehiclesParent.GetChild(i).gameObject);
        
        _vehicles.Clear();
        _queue.Clear();
    }
}

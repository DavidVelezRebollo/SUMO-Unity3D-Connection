using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Vehicles;
    [SerializeField] private Transform VehiclesParent;
    private readonly ConcurrentDictionary<string, Vehicle> _vehicles = new();
    
    private bool _addVehicle;
    private Vector3 _pos;
    private string _id;

    public Action<Vehicle> OnVehicleAdded; 

    private void Update()
    {
        if (_addVehicle) InstantiateVehicle();
    }

    public ConcurrentDictionary<string, Vehicle> GetVehicles() => _vehicles;

    private void InstantiateVehicle()
    {
        Vehicle v = Instantiate(Vehicles[Random.Range(0, Vehicles.Length)], _pos, Quaternion.identity, VehiclesParent).GetComponent<Vehicle>();
        _vehicles.TryAdd(_id, v);
        v.Initialize(_id, _pos);

        v.OnVehicleDestroy += RemoveVehicle;
        OnVehicleAdded?.Invoke(v);
        
        _addVehicle = false;
    }

    public void OnVehicleReceive(string id, Vector3 pos)
    {
        if (_vehicles.TryGetValue(id, out Vehicle vehicle))
        {
            vehicle.UpdatePosition(pos);
            return;
        }

        _addVehicle = true;
        _pos = pos;
        _id = id;
    }

    private void RemoveVehicle(Vehicle v)
    {
        v.OnVehicleDestroy -= RemoveVehicle;
        _vehicles.TryRemove(v.ID, out Vehicle a);
    }

    public void CleanVehicles()
    {
        int count = VehiclesParent.childCount;
        for (int i = 0; i < count; i++)
            Destroy(VehiclesParent.GetChild(i).gameObject);
        
        _vehicles.Clear();
    }
}

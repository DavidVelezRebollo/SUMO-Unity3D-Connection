using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Vehicle
{
    private readonly string _id;
    private readonly GameObject[] _prefabs;
    private readonly Transform _parent;

    private GameObject _vehicleObject;
    private Transform _cameraSpot;
    private Vector3 _position;
    private Quaternion _rotation;

    private const float _DESTROY_TIME = 0.5f;
    private float _timer;
    private bool _destroy;

    public Action<Vehicle> OnVehicleDestroy;

    public Vehicle(string id, Vector3 position, GameObject[] prefabs, Transform parent)
    {
        _id = id;
        _position = position;
        _prefabs = prefabs;
        _parent = parent;

        _timer = _DESTROY_TIME;
    }

    public void Update()
    {
        if (!_vehicleObject) return;
        if (_timer <= 0) _destroy = true;
        if (_destroy)
        {
            Object.Destroy(_vehicleObject.gameObject);
            OnVehicleDestroy?.Invoke(this);
            _vehicleObject = null;
            return;
        }
        
        _vehicleObject.transform.position = _position;
        _vehicleObject.transform.rotation = Quaternion.Slerp(_vehicleObject.transform.rotation, _rotation, 1);

        if (TrafficSimulator.Instance.SimulationStopped()) return;
        _timer -= Time.deltaTime;
    }

    public void Instantiate()
    {
        if (_vehicleObject) return;
        
        _vehicleObject = Object.Instantiate(_prefabs[Random.Range(0, _prefabs.Length)], _position, Quaternion.identity, _parent);
        _vehicleObject.name = _id;
        _cameraSpot = _vehicleObject.transform.GetChild(1);
    }

    public void UpdatePosition(Vector3 position)
    {
        if (_vehicleObject == null) return;

        Vector3 direction = position - _position;
        _position = position;
        _rotation = Quaternion.LookRotation(direction);
        _timer = _DESTROY_TIME;
    }
}

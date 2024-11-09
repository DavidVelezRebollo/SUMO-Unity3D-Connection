using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Vehicle : MonoBehaviour
{
    public string ID { get; private set; }
    [SerializeField] private Transform CameraSpot;

    private Transform _transform;
    private Quaternion _rotation;
    private CameraController _camera;

    private const float _DESTROY_TIME = 3f;
    private Vector3 _position;
    private float _timeDelta;
    private bool _destroy;

    public Action<Vehicle> OnVehicleDestroy;

    private void OnEnable()
    {
        _timeDelta = _DESTROY_TIME;
        _camera = CameraController.Instance;
    }

    public void Initialize(string id, Vector3 pos)
    {
        ID = id;
        _position = pos;
        gameObject.name = id;
    }

    public void Update()
    {
        if (_timeDelta <= 0)
        {
            OnVehicleDestroy?.Invoke(this);
            Destroy(gameObject);
            return;
        }
        
        transform.position = _position;
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, 1);

        if (TrafficSimulator.Instance.SimulationStopped()) return;

        _timeDelta -= Time.deltaTime;
    }

    public void UpdatePosition(Vector3 position)
    {
        Vector3 direction = position - _position;
        _position = position;
        if (direction != Vector3.zero) _rotation = Quaternion.LookRotation(direction); 
        _timeDelta = _DESTROY_TIME;
    }

    public Transform GetCameraSpot() => CameraSpot;

    private void OnMouseDown()
    {
        _camera.SetOnVehicle(GetCameraSpot());
    }
}

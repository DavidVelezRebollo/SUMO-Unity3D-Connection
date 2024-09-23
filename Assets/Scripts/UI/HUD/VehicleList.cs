using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class VehicleList : MonoBehaviour
{
    [SerializeField] private GameObject VehicleTextPrefab;
    [SerializeField] private Transform Content;

    private VehicleManager _vehicleManager;
    private CanvasGroup _canvas;
    private StopButton _stopButton;
    private ConcurrentDictionary<string, Vehicle> _vehicles;
    private readonly Dictionary<Vehicle, GameObject> _textGameObjects = new();
    private bool _isOpen;
    
    private bool _instantiate;
    private Vehicle _vehicleToInstantiate;

    private void Awake()
    {
        _vehicleManager = FindObjectOfType<VehicleManager>();
        _canvas = GetComponentInChildren<CanvasGroup>();
        _stopButton = FindObjectOfType<StopButton>();

        _canvas.alpha = 0;
        _canvas.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        _stopButton.OnStop += OnClose;
        _vehicleManager.OnVehicleAdded += OnVehicleAdd;
    }

    private void OnDisable()
    {
        _stopButton.OnStop -= OnClose;
        _vehicleManager.OnVehicleAdded -= OnVehicleAdd;
        TrafficSimulator.Instance.OnSimulationFinish -= Clear;
    }

    private void Start()
    {
        TrafficSimulator.Instance.OnSimulationFinish += Clear;
    }

    private void Update()
    {
        if (!_instantiate) return;
        
        InstantiateText(_vehicleToInstantiate);
        _instantiate = false;
    }

    private void OnVehicleAdd(Vehicle v)
    {
        _instantiate = true;
        _vehicleToInstantiate = v;
    }

    public void OnOpen()
    {
        _canvas.alpha = 1;
        _canvas.blocksRaycasts = true;
        
        _isOpen = true;
    }
    
    public void OnClose()
    {
        _canvas.alpha = 0;
        _canvas.blocksRaycasts = false;
        
        // Clear();
        _isOpen = false;
    }

    private void Clear()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
        
        _vehicles?.Clear();
    }

    private void DeleteText(Vehicle v)
    {
        v.OnVehicleDestroy -= DeleteText;
        Destroy(_textGameObjects[v].gameObject);
        _textGameObjects.Remove(v);
    }

    private void InstantiateText(Vehicle v)
    {
        GameObject go = Instantiate(VehicleTextPrefab, Content);
        go.GetComponent<VehicleText>().SetVehicle(v);
        
        _textGameObjects.Add(v, go);
        v.OnVehicleDestroy += DeleteText;
    }
}

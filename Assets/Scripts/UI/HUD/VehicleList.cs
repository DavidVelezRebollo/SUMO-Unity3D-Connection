using System;
using System.Collections;
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
    private Dictionary<string, Vehicle> _vehicles;
    private Dictionary<Vehicle, GameObject> _textGameObjects = new();
    private bool _isOpen;
    
    private bool _instantiate;
    private string _stringToInstantiate;
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
    }

    private void Update()
    {
        if (!_instantiate) return;
        
        InstantiateText(_stringToInstantiate, _vehicleToInstantiate);
        _instantiate = false;
    }

    private void OnVehicleAdd(string s, Vehicle v)
    {
        if (!_isOpen) return;

        _stringToInstantiate = s;
        _instantiate = true;
        _vehicleToInstantiate = v;
    }

    public void OnOpen()
    {
        _canvas.alpha = 1;
        _canvas.blocksRaycasts = true;
        
        _vehicles = _vehicleManager.GetVehicles();
        foreach (string s in _vehicles.Keys) InstantiateText(s, _vehicles[s]);
        _isOpen = true;
    }
    
    public void OnClose()
    {
        _canvas.alpha = 0;
        _canvas.blocksRaycasts = false;
        
        Clear();
        _isOpen = false;
    }

    private void Clear()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
        
        _vehicles.Clear();
    }

    private void DeleteText(Vehicle v)
    {
        Destroy(_textGameObjects[v].gameObject);
        v.OnVehicleDestroy -= DeleteText;
        _textGameObjects.Remove(v);
    }

    private void InstantiateText(string s, Vehicle v)
    {
        GameObject go = Instantiate(VehicleTextPrefab, Content);
        go.GetComponent<TMP_Text>().SetText(s);
        go.name = s;
        
        _textGameObjects.Add(v, go);
        v.OnVehicleDestroy += DeleteText;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleButton : MonoBehaviour
{
    private VehicleList _vehicleList;
    private bool _isOpen;
    
    private void Awake()
    {
        _vehicleList = FindObjectOfType<VehicleList>();
    }

    public void OnButton()
    {
        _isOpen = !_isOpen;
        if (_isOpen) _vehicleList.OnOpen();
        else _vehicleList.OnClose();
    }
}

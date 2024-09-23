using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class VehicleText : MonoBehaviour, IPointerDownHandler
{
    private TMP_Text _text;
    private Vehicle _vehicle;
    private CameraController _camera;
    private bool _cameraOnVehicle;

    private void OnEnable()
    {
        _text = GetComponent<TMP_Text>();
        _camera = FindObjectOfType<CameraController>();
    }

    public void SetVehicle(Vehicle vehicle)
    {
        _vehicle = vehicle;
        _text.text = _vehicle.ID;
        _vehicle.OnVehicleDestroy += ResetCamera;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _camera.transform.parent = _vehicle.GetCameraSpot();
        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.identity;
        _camera.LockCamera(true);
        _cameraOnVehicle = true;
    }

    private void ResetCamera(Vehicle vehicle)
    {
        if (!_cameraOnVehicle) return;
        _camera.ResetCamera();
    }
}

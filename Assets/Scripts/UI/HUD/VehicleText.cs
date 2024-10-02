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
    private ExitVehicleButton _exitButton;
    private bool _cameraOnVehicle;

    private void OnEnable()
    {
        _text = GetComponent<TMP_Text>();
        _camera = FindObjectOfType<CameraController>();
        _exitButton = FindObjectOfType<ExitVehicleButton>();

        _exitButton.OnButtonPress += ResetCamera;
    }

    private void OnDisable()
    {
        _vehicle.OnVehicleDestroy -= Aux;
        _exitButton.OnButtonPress -= ResetCamera;
    }

    public void SetVehicle(Vehicle vehicle)
    {
        _vehicle = vehicle;
        _text.text = _vehicle.ID;
        _vehicle.OnVehicleDestroy += Aux;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _camera.transform.parent = _vehicle.GetCameraSpot();
        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.identity;
        _camera.LockCamera(true);
        _cameraOnVehicle = true;
        _exitButton.Show();
    }

    private void Aux(Vehicle v) => ResetCamera();

    private void ResetCamera()
    {
        if (!_cameraOnVehicle) return;
        _camera.ResetCamera();
        _exitButton.Hide();
    }
}

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

    private void OnEnable()
    {
        _text = GetComponent<TMP_Text>();
        _exitButton = FindObjectOfType<ExitVehicleButton>();
        _camera = CameraController.Instance;
    }

    private void OnDisable()
    {
        _vehicle.OnVehicleDestroy -= Aux;
    }

    public void SetVehicle(Vehicle vehicle)
    {
        _vehicle = vehicle;
        _text.text = _vehicle.ID;
        _vehicle.OnVehicleDestroy += Aux;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _camera.SetOnVehicle(_vehicle.GetCameraSpot());
    }

    private void Aux(Vehicle v)
    {
        _exitButton.ResetButton();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitVehicleButton : MonoBehaviour
{
    private CanvasGroup _canvas;

    private CameraController _camera;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        Hide();
    }

    private void Start()
    {
        _camera = CameraController.Instance;
    }

    public void Show()
    {
        _canvas.alpha = 1;
        _canvas.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvas.alpha = 0;
        _canvas.blocksRaycasts = false;
    }
    
    public void OnButton()
    {
        ResetButton();
    }

    public void ResetButton()
    {
        _camera.ResetCamera();
    }
}

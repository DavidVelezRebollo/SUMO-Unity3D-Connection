using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitVehicleButton : MonoBehaviour
{
    private CanvasGroup _canvas;
    public Action OnButtonPress;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        Hide();
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
        OnButtonPress?.Invoke();
    }
}

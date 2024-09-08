using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    private CanvasGroup _canvas;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        _canvas.alpha = 0;
        _canvas.blocksRaycasts = false;
        
        FillStats();
    }

    private void FillStats()
    {
        
    }

    public void OnOpen()
    {
        _canvas.alpha = 1;
        _canvas.blocksRaycasts = true;
    }

    public void OnClose()
    {
        _canvas.alpha = 0;
        _canvas.blocksRaycasts = false;
    }
}

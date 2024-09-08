using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }

    private void Start()
    {
        TrafficSimulator.Instance.OnSimulationStart += Hide;
        TrafficSimulator.Instance.OnSimulationFinish += Show;
    }
    
    private void OnDisable()
    {
        TrafficSimulator.Instance.OnSimulationStart -= Hide;
        TrafficSimulator.Instance.OnSimulationFinish -= Show;
    }

    private void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private TrafficSimulator _trafficSimulator;
    private Image _progressBar;

    private float _step;
    private float _maxStep;
    private bool _onStepChange;

    private void Awake()
    {
        _progressBar = GetComponent<Image>();
        _progressBar.fillAmount = 0;
    }

    private void Start()
    {
        _trafficSimulator = TrafficSimulator.Instance;
        _trafficSimulator.OnStepChange += FillBar;
    }

    private void Update()
    {
        if (!_onStepChange) return;

        _progressBar.fillAmount = _step / _maxStep;
        _onStepChange = false;
    }

    private void FillBar(int step, int maxStep)
    {
        _step = step;
        _maxStep = maxStep;
        _onStepChange = true;
    }
}

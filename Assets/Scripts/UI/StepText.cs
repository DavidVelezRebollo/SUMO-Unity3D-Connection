using System;
using TMPro;
using UnityEngine;

public class StepText : MonoBehaviour
{
    private TMP_Text _text;
    private TrafficSimulator _trafficSimulator;

    private int _step;
    private int _maxStep;
    private bool _changeText;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _trafficSimulator = FindObjectOfType<TrafficSimulator>();

        _trafficSimulator.OnStepChange += UpdateText;
    }

    private void Update()
    {
        if (!_changeText) return;

        _text.SetText($"Step: {_step}\nMax Step: {_maxStep}");
        _changeText = false;
    }

    private void UpdateText(int step, int maxStep)
    {
        _step = step;
        _maxStep = maxStep;
        _changeText = true;
    }
}

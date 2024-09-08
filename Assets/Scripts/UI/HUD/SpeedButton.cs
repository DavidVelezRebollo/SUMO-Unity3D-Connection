using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedButton : MonoBehaviour
{
    private static readonly float[] _SPEEDS = { 1, 0.75f, 0.5f };
    private static readonly string[] _TEXTS = { "x1",  "x1.5", "x2"};

    private TMP_Text _text;
    private float _currentSpeed;
    private int _currentIdx;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void OnButton()
    {
        _currentIdx = (_currentIdx + 1) % _SPEEDS.Length;

        _currentSpeed = _SPEEDS[_currentIdx];
        _text.SetText(_TEXTS[_currentIdx]);
        TrafficSimulator.Instance.SetSpeed(_SPEEDS[_currentIdx]);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    private Statistics _statistics;
    private bool _isOpen;

    private void Awake()
    {
        _statistics = FindObjectOfType<Statistics>();
    }

    public void OnButton()
    {
        _isOpen = !_isOpen;
        if (_isOpen) _statistics.OnOpen();
        else _statistics.OnClose();
    }
}

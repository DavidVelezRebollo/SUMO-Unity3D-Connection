using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private bool VisibleAtStart;

    private CanvasGroup _canvas;
    private bool _isVisible;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        _canvas.alpha = VisibleAtStart ? 1 : 0;
        _canvas.blocksRaycasts = VisibleAtStart;
        _isVisible = VisibleAtStart;
    }

    public void Show()
    {
        _canvas.alpha = 1;
        _canvas.blocksRaycasts = true;
        _isVisible = true;
    }

    public void Hide()
    {
        _canvas.alpha = 0;
        _canvas.blocksRaycasts = false;
        _isVisible = false;
    }

    public bool Visible() => _isVisible;
}

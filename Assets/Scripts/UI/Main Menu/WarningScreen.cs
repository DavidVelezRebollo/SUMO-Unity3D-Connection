using System;
using TMPro;
using UnityEngine;

public class WarningScreen : MonoBehaviour
{
    public static WarningScreen Instance;
    
    [SerializeField] private TMP_Text WarningText;
    private CanvasGroup _group;

    private bool _changeText;
    private string _text;

    private void Awake()
    {
        if (!Instance) Instance = this;
        
        _group = GetComponent<CanvasGroup>();
        ToggleVisibility(false);
    }

    private void Update()
    {
        if (!_changeText) return;
        
        WarningText.text = _text;
        ToggleVisibility(true);
        _changeText = false;
    }

    public void OnError(string error)
    {
        _text = error;
        _changeText = true;
    }

    private void ToggleVisibility(bool isVisible)
    {
        _group.alpha = isVisible ? 1 : 0;
        _group.blocksRaycasts = isVisible;
    }
}

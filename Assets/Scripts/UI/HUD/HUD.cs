using System;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        ToggleVisibility(false);
    }

    public void ToggleVisibility(bool isVisible)
    {
        Canvas.ForceUpdateCanvases();
        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
    }
}

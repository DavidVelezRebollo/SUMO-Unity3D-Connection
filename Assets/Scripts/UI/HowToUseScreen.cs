using System;
using UnityEngine;

public class HowToUseScreen : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        ToggleVisibility(false);
    }

    public void ToggleVisibility(bool isVisible)
    {
        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
    }

    public void ToggleRaycasts(bool raycasts) => _canvasGroup.blocksRaycasts = raycasts;
}

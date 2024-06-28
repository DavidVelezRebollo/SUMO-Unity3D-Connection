using System;
using UnityEngine;

public class InitialScreen : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ToggleVisibility(bool visibility)
    {
        _canvasGroup.alpha = visibility ? 1 : 0;
        _canvasGroup.blocksRaycasts = visibility;
    }
}

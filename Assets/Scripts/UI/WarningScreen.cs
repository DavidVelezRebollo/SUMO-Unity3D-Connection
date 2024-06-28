using System;
using UnityEngine;

public class WarningScreen : MonoBehaviour
{
    private CanvasGroup _group;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        ToggleVisibility(false);
    }

    public void ToggleVisibility(bool isVisible)
    {
        _group.alpha = isVisible ? 1 : 0;
        _group.blocksRaycasts = isVisible;
    }
}

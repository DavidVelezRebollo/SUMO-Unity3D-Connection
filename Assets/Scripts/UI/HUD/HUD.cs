using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup ButtonsVerticalLayout;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        ToggleVisibility(false);
    }

    public void ToggleVisibility(bool isVisible)
    {
        if (isVisible) StartCoroutine(UpdateLayout());
        
        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
    }

    private IEnumerator UpdateLayout()
    {
        yield return new WaitForEndOfFrame();

        ButtonsVerticalLayout.enabled = false;
        ButtonsVerticalLayout.CalculateLayoutInputVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) ButtonsVerticalLayout.transform);
        ButtonsVerticalLayout.enabled = true;
    }
}

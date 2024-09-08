using UnityEngine;

public class BackButton : MonoBehaviour
{
    [SerializeField] private CanvasGroup HideGroup;
    [SerializeField] private CanvasGroup ShowGroup;

    public void OnButtonPress()
    {
        HideGroup.alpha = 0;
        HideGroup.blocksRaycasts = false;

        ShowGroup.alpha = 1;
        ShowGroup.blocksRaycasts = true;
        
    }
}

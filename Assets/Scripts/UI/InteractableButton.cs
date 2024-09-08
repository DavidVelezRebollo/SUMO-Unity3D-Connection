using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.Slerp(transform.localScale, new Vector3(1.2f, 1.2f, 1.2f), 2);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one, 2);
    }
}

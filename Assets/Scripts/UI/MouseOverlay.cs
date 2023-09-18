using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MouseOverlayUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected abstract string OverlayText { get; }
    public virtual void OnPointerEnter(PointerEventData data)
    {
        OverlayTextManager.Instance.UpdateMouseOverlay(OverlayText);
    }

    public virtual void OnPointerExit(PointerEventData data)
    {
        OverlayTextManager.Instance.RemoveMouseOverlay(OverlayText);
    }
}
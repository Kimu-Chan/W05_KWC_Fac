using UnityEngine;
public class OverlayTextDestoryCallback : MonoBehaviour
{
    private void OnDestroy()
    {
        if (OverlayTextManager.Instance != null)
        {
            OverlayTextManager.Instance.RemoveOverlay(gameObject);
        }
    }
}
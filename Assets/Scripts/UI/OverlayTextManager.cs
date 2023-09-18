using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;



public class OverlayTextManager : MonoBehaviour
{
    private static OverlayTextManager overlayTextManager;
    public static OverlayTextManager Instance
    {
        get
        {
            return overlayTextManager;
        }
    }

    private void OnDestroy()
    {
        overlayTextManager = null;
    }

    private Dictionary<GameObject, TextMeshProUGUI> overlayMap = new Dictionary<GameObject, TextMeshProUGUI>();

    TextMeshProUGUI mouseOverlay;


    private void Awake()
    {
        overlayTextManager = this;
    }

    private void Start()
    {
        mouseOverlay = UIManager.Instance.InstantiateMouseOverlayText("").GetComponentInChildren<TextMeshProUGUI>();
        Debug.Assert(mouseOverlay != null);
        mouseOverlay.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public bool IsInViewport(GameObject obj)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(obj.transform.position);
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1;
    }

    public void UpdateMouseOverlay(string text)
    {
        Debug.Log("update mouse overlay");
        mouseOverlay.text = text;
        mouseOverlay.gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void RemoveMouseOverlay(string text)
    {
        if (mouseOverlay.text == text)
        {
            mouseOverlay.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    public void AddOrUpdateOverlay(GameObject obj, string text)
    {
        if (overlayMap.ContainsKey(obj))
        {
            overlayMap[obj].text = text;
        }
        else
        {
            GameObject overlay = UIManager.Instance.InstantiateBuildingOverlayText(text);
            overlay.transform.SetParent(transform);
            overlayMap.Add(obj, overlay.GetComponent<TextMeshProUGUI>());
            obj.AddComponent<OverlayTextDestoryCallback>();
        }
    }

    public void RemoveOverlay(GameObject obj)
    {
        if (overlayMap.ContainsKey(obj))
        {
            Destroy(overlayMap[obj].gameObject);
            overlayMap.Remove(obj);
        }
    }

    private void Update()
    {
        foreach (var (obj, overlay) in overlayMap)
        {
            bool isInViewport = IsInViewport(obj);
            if (isInViewport)
            {
                overlay.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
            }
            overlay.enabled = isInViewport;
        }

        if (mouseOverlay.gameObject.transform.parent.gameObject.activeSelf)
        {
            mouseOverlay.gameObject.transform.parent.transform.position = Input.mousePosition + new Vector3(5, 5, 0);
        }
    }
}
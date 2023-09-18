using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    private ResourceType resourceType = ResourceType.Invalid;
    private ResourceManager resourceManager;

    public Image imageComponent;
    public TextMeshProUGUI textComponent;

    int curValue;

    void Awake()
    {
        resourceManager = ResourceManager.Instance();
    }

    // Update is called once per frame
    void Update()
    {
        if (resourceType != ResourceType.Invalid)
        {
            int v = resourceManager.GetResourceValue(resourceType);
            if (v != curValue)
            {
                curValue = v;
                textComponent.text = v.ToString();
            }
        }
    }
}

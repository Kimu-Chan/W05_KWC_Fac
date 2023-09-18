using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConverterItemPanelUI : MouseOverlayUI
{
    private ResourceType resourceType;
    public RawImage itemImage;
    public TextMeshProUGUI itemCount;
    public TextMeshProUGUI itemNeed;

    protected override string OverlayText => resourceType.ToString();

    public void UpdatePanelImage(ResourceType resourceType)
    {
        this.resourceType = resourceType;
        itemImage.texture = UIManager.LoadResourceTexture(resourceType);
    }

    public void UpdateItemValue(ConverterItem converterItem)
    {
        itemCount.text = converterItem.currentValue.ToString();
        itemNeed.text = converterItem.convertValue.ToString();
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BuildingButtonUI : MouseOverlayUI
{
    public TextMeshProUGUI keyBindText;
    public RawImage buildingImage;
    public ResourceType resourceType;
    public int buttonIdx;

    protected override string OverlayText => resourceType.ToString();

    public void UpdateButton(ResourceType resourceType, int buttonIdx)
    {
        this.resourceType = resourceType;
        this.buttonIdx = buttonIdx;

        buildingImage.texture = UIManager.LoadResourceTexture(resourceType);
        keyBindText.text = $"{buttonIdx + 1}F";
        GetComponent<Button>().onClick.AddListener(StartBuild);
    }

    public void StartBuild()
    {
        BuildModeManager.Instance.HandleBuildMode(resourceType);
    }
}
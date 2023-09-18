using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConverterPrototypeWindow : BuildingWindowUIManager
{
    public TextMeshProUGUI buildingNameText;
    public Transform inputPanelTransform;
    public Transform outputPanelTransform;
    public GameObject converterItemPanelPrefab;

    public List<ConverterItemPanelUI> converterItemPanelUIs = new List<ConverterItemPanelUI>();

    private ConverterPrototype prototype = null;

    public Slider ProgressBar;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        if (!gameObject.activeSelf)
        {
            return;
        }

        ProgressBar.value = prototype.Progress;
        int i = 0;

        foreach (var input in prototype.inputs)
        {
            var converterItemPanelUI = converterItemPanelUIs[i++];
            converterItemPanelUI.UpdateItemValue(input.Value);
        }

        foreach (var output in prototype.outputs)
        {
            var converterItemPanelUI = converterItemPanelUIs[i++];
            converterItemPanelUI.UpdateItemValue(output.Value);
        }
    }

    public override bool OpenWindowCallback(FactoryBuilding factoryBuilding)
    {
        prototype = (ConverterPrototype)factoryBuilding.prototype;
        buildingNameText.text = factoryBuilding.buildingClass.buildingName;

        foreach (var input in prototype.inputs)
        {
            var inputItemPanelGameObject = Instantiate(converterItemPanelPrefab);
            inputItemPanelGameObject.transform.SetParent(inputPanelTransform);
            var converterItemPanelUI = inputItemPanelGameObject.GetComponent<ConverterItemPanelUI>();
            converterItemPanelUI.UpdatePanelImage(input.Key);
            converterItemPanelUI.UpdateItemValue(input.Value);
            converterItemPanelUIs.Add(converterItemPanelUI);
        }

        foreach (var output in prototype.outputs)
        {
            var outputItemPanelGameObject = Instantiate(converterItemPanelPrefab);
            outputItemPanelGameObject.transform.SetParent(outputPanelTransform);
            var converterItemPanelUI = outputItemPanelGameObject.GetComponent<ConverterItemPanelUI>();
            converterItemPanelUI.UpdatePanelImage(output.Key);
            converterItemPanelUI.UpdateItemValue(output.Value);
            converterItemPanelUIs.Add(converterItemPanelUI);
        }

        return true;
    }

    public override void CloseWindowCallback()
    {
        prototype = null;
        converterItemPanelUIs.ForEach((g) => Destroy(g.gameObject));
        converterItemPanelUIs.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    public Dictionary<string, WindowUIManager> windows = new Dictionary<string, WindowUIManager>();
    public Dictionary<BuildingPrototype, BuildingWindowUIManager> buildingWindows = new Dictionary<BuildingPrototype, BuildingWindowUIManager>();
    public GameObject buildingWindowsGameObject;
    public GameObject buildingOverlayGameObject;
    public GameObject buildingOverlayTextPrefab;
    public GameObject mouseOverlayTextPrefab;
    public GameObject canvasGameObject;
    void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }

    public static UIManager Instance
    {
        get
        {
            Debug.Assert(instance != null);
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buildingWindowsGameObject.transform.childCount; ++i)
        {
            Transform child = buildingWindowsGameObject.transform.GetChild(i);
            // if (child.name == buildingWindowsGameObject.name)
            // {
            //     continue;
            // }

            BuildingPrototype prototype = (BuildingPrototype)Enum.Parse(typeof(BuildingPrototype), child.name.Replace("PrototypeWindow", ""));
            buildingWindows.Add(prototype, child.GetComponent<BuildingWindowUIManager>());
        }
    }

    public bool TryOpenWindow(string name)
    {
        Debug.Assert(windows.ContainsKey(name));
        return windows[name].TryOpenWindow();
    }

    public bool TryOpenBuildingWindow(FactoryBuilding building)
    {
        BuildingPrototype prototype = building.buildingClass.prototype;
        Debug.Assert(prototype != BuildingPrototype.Invalid);
        return buildingWindows[prototype].TryOpenBuildingWindow(building);
    }

    public void CloseAllWindow()
    {
        foreach (var v in windows.Values)
        {
            v.CloseWindow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenWindow("IngameInventory");
        }
    }

    public GameObject InstantiateBuildingOverlayText(string text)
    {
        GameObject overlayText = Instantiate(buildingOverlayTextPrefab);
        overlayText.transform.SetParent(buildingOverlayGameObject.transform);
        overlayText.GetComponent<TextMeshProUGUI>().text = text;
        return overlayText;
    }

    public GameObject InstantiateMouseOverlayText(string text)
    {
        GameObject overlayText = Instantiate(mouseOverlayTextPrefab);
        overlayText.transform.SetParent(canvasGameObject.transform);
        overlayText.GetComponentInChildren<TextMeshProUGUI>().text = text;
        return overlayText;
    }

    public static Texture2D LoadResourceTexture(ResourceType resourceType)
    {
        Texture2D result = Resources.Load<Texture2D>($"Icons/{resourceType.ToString()}");
        if (result == null)
        {
            return Resources.Load<Texture2D>("Icons/Missing");
        }
        return result;
    }
}

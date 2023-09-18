using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BuildingButtonPanelUI : MonoBehaviour
{
    private bool Visible;
    public GameObject buildingButtonUIPrefab;
    private List<GameObject> buildingButtonUIs = new List<GameObject>();
    [SerializeField]
    private int currentClicked;

    private void Start()
    {
        ClearBuildingButtonPanelUI();
        currentClicked = -1;
    }

    private void Update()
    {
        int clicked = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            clicked = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            clicked = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            clicked = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            clicked = 4;
        }

        if (currentClicked != -1 && clicked != -1)
        {
            if (currentClicked == clicked)
            {
                ClearBuildingButtonPanelUI();
                return;
            }
            ClearBuildingButtonPanelUI();
        }
        if (clicked != -1)
        {
            switch (clicked)
            {
                case 1:
                    UpdateBuildingButtonPanelUI(BuildingCategory.Food);
                    break;
                case 2:
                    UpdateBuildingButtonPanelUI(BuildingCategory.Metal);
                    break;
                case 3:
                    UpdateBuildingButtonPanelUI(BuildingCategory.Energy);
                    break;
                case 4:
                    UpdateBuildingButtonPanelUI(BuildingCategory.Minion);
                    break;
                default:
                    break;
            }
            currentClicked = clicked;
        }
    }

    public void UpdateBuildingButtonPanelUI(BuildingCategory category)
    {
        GetComponent<Image>().enabled = true;
        var buildings = FactoryBuildingManager.Instance.GetBuildingListByBuildingCategory(category);
        BuildModeManager.currentKeybindBuildings = buildings;
        for (int idx = 0; idx < buildings.Count; ++idx)
        {
            var button = Instantiate(buildingButtonUIPrefab);
            button.transform.SetParent(transform);
            button.transform.localScale = Vector3.one;
            button.GetComponent<BuildingButtonUI>().UpdateButton(buildings[idx], idx);

            buildingButtonUIs.Add(button);
        }
        Visible = true;
    }

    public void ClearBuildingButtonPanelUI()
    {
        Visible = false;
        GetComponent<Image>().enabled = false;
        BuildModeManager.currentKeybindBuildings = null;
        foreach (var button in buildingButtonUIs)
        {
            Destroy(button);
        }
        currentClicked = -1;
    }
}
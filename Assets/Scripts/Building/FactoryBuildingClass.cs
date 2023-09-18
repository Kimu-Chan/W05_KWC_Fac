using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FactoryBuildingClass
{
    public BuildingPrototype prototype;
    public Resource resource;
    public Dictionary<ResourceType, int> costs;
    public HashSet<ResourceType> addons;

    public string buildingName;
    public string prefabName;
    public Action<FactoryBuildingPrototype> initPrototype;
    public BuildingCategory buildingCategory;
    public ResourceType resourceType;

    public int xSize;
    public int ySize;
    public int xCenter;
    public int yCenter;

    public int buildedCount = 0;
    public int canBuildCount = 0;

    public bool isAssemblyRequired = false;
    public bool isPipeSender = false;
    public bool isPipeReciver = false;
    public bool isLogisticsAvailable = false;
    public bool canHaveKeyBindingAction = false;
    public bool isBuildDragAble = false;
    public bool hasOverlay = true;

    public FactoryBuildingClass()
    {
        costs = new Dictionary<ResourceType, int>();
        addons = new HashSet<ResourceType>();
    }

    public FactoryBuilding Instantiate(MapTile mapTile)
    {
        var factoryBuilding = new FactoryBuilding(this, mapTile);
        return factoryBuilding;
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FactoryBlueprintMaterial
{
    public int currentValue;
    public int costValue;
    public FactoryBlueprintMaterial(int neededValue)
    {
        this.costValue = neededValue;
        this.currentValue = 0;
    }
    public bool IsSatisfied
    {
        get
        {
            return costValue <= currentValue;
        }
    }

    public int NeededValue
    {
        get
        {
            return Math.Max(costValue - currentValue, 0);
        }
    }

    public void AddMaterial(int value)
    {
        currentValue += value;
    }
}

public class FactoryBlueprint : ILogisticRequestAvailable
{
    public Dictionary<ResourceType, FactoryBlueprintMaterial> materials = new Dictionary<ResourceType, FactoryBlueprintMaterial>();
    public readonly FactoryBuilding building;
    private LogisticsSystem logisticsSystem;

    public string GetBlueprintOverlayText()
    {
        if (logisticsSystem == null)
        {
            return "\n<#FF0000>연결된 길 없음</color>";
        }

        string text = "";
        foreach (var (resourceType, material) in materials)
        {
            text += $"\n{resourceType.ToString()}: ";
            if (material.IsSatisfied)
            {
                text += "<#00FF00>";
            }
            else
            {
                text += "<#FFF000>";
            }
            text += $"{material.currentValue}/{material.costValue}</color>";
        }
        return text;
    }
    private bool _isFinished = false;
    public bool IsFinished
    {
        get
        {
            return _isFinished;
        }
        set
        {
            _isFinished = value;
        }
    }
    public Vector3 LogisticPortPosition
    {
        get
        {
            Debug.Assert(building != null);
            return building.LogisticPortPosition;
        }
    }

    public FactoryBlueprint(FactoryBuilding building, Dictionary<ResourceType, int> costs) : base()
    {
        Debug.Log($"Factory Blueprint Generated: {building.buildingClass.buildingName}");
        this.building = building;
        foreach (var (resourceType, cost) in costs)
        {
            materials.Add(resourceType, new FactoryBlueprintMaterial(cost));
        }
    }

    public void ApplyLogisticRequest(LogisticSchedule logisticSchedule)
    {
        // TODO: 물류 처리가 불가능한 경우 처리
        materials[logisticSchedule.resourceType].AddMaterial(logisticSchedule.value);
    }

    public void RegisterLogistics(LogisticsSystem logisticsSystem)
    {
        this.logisticsSystem = logisticsSystem;
    }

    public void DoFactoryBlueprintPhase()
    {
        if (logisticsSystem == null)
        {
            return;
        }

        bool isFinished = true;
        foreach (var (resourceType, material) in materials)
        {
            int neededValue = material.NeededValue;
            isFinished &= neededValue == 0;
            logisticsSystem.UpdateRequests(this, resourceType, material.NeededValue);
        }
        IsFinished = isFinished;
    }
}
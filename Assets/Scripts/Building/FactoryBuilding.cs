using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryBuilding
{
    public bool isBlueprint;
    public FactoryBuildingClass buildingClass;
    public MapTile mapTile;
    public FactoryBlueprint blueprint;

    public List<FactoryBuilding> connectedNeighbors;
    public FactoryBuildingPrototype prototype;
    public BuildingObject buildingObject;



    public FactoryBuilding(FactoryBuildingClass buildingClass, MapTile mapTile)
    {
        this.buildingClass = buildingClass;
        this.mapTile = mapTile;
        this.connectedNeighbors = new List<FactoryBuilding>();
        this.buildingObject = null;

        // 조립기 필요없음: 블루프린트 방식
        this.isBlueprint = !buildingClass.isAssemblyRequired;
        if (isBlueprint)
        {
            blueprint = new FactoryBlueprint(this, buildingClass.costs);
            // TODO: 길 연결 상태 확인
            blueprint.RegisterLogistics(LogisticsSystemManager.Instance.logisticsSystems[0]);
        }

        switch (buildingClass.prototype)
        {
            case BuildingPrototype.Converter:
                ConverterPrototype converterPrototype = new ConverterPrototype(this, buildingClass.buildingName);
                converterPrototype.RegisterLogistics(LogisticsSystemManager.Instance.logisticsSystems[0]);
                prototype = converterPrototype;
                break;
            case BuildingPrototype.MinionHouse:
                prototype = new MinionHousePrototype(this);
                break;
            case BuildingPrototype.LogisticRoad:
                prototype = new LogisticRoadPrototype(this);
                break;
            default:
                buildingObject = null;
                break;
        }

        if (buildingClass.initPrototype != null)
        {
            buildingClass.initPrototype(prototype);
        }
    }

    public void RegisterBuildingObject(BuildingObject buildingObject)
    {
        Debug.Assert(this.buildingObject == null);
        this.buildingObject = buildingObject;
        buildingObject.building = this;
    }

    // 성공시 true
    public bool ConnectBuilding(FactoryBuilding other)
    {
        if (other == null)
        {
            return false;
        }

        if (connectedNeighbors.Contains(other) || other.connectedNeighbors.Contains(this))
        {
            Debug.Assert(connectedNeighbors.Contains(other) && other.connectedNeighbors.Contains(this));
            return false;
        }

        connectedNeighbors.Add(other);
        other.connectedNeighbors.Add(this);

        return true;
    }

    public void DoFactoryBuildingUpdatePhase()
    {
        if (prototype == null)
        {
            return;
        }

        if (isBlueprint)
        {
            Debug.Assert(blueprint != null);
            blueprint.DoFactoryBlueprintPhase();
            if (blueprint.IsFinished)
            {
                isBlueprint = false;
                blueprint = null;
            }
        }

        if (!isBlueprint)
        {
            prototype.Update();
        }
    }

    // TODO: 건물 입구를 나타내도록
    public Vector3 LogisticPortPosition
    {
        // TODO: 건물마다, 건물 회전마다 다르게
        get
        {
            return buildingObject.transform.Find("LogisticsInterface").transform.position;
        }
    }

    public string GetBuildingOverlayText()
    {
        string text = buildingClass.buildingName;
        if (isBlueprint)
        {
            text += blueprint.GetBlueprintOverlayText();
        }
        return text;
    }
}

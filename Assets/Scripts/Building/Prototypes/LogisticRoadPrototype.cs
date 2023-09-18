/*

Logistic Road Prototype

Minion이 이동할 수 있는 길

*/

using System;
using System.Collections;
using System.Collections.Generic;

public class LogisticRoadPrototype : FactoryBuildingPrototype
{
    // TODO: 구현
    public LogisticRoadPrototype(FactoryBuilding factoryBuilding) : base(factoryBuilding)
    {

    }

    public override void Start()
    {
        base.Start();

        if (factoryBuilding.buildingClass.resourceType == ResourceType.B_StoneRoad)
        {
            factoryBuilding.buildingObject.transform.SetParent(BuildModeManager.Instance.surfaceTransform);
            factoryBuilding.buildingObject.tag = "LogisticRoad";
        }
    }

    public override void FinishBuildMode()
    {
        AstarPath.active.Scan();
    }
}
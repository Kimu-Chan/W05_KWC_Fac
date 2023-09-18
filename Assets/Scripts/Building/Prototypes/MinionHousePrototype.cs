/*

Minion House Prototype

MinionObject가 필요시 GameObject로 Instantiate 되고, 작업이 없을 때 돌아가 Destroy 되는 시설의 Prototype입니다

*/

using UnityEngine;

public class MinionHousePrototype : FactoryBuildingPrototype, ILogisticAvailable
{
    LogisticsSystem logisticsSystem;
    // TODO: 구현
    public MinionHousePrototype(FactoryBuilding factoryBuilding) : base(factoryBuilding)
    {

    }

    public Vector3 LogisticPortPosition => factoryBuilding.LogisticPortPosition;

    public override void MouseDownCallback()
    {
        base.MouseDownCallback();
        var obj = MinionManager.Instance.Instantiate(MinionManager.Instance.GenerateMinion("hello>?"));
        obj.transform.position = LogisticPortPosition;
    }

    public void RegisterLogistics(LogisticsSystem logisticsSystem)
    {
        this.logisticsSystem = logisticsSystem;
    }
}
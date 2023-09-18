using System;
using System.Collections;
using System.Collections.Generic;

public abstract class FactoryBuildingPrototype
{
    protected FactoryBuilding factoryBuilding;
    public FactoryBuildingPrototype(FactoryBuilding factoryBuilding)
    {
        this.factoryBuilding = factoryBuilding;
    }

    public virtual void Update()
    {

    }
    public virtual void Start()
    {

    }

    // 해당 건물의 buildmode가 끝난 후 호출됨
    // 드래그 방식일 경우, 마지막 건물이 건설된 이후 단 한번 호출
    public virtual void FinishBuildMode()
    {

    }

    public virtual void MouseDownCallback()
    {
    }
}
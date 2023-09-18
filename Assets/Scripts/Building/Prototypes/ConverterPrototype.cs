/*

Converter Prototype

특정 자원을 다른 자원으로 변환하는 시설의 프로토타입

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConverterItem
{
    public int convertValue;
    public int currentValue;
    public ConverterItem(int convertValue)
    {
        this.convertValue = convertValue;
        this.currentValue = 0;
    }
    public bool IsSatisfied
    {
        get
        {
            return convertValue <= currentValue;
        }
    }

    public void UseResource()
    {
        currentValue -= convertValue;
    }

    public void GenerateResource()
    {
        currentValue += convertValue;
    }
}

public class ConverterPrototype : FactoryBuildingPrototype, ILogisticAvailable, ILogisticProvideAvailable, ILogisticRequestAvailable
{

    protected LogisticsSystem logisticsSystem;
    public string Name { get; set; }

    public Vector3 LogisticPortPosition
    {
        get
        {
            return factoryBuilding.LogisticPortPosition;
        }
    }

    public ConverterPrototype(FactoryBuilding factoryBuilding, String name) : base(factoryBuilding)
    {
        Name = name;
        isWorking = false;
    }

    public Dictionary<ResourceType, ConverterItem> inputs = new Dictionary<ResourceType, ConverterItem>();
    public Dictionary<ResourceType, ConverterItem> outputs = new Dictionary<ResourceType, ConverterItem>();

    public float IntervalTick { get; set; }
    private bool isWorking;
    private long startTime;
    public float Progress
    {
        get
        {
            if (!isWorking) return 0;
            if (IsFinished) return 1;
            return (GameManager.Tick - startTime) / IntervalTick;
        }
    }

    public void ApplyLogisticRequest(LogisticSchedule logisticSchedule)
    {
        // TODO: 물류 처리가 불가능한 경우 처리
        inputs[logisticSchedule.resourceType].currentValue += logisticSchedule.value;
    }

    public void ApplyLogisticProvide(LogisticSchedule logisticSchedule)
    {
        outputs[logisticSchedule.resourceType].currentValue -= logisticSchedule.value;
        Debug.Log($"{logisticSchedule.value}, next current value: {outputs[logisticSchedule.resourceType].currentValue}");
    }

    public void SetInput(ResourceType resourceType, int value)
    {
        inputs[resourceType] = new ConverterItem(value);
    }

    public void SetOutput(ResourceType resourceType, int value)
    {
        outputs[resourceType] = new ConverterItem(value);
    }

    public bool IsFinished
    {
        get
        {
            return (GameManager.Tick - startTime) >= IntervalTick;
        }
    }

    public override void Update()
    {
        base.Update();

        do
        {
            if (isWorking)
            {
                if (IsFinished)
                {
                    foreach (var v in inputs.Values)
                    {
                        v.UseResource();
                    }
                    foreach (var v in outputs.Values)
                    {
                        v.GenerateResource();
                    }
                    isWorking = false;
                }
            }
            else
            {
                bool canStartWork = true;

                // 자원 개수 비교
                foreach (var v in inputs.Values)
                {
                    canStartWork &= v.IsSatisfied;
                }

                if (canStartWork)
                {
                    startTime = GameManager.Tick;
                    isWorking = true;
                    continue;
                }
            }

            break;
        } while (true);

        // Logistic 정보 업데이트
        foreach (var (resourceType, converterItem) in inputs)
        {
            // TODO: 미리 요청해 놓는 비율 조절
            // 1배까지 요청
            int request = Math.Max(converterItem.convertValue * 1 - converterItem.currentValue, 0);
            logisticsSystem.UpdateRequests(this, resourceType, request);
        }

        foreach (var (resourceType, converterItem) in outputs)
        {
            int provide = converterItem.currentValue;
            logisticsSystem.UpdateProvides(this, resourceType, provide);
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public void RegisterLogistics(LogisticsSystem logisticsSystem)
    {
        this.logisticsSystem = logisticsSystem;
    }

    public override void MouseDownCallback()
    {
        base.MouseDownCallback();
        UIManager.Instance.TryOpenBuildingWindow(factoryBuilding);
    }
}
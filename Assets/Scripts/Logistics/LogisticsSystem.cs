using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LogisticSchedule
{
    public LogisticsSystem logisticsSystem;
    public ResourceType resourceType;
    public ILogisticProvideAvailable provider;
    public ILogisticRequestAvailable requester;
    public int value;
    public LogisticSchedule(LogisticsSystem logisticsSystem, ResourceType resourceType, ILogisticProvideAvailable provider, ILogisticRequestAvailable requester, int value)
    {
        this.logisticsSystem = logisticsSystem;
        this.resourceType = resourceType;
        this.provider = provider;
        this.requester = requester;
        this.value = value;
    }

    public void ApplyLogisticProvide()
    {
        Debug.Log("LogisticSchedule ApplyLogisticProvide");
        provider.ApplyLogisticProvide(this);

        logisticsSystem.provides[resourceType][provider].reservedValue -= value;
        // logisticsSystem.provides[resourceType][provider].originalValue -= value;
    }

    public void ApplyLogisticRequest()
    {
        requester.ApplyLogisticRequest(this);

        logisticsSystem.requests[resourceType][requester].reservedValue += value;
        // logisticsSystem.requests[resourceType][requester].originalValue += value;
    }
}

public class LogisticValue
{
    public int originalValue;
    public int reservedValue;
    public LogisticValue(int originalValue)
    {
        this.originalValue = originalValue;
        reservedValue = 0;
    }
    public int AvailableValue
    {
        get
        {
            // Debug.Log($"AvailableValue: {originalValue}, {reservedValue}");
            return originalValue - reservedValue;
        }
    }
}

public class LogisticsSystem
{
    public Dictionary<ResourceType, Dictionary<ILogisticRequestAvailable, LogisticValue>> requests = new Dictionary<ResourceType, Dictionary<ILogisticRequestAvailable, LogisticValue>>();
    public Dictionary<ResourceType, Dictionary<ILogisticProvideAvailable, LogisticValue>> provides = new Dictionary<ResourceType, Dictionary<ILogisticProvideAvailable, LogisticValue>>();
    public Queue<LogisticSchedule> logisticQueue = new Queue<LogisticSchedule>();


    private void UpdateOriginalValue<T>(ref Dictionary<ResourceType, Dictionary<T, LogisticValue>> dict, T factoryBuilding, ResourceType resourceType, int changedOriginalValue)
    {
        if (dict.ContainsKey(resourceType))
        {
            var d = dict[resourceType];
            if (d.ContainsKey(factoryBuilding))
            {
                d[factoryBuilding].originalValue = changedOriginalValue;
            }
            else
            {
                d[factoryBuilding] = new LogisticValue(changedOriginalValue);
            }
        }
        else
        {
            var d = new Dictionary<T, LogisticValue>();
            d[factoryBuilding] = new LogisticValue(changedOriginalValue);
            dict[resourceType] = d;
        }
    }

    public void UpdateRequests(ILogisticRequestAvailable buildingObject, ResourceType resourceType, int value)
    {
        lock (this)
        {
            UpdateOriginalValue(ref requests, buildingObject, resourceType, -value);
        }
    }

    public void UpdateProvides(ILogisticProvideAvailable buildingObject, ResourceType resourceType, int value)
    {
        lock (this)
        {
            UpdateOriginalValue(ref provides, buildingObject, resourceType, value);
        }
    }

    public LogisticSchedule TryDequeueLogisticSchedule()
    {
        lock (this)
        {
            if (logisticQueue.Count == 0)
            {
                return null;
            }
            else
            {
                return logisticQueue.Dequeue();
            }
        }
    }

    public LogisticSchedule EnqueueLogisticSchedule(ResourceType resourceType, ILogisticProvideAvailable provider, ILogisticRequestAvailable requester, int value)
    {
        LogisticSchedule newSchedule = new LogisticSchedule(this, resourceType, provider, requester, value);
        Debug.Log($"Logistic Schedule. Value: {value}");
        logisticQueue.Enqueue(newSchedule);
        return newSchedule;
    }

    public void DoLogisticSystemUpdatePhase()
    {
        var availableResources = requests.Keys.Intersect(provides.Keys);
        foreach (var targetResource in availableResources)
        {
            var requestDict = requests[targetResource];
            var provideDict = provides[targetResource];

            var provideKeys = provideDict.Keys.ToArray();
            int currentProvideIdx = 0;
            int provideCount = provideDict.Count;

            // TODO: 더 짧은 거리 우선
            // TODO: 물류 우선순위
            // TODO: 랜덤 or 분산

            foreach (var requestPair in requestDict)
            {
                var (requestFactoryBuilding, requestValue) = requestPair;
                while (currentProvideIdx < provideKeys.Count() && requestValue.AvailableValue < 0)
                {
                    Debug.Log(requestValue.AvailableValue);
                    var provideFactoryBuilding = provideKeys[currentProvideIdx];
                    var provideValue = provideDict[provideFactoryBuilding];

                    // 요청된 양. requestValue.AvailableValue는 항상 음수이다
                    int currentRequestedValue = -requestValue.AvailableValue;
                    Debug.Log($"currentRequestedValue: {currentRequestedValue}");

                    // 만약 currentProvide의 가용량이 요청된 양을 충족시키면
                    if (currentRequestedValue <= provideValue.AvailableValue)
                    {
                        // request - reserved 음수
                        // provide - reserved 양수
                        requestValue.reservedValue -= currentRequestedValue;
                        provideValue.reservedValue += currentRequestedValue;

                        // 물류 스케쥴 추가
                        EnqueueLogisticSchedule(targetResource, provideFactoryBuilding, requestFactoryBuilding, currentRequestedValue);
                    }
                    // 충족시키지 않았으면
                    // provide의 모든 양을 request로 운송
                    // 이후 다음 provide로 진행
                    else
                    {
                        int currentAvaliableProvideValue = provideValue.AvailableValue;
                        if (currentAvaliableProvideValue > 0)
                        {
                            Debug.Log($"currentAvaliableProvideValue: {currentAvaliableProvideValue}");

                            requestValue.reservedValue -= currentAvaliableProvideValue;
                            provideValue.reservedValue += currentAvaliableProvideValue;

                            // 물류 스케쥴 추가
                            EnqueueLogisticSchedule(targetResource, provideFactoryBuilding, requestFactoryBuilding, currentAvaliableProvideValue);
                        }

                        // 다음 provide로
                        ++currentProvideIdx;
                    }

                    // 실제 dict에 반영? - class므로 괜찮음
                    // requestDict[requestBuildingObject] = requestValue;
                    // provideDict[provideBuildingObject] = provideValue;
                }

                // Provider가 남지 않았으면 break
                if (currentProvideIdx == provideCount)
                {
                    break;
                }
            }

        }
    }
}
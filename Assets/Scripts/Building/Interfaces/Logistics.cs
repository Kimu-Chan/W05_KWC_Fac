using UnityEngine;

public interface ILogisticAvailable
{
    public void RegisterLogistics(LogisticsSystem logisticsSystem);
    public Vector3 LogisticPortPosition { get; }
}

public interface ILogisticRequestAvailable : ILogisticAvailable
{
    public void ApplyLogisticRequest(LogisticSchedule schedule);
}

public interface ILogisticProvideAvailable : ILogisticAvailable
{
    public void ApplyLogisticProvide(LogisticSchedule schedule);
}
using System;
using System.Collections;
using System.Collections.Generic;

public class LogisticsSystemManager
{
    private static LogisticsSystemManager logisticsSystemManager = null;
    public static LogisticsSystemManager Instance
    {
        get
        {
            if (logisticsSystemManager == null)
            {
                logisticsSystemManager = new LogisticsSystemManager();
            }
            return logisticsSystemManager;
        }
    }

    public List<LogisticsSystem> logisticsSystems = new List<LogisticsSystem>();
    private LogisticsSystemManager()
    {
        // TODO: 지금은 전역 logisitics
        logisticsSystems.Add(new LogisticsSystem());
    }

    public void DoAllLogisticsSystemUpdatePhase()
    {
        foreach (var logisitcsSystem in logisticsSystems)
        {
            logisitcsSystem.DoLogisticSystemUpdatePhase();
        }
    }
}
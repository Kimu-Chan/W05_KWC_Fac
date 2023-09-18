using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionStatus
{
    public int Power;
    public int Energy;
    public float Speed;
}

public class Minion
{
    public readonly int minionID;
    public string Name { get; set; }
    public MinionJob currentJob = null;
    public Queue<MinionJob> jobQueue = new Queue<MinionJob>();
    public MinionStatus minionStatus;
    public MinionObject minionObject;

    public Minion(int minionID, string name = null)
    {
        this.minionID = minionID;

        if (name == null)
        {
            Name = "TODO: Random Name";
        }
        else
        {
            Name = name;
        }

        minionStatus = new MinionStatus();

        minionStatus.Power = 1;
        minionStatus.Energy = 50;
        minionStatus.Speed = 1;
    }

    public void DoMinionAssignJobPhase()
    {
        if (currentJob != null)
        {
            return;
        }
        else if (jobQueue.Count > 0)
        {
            currentJob = jobQueue.Dequeue();
            return;
        }

        // TODO: minion의 욕구가 부족하다 -> 욕구 충족 우선?

        // Logistic 요청 처리
        // TODO: logistic system 마다 다른 minion 할당?
        foreach (var logisticsSystem in LogisticsSystemManager.Instance.logisticsSystems)
        {
            LogisticSchedule schedule = logisticsSystem.TryDequeueLogisticSchedule();
            Debug.Log(schedule);
            if (schedule != null)
            {
                Debug.Log("getJob");
                currentJob = new LogisticDroneJob(this, schedule);
            }
        }
    }

    public void DoMinionJobPhase()
    {
        if (currentJob != null)
        {
            currentJob.DoJobPhase();
        }
    }

    public bool DoMinionCheckJobFinishedPhase()
    {
        if (currentJob != null && currentJob.IsFinished)
        {
            currentJob = null;
            return true;
        }
        return false;
    }
}
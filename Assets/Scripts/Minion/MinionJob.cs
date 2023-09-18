using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinionJob
{
    public string jobName;
    public Minion minion;
    public abstract bool IsFinished { get; }
    public abstract float Progress { get; }
    public abstract void DoJobPhase();
    public MinionJob(Minion minion)
    {
        this.minion = minion;
    }
}

public class MovementJob : MinionJob
{
    public float epsilon = 1f;
    private Vector3 start;
    private Vector3 destination;
    private float distance;
    private bool needDestinationSet;
    private Pathfinding.IAstarAI pf;
    public MovementJob(Minion minion, Vector3 destination) : base(minion)
    {
        jobName = "minion movement";

        start = minion.minionObject.transform.position;
        this.destination = destination;
        Debug.Log($"New destination: {this.destination}");
        distance = Vector3.Distance(start, destination);
        needDestinationSet = true;

        pf = minion.minionObject.GetComponent<Pathfinding.AIPath>();
        pf.maxSpeed = 5;
    }
    public override bool IsFinished
    {
        get
        {
            Debug.Log($"isFinished?: {pf.reachedDestination}");
            if (pf.reachedDestination)
            {
            }
            if (needDestinationSet)
            {
                return false;
            }
            return pf.reachedDestination;
        }
    }

    public override float Progress
    {
        get
        {
            return IsFinished ? 1 : Vector3.Distance(minion.minionObject.transform.position, destination) / distance;
        }
    }

    public override void DoJobPhase()
    {
        if (needDestinationSet)
        {
            pf.destination = destination;
            pf.SearchPath();
            needDestinationSet = false;
        }
        else
        {

        }
        // minion.minionObject.transform.position = Vector3.MoveTowards(minion.minionObject.transform.position, destination, minion.minionStatus.Speed);
    }
}

public class LogisticDroneJob : MinionJob
{
    public enum LogisticDroneJobStatus
    {
        Pending,
        MoveToProvider,
        MoveToRequester,
        Finished
    }

    private MovementJob currentMovementJob = null;
    private LogisticSchedule logisticSchedule;
    public LogisticDroneJobStatus logisticDroneJobStatus = LogisticDroneJobStatus.Pending;

    public LogisticDroneJob(Minion minion, LogisticSchedule logisticSchedule) : base(minion)
    {
        jobName = "logistic drone";
        this.logisticSchedule = logisticSchedule;
    }

    public override bool IsFinished
    {
        get
        {
            return logisticDroneJobStatus == LogisticDroneJobStatus.Finished;
        }
    }

    public override float Progress
    {
        get
        {
            return 0;
        }
    }

    public override void DoJobPhase()
    {
        // TODO: 운송 중에 건물이 삭제/이동되면?
        // TODO: power 기반 물품 받는 시간?

        switch (logisticDroneJobStatus)
        {
            case LogisticDroneJobStatus.Pending:
                currentMovementJob = new MovementJob(minion, logisticSchedule.provider.LogisticPortPosition);
                logisticDroneJobStatus = LogisticDroneJobStatus.MoveToProvider;
                break;
            case LogisticDroneJobStatus.MoveToProvider:
                if (currentMovementJob.IsFinished)
                {
                    // Provider에게서 물품 받기
                    logisticSchedule.ApplyLogisticProvide();

                    currentMovementJob = new MovementJob(minion, logisticSchedule.requester.LogisticPortPosition);
                    logisticDroneJobStatus = LogisticDroneJobStatus.MoveToRequester;
                }
                else
                {
                    currentMovementJob.DoJobPhase();
                }
                break;
            case LogisticDroneJobStatus.MoveToRequester:
                if (currentMovementJob.IsFinished)
                {
                    // Provider에게서 물품 받기
                    logisticSchedule.ApplyLogisticRequest();
                    logisticDroneJobStatus = LogisticDroneJobStatus.Finished;
                }
                else
                {
                    currentMovementJob.DoJobPhase();
                }
                break;
        }
    }
}
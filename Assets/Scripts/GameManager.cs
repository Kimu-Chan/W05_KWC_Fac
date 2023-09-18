using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager = null;
    public const int TickMillisecond = 20;
    private static long _tick = 0;
    public static long Tick
    {
        get
        {
            return _tick;
        }
    }

    public static long MillisecondToTick(long millisecond)
    {
        return millisecond / TickMillisecond;
    }

    public static long TickToMillisecond(long tick)
    {
        return tick * TickMillisecond;
    }

    public GameManager Instance
    {
        get
        {
            return gameManager;
        }
    }

    void Awake()
    {
        gameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        ++_tick;

        // Logisitics 요청 업데이트
        LogisticsSystemManager.Instance.DoAllLogisticsSystemUpdatePhase();

        // Minion에 일 할당
        MinionManager.Instance.DoAllMinionsAssignJobPhase();

        // Minion 작업 진행 처리
        MinionManager.Instance.DoAllMinionsJobPhase();

        // Minion 작업 완료한 경우 처리
        MinionManager.Instance.DoAllMinionCheckJobFinishedPhase();

        // 

        // Surface 내의 공장 업데이트
        SurfaceManager.Instance.DoAllSurfacesUpdatePhase();

    }
}

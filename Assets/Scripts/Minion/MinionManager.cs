using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager
{
    private static MinionManager minionManager = null;
    public static MinionManager Instance
    {
        get
        {
            if (minionManager == null)
            {
                minionManager = new MinionManager();
            }
            return minionManager;
        }
    }

    private int minionID = 0;
    private Dictionary<int, Minion> minions = new Dictionary<int, Minion>();

    private MinionManager()
    {

    }

    public Minion GenerateMinion(string name = null)
    {
        Minion minion = new Minion(minionID, name);
        minions[minionID++] = minion;
        return minion;
    }

    public GameObject Instantiate(Minion minion)
    {
        GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Minion"));
        MinionObject minionObj = obj.GetComponent<MinionObject>();
        minionObj.minion = minion;
        minion.minionObject = minionObj;
        return obj;
    }

    public void DoAllMinionsAssignJobPhase()
    {
        foreach (var minion in minions.Values)
        {
            minion.DoMinionAssignJobPhase();
        }
    }

    public void DoAllMinionsJobPhase()
    {
        foreach (var minion in minions.Values)
        {
            minion.DoMinionJobPhase();
        }
    }

    public void DoAllMinionCheckJobFinishedPhase()
    {
        foreach (var minion in minions.Values)
        {
            minion.DoMinionCheckJobFinishedPhase();
        }
    }
}
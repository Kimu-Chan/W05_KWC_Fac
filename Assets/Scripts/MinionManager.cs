using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinionManagerRegacy : MonoBehaviour
{
    public int MinionSpeed = 3;
    public int MinionNum = 1;
    public GameObject countText;
    static public int speed;
    static public int minionNum;
    public static int resourceCount = 0;
    public PathFinder pf;
    public static int currentMinionNum = 0;
    GameObject minion;
    int targetNum;
    int targetIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        speed = MinionSpeed;
        minionNum = MinionNum;
        minion = Resources.Load("Minion") as GameObject;
        targetNum = pf.targetList.Count;
    }

    // Update is called once per frame
    void Update()
    {
        countText.GetComponent<TMP_Text>().text = resourceCount.ToString();
        if (currentMinionNum < minionNum && targetIdx < pf.targetList.Count)
        {
            List<Node> path = pf.PathFinding(pf.targetList[targetIdx].x, pf.targetList[targetIdx].y);
            targetIdx++;
            addMinion().GetComponent<MinionBehaviour>().Work(path);
        }
    }
    GameObject addMinion()
    {
        GameObject clone = Instantiate(minion);
        clone.name = "minion";
        Vector3 pos = new Vector3(0, 0, 0);
        clone.transform.position = pos;
        currentMinionNum++;
        return clone;
    }
}

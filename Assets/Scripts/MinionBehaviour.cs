using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionBehaviour : MonoBehaviour
{
    Vector2 targetPos;
    bool arrival = false;
    int idx = 0;
    List<Node> path = null;
    public void Work(List<Node> _path)
    {
        path = _path;
        targetPos = new Vector2(path[0].x, path[0].y);
    }

    // Update is called once per frame
    void Update()
    {
        if (path != null)
        {
            if (transform.position.Equals(targetPos))
            {
                if (idx < path.Count - 1)
                {
                    idx++;
                    targetPos = new Vector2(path[idx].x, path[idx].y);
                }
                else
                {
                    if (arrival)
                    {
                        MinionManagerRegacy.resourceCount++;
                        MinionManagerRegacy.currentMinionNum--;
                        Destroy(gameObject);
                    }
                    else
                    {
                        arrival = true;
                        path.Reverse();
                        path.Add(new Node(false, 0, 0));
                        idx = 1;
                        targetPos = new Vector2(path[idx].x, path[idx].y);
                    }
                }
            }
            transform.position = Vector2.MoveTowards(transform.position, targetPos, MinionManagerRegacy.speed * Time.deltaTime);

        }
    }

    void OnDrawGizmos()
    {
        if (path != null) for (int i = 0; i < path.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(path[i].x, path[i].y), new Vector2(path[i + 1].x, path[i + 1].y));
    }
}

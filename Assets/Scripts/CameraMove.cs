using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject target;
    Vector3 dposition;
    // Start is called before the first frame update
    void Start()
    {
        if(target != null)
            dposition = (transform.position - target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
            transform.position = target.transform.position+ dposition;
    }
}

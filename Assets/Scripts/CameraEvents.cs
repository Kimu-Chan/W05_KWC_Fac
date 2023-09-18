using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEvents : MonoBehaviour
{
    public float cameraSpeedMultiplyer = 0.5f;
    public float cameraScrollMultiplyer = 10f;
    
    private Camera cameraComponent;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float xMove = 0f;
        float yMove = 0f;
        float scroll = Input.GetAxis("Mouse ScrollWheel") * cameraScrollMultiplyer;

        cameraComponent.orthographicSize = Math.Clamp(cameraComponent.orthographicSize + scroll, 20f, 60f);

        if (Input.GetKey(KeyCode.W))
        {
            yMove += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            yMove -= 1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            xMove -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            xMove += 1f;
        }

        transform.Translate(new Vector3(xMove, yMove, 0) * cameraSpeedMultiplyer);
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class BuildingObject : MonoBehaviour
{
    public bool isActivated;
    public bool IsBlueprint
    {
        get
        {
            if (building == null)
            {
                return false;
            }

            return building.isBlueprint;
        }
    }
    public bool isMouseTracking;
    public bool isPipeSending;
    public FactoryBuilding building;
    private Color beforeColor;
    private BuildModeManager buildModeManager;

    private Camera mainCamera;


    void Awake()
    {
        isActivated = false;
        isMouseTracking = false;
        beforeColor = GetComponent<Renderer>().material.color;
    }

    void Start()
    {
        mainCamera = UnityEngine.Camera.main;
    }

    void Update()
    {
        if (isMouseTracking)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            transform.position = SurfaceManager.RoundWorldPosToGrid(mousePos);
        }
        if (building != null && building.buildingClass.hasOverlay)
        {
            OverlayTextManager.Instance.AddOrUpdateOverlay(gameObject, building.GetBuildingOverlayText());
        }
        if (IsBlueprint)
        {
        }
    }

    private void OnMouseDown()
    {
        if (!isActivated)
        {
            return;
        }

        if (building.buildingClass.isPipeSender && !isPipeSending)
        {
            Debug.Log("PipeSending");
            if (!GlobalLocks.TryLock(ref GlobalLocks.uiLock))
            {
                return;
            }
            Debug.Log("PipeSending!!");
            isPipeSending = true;
            StartCoroutine("MakePipe");
        }

        if (building.buildingClass.prototype != BuildingPrototype.Invalid)
        {
            building.prototype.MouseDownCallback();
        }
    }

    public IEnumerator MakePipe()
    {
        GameObject lineObject = new GameObject();
        LineRenderer renderer = lineObject.AddComponent<LineRenderer>();
        renderer.SetPosition(0, transform.position);

        MapTile target = null; ;
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                break;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = SurfaceManager.RoundWorldPosToGrid(mousePos);
            mousePos.z = transform.position.z;

            float xDiff = Math.Abs(mousePos.x - transform.position.x);
            float yDiff = Math.Abs(mousePos.y - transform.position.y);

            if (xDiff < yDiff)
            {
                mousePos.x = transform.position.x;
            }
            else
            {
                mousePos.y = transform.position.y;
            }

            var (mapX, mapY) = SurfaceManager.WorldPosToMapPos(mousePos.x, mousePos.y);
            target = SurfaceManager.selectedSurface.GetTile(mapX, mapY);
            if (target != null && !target.building.buildingClass.isPipeReciver)
            {
                target = null;
            }

            if (target != null)
            {
                renderer.material.color = Color.gray;
            }
            else
            {
                renderer.material.color = Color.red;
            }
            renderer.SetPosition(1, mousePos);

            yield return null;
        }

        if (target == null)
        {
            Debug.Log("PipeSending failure");
            Destroy(lineObject);
            isPipeSending = false;
            GlobalLocks.UnLock(ref GlobalLocks.uiLock);
        }
        else
        {
            isPipeSending = false;
            GlobalLocks.UnLock(ref GlobalLocks.uiLock);
        }
    }

    public void OnMouseTracking()
    {
        isMouseTracking = true;
        beforeColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void OffMouseTracking()
    {
        isMouseTracking = false;
        GetComponent<Renderer>().material.color = beforeColor;
    }

    public void Activate()
    {
        isActivated = true;
        if (building.prototype != null)
        {
            building.prototype.Start();
        }
    }

    private void OnDrawGizmos()
    {
        if (isActivated && building != null)
        {
            foreach (var neighbor in building.connectedNeighbors)
            {
                if (neighbor.buildingObject != null)
                {
                    Gizmos.color = new Color32(145, 244, 139, 210);
                    Gizmos.DrawLine(transform.position, neighbor.buildingObject.transform.position);
                }
            }
        }
    }
}
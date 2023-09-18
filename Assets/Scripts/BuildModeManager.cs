using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildModeManager : MonoBehaviour
{
    public SurfaceManager mapManager;
    public bool isBuildMode;
    public ResourceType buildTargetType;
    public Dictionary<ResourceType, GameObject> buildingPrefabs;
    public GameObject infiniteConverterGameObject;
    public Transform surfaceTransform;
    public static List<ResourceType> currentKeybindBuildings;
    private static BuildModeManager buildModeManager;
    public static BuildModeManager Instance
    {
        get
        {
            return buildModeManager;
        }
    }


    private BuildingRot curRot;

    private void Awake()
    {
        isBuildMode = false;
        buildingPrefabs = new Dictionary<ResourceType, GameObject>();

        buildModeManager = this;
        foreach (ResourceType buildingType in FactoryBuildingManager.Instance.BuildingResourceTypes)
        {
            FactoryBuildingClass buildingClass = FactoryBuildingManager.Instance.GetBuildingClass(buildingType);

            // Prefab 가공
            // 건물 scale은 buildingObject에서 함
            if (buildingClass.prefabName == null)
            {
                GameObject prefab = Resources.Load($"BuildingPrefabs/{buildingClass.xSize}x{buildingClass.ySize}_Quad") as GameObject;
                if (prefab.GetComponent<BuildingObject>() == null)
                {
                    prefab.AddComponent<BuildingObject>();
                }
                buildingPrefabs.Add(buildingType, prefab);
            }
            else
            {
                GameObject prefab = Resources.Load($"BuildingPrefabs/{buildingClass.prefabName}") as GameObject;
                if (prefab.GetComponent<BuildingObject>() == null)
                {
                    prefab.AddComponent<BuildingObject>();
                }
                buildingPrefabs.Add(buildingType, prefab);
            }
        }

        // buildingPrefabs.Add(ResourceType.B_SolarPanel_1, testSolarPanelGameObject);
        // buildingPrefabs.Add(ResourceType.B_Sender, testSenderGameObject);
        // buildingPrefabs.Add(ResourceType.B_Receiver, testReceiverGameObject);
        // buildingPrefabs.Add(ResourceType.B_Cross, testCrossGameObject);
        // buildingPrefabs.Add(ResourceType.B_ArmMotor_1, testMotorGameObject);
        // buildingPrefabs.Add(ResourceType.B_InfiniteConverter, infiniteConverterGameObject);
    }

    private void Update()
    {
        CheckBuildKeys();
        if (!isBuildMode)
        {
        }
    }

    public void HandleBuildMode(ResourceType buildingType)
    {
        if (isBuildMode)
        {
            isBuildMode = false;
        }
        else
        {
            isBuildMode = true;
            buildTargetType = buildingType;
            StartCoroutine("BuildMode", buildTargetType);
        }
    }

    private void CheckBuildKeys()
    {
        if (currentKeybindBuildings != null)
        {
            for (int i = 0; i < Math.Min(currentKeybindBuildings.Count, 12); ++i)
            {
                if (Input.GetKeyDown(KeyCode.F1 + i))
                {
                    HandleBuildMode(currentKeybindBuildings[i]);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            curRot = curRot.RotCW();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            curRot = curRot.RotCCW();
        }
        if (Input.GetMouseButton(1))
        {
            isBuildMode = false;
        }
    }

    public IEnumerator BuildMode(ResourceType type)
    {
        if (!GlobalLocks.TryLock(ref GlobalLocks.uiLock))
        {
            yield break;
        }

        curRot = BuildingRot.UP;
        BuildingRot recentRot = BuildingRot.UP;

        var fbm = FactoryBuildingManager.Instance;
        var buildingPrefab = buildingPrefabs[type];
        var buildingClass = fbm.GetBuildingClass(type);
        while (isBuildMode)
        {
            if (buildingClass.isBuildDragAble)
            {
                // TODO
                while (true && isBuildMode)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        break;
                    }
                    if (curRot != recentRot)
                    {
                        recentRot = curRot;
                        // obj.transform.rotation = recentRot.ToQuaternion();
                    }
                    yield return 0;
                }

                Vector3 mousePos = SurfaceManager.RoundWorldPosToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                mousePos.z = -1;

                (int startXPos, int startYPos) = SurfaceManager.WorldPosToMapPos((int)mousePos.x, (int)mousePos.y);
                int lastXPos = startXPos;
                int lastYPos = startYPos;
                List<BuildingObject> tmpObjects = new List<BuildingObject>();
                Dictionary<BuildingObject, Tuple<int, int>> tmpObjectsPos = new Dictionary<BuildingObject, Tuple<int, int>>();
                while (true && isBuildMode)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        break;
                    }
                    mousePos = SurfaceManager.RoundWorldPosToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    mousePos.z = -1;

                    (int currentXPos, int currentYPos) = SurfaceManager.WorldPosToMapPos((int)mousePos.x, (int)mousePos.y);
                    if (currentXPos == lastXPos && currentYPos == lastYPos)
                    {
                        yield return 0;
                        continue;
                    }
                    foreach (var tmpObj in tmpObjects)
                    {
                        tmpObj.gameObject.SetActive(false);
                    }
                    lastXPos = currentXPos;
                    lastYPos = currentYPos;

                    int xDiff = currentXPos - startXPos;
                    int yDiff = currentYPos - startYPos;

                    int tmpBuildingIdx = 0;
                    Action<int, int> tmpBuilder = (xPos, yPos) =>
                    {
                        (float worldXPos, float worldYPos) = SurfaceManager.MapPosToWorldPos(xPos, yPos);
                        BuildingObject tmpObj = null;
                        if (tmpBuildingIdx >= tmpObjects.Count)
                        {
                            tmpObj = Instantiate(buildingPrefab, mousePos, buildingPrefab.transform.rotation).GetComponent<BuildingObject>();
                            tmpObjects.Add(tmpObj);
                        }
                        else
                        {
                            tmpObj = tmpObjects[tmpBuildingIdx];
                        }
                        ++tmpBuildingIdx;


                        if (SurfaceManager.selectedSurface.IsBuildAble(xPos, xPos, yPos, yPos))
                        {
                            tmpObj.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        }
                        else
                        {
                            tmpObj.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        }

                        tmpObjectsPos[tmpObj] = new Tuple<int, int>(xPos, yPos);

                        tmpObj.transform.position = new Vector3(worldXPos, worldYPos, tmpObj.transform.position.z);
                        tmpObj.gameObject.SetActive(true);
                    };

                    if (Math.Abs(xDiff) >= Math.Abs(yDiff))
                    {
                        if (xDiff > 0)
                        {
                            for (int xPos = startXPos; xPos <= currentXPos; ++xPos)
                            {
                                tmpBuilder(xPos, startYPos);
                            }
                        }
                        else
                        {
                            for (int xPos = startXPos; xPos >= currentXPos; --xPos)
                            {
                                tmpBuilder(xPos, startYPos);
                            }
                        }
                    }
                    else
                    {

                        if (yDiff > 0)
                        {
                            for (int yPos = startYPos; yPos <= currentYPos; ++yPos)
                            {
                                tmpBuilder(startXPos, yPos);
                            }
                        }
                        else
                        {
                            for (int yPos = startYPos; yPos >= currentYPos; --yPos)
                            {
                                tmpBuilder(startXPos, yPos);
                            }
                        }
                    }
                }

                FactoryBuilding lastBuildedBuilding = null;

                if (!isBuildMode)
                {
                    Debug.Log("Drag Build Cancel");
                    foreach (var tmpObj in tmpObjects)
                    {
                        Destroy(tmpObj.gameObject);
                    }
                    GlobalLocks.UnLock(ref GlobalLocks.uiLock);
                    yield break;
                }
                else
                {
                    foreach (var tmpObj in tmpObjects)
                    {
                        if (!tmpObj.gameObject.activeSelf)
                        {
                            continue;
                        }
                        FactoryBuilding building = null;
                        (int xPos, int yPos) = tmpObjectsPos[tmpObj];
                        Debug.Log($"DRAG BUILD: {xPos}, {yPos}");
                        if (isBuildMode && !TryBuild(type, xPos, yPos, out building))
                        {
                            Debug.Log("Build Failure");
                            Destroy(tmpObj.gameObject);
                            // isBuildMode = false;
                            // GlobalLocks.UnLock(ref GlobalLocks.uiLock);
                            // yield break;
                        }
                        else
                        {
                            Debug.Assert(building != null);

                            // TODO: surface 다름

                            building.RegisterBuildingObject(tmpObj);
                            building.buildingObject.transform.SetParent(surfaceTransform);
                            lastBuildedBuilding = building;
                            tmpObj.gameObject.transform.position += Vector3.forward;
                            tmpObj.OffMouseTracking();
                            tmpObj.Activate();
                        }
                    }
                }

                lastBuildedBuilding?.prototype.FinishBuildMode();
            }
            else
            {
                Vector3 mousePos = SurfaceManager.RoundWorldPosToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                mousePos.z = -1;
                var obj = Instantiate(buildingPrefab, mousePos, buildingPrefab.transform.rotation);
                OverlayTextManager.Instance.AddOrUpdateOverlay(obj, buildingClass.buildingName);
                BuildingObject buildingObject = obj.GetComponent<BuildingObject>();
                buildingObject.OnMouseTracking();
                GameObject logisticsInterfaceObj = null;
                // Logistic Interface
                if (buildingClass.isLogisticsAvailable)
                {
                    logisticsInterfaceObj = Instantiate(Resources.Load("BuildingPrefabs/LogisticsInterface")) as GameObject;
                    logisticsInterfaceObj.name = "LogisticsInterface";
                    logisticsInterfaceObj.transform.SetParent(buildingObject.transform);
                    logisticsInterfaceObj.transform.position = buildingObject.transform.position + recentRot.ToQuaternion() * (Vector3.up * buildingClass.ySize + Vector3.up);
                }

                while (true && isBuildMode)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        break;
                    }
                    if (curRot != recentRot)
                    {
                        recentRot = curRot;
                        obj.transform.rotation = recentRot.ToQuaternion();
                        if (logisticsInterfaceObj != null)
                            logisticsInterfaceObj.transform.position = buildingObject.transform.position + recentRot.ToQuaternion() * (Vector3.up * fbm.GetBuildingClass(type).ySize + Vector3.up);
                    }
                    yield return 0;
                }

                while (true && isBuildMode)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        break;
                    }
                    if (curRot != recentRot)
                    {
                        recentRot = curRot;
                        obj.transform.rotation = recentRot.ToQuaternion();
                        if (logisticsInterfaceObj != null)
                            logisticsInterfaceObj.transform.position = buildingObject.transform.position + recentRot.ToQuaternion() * (Vector3.up * fbm.GetBuildingClass(type).ySize + Vector3.up);
                    }
                    yield return 0;
                }

                buildingObject.OffMouseTracking();

                if (!isBuildMode)
                {
                    Debug.Log("Build Cancel");
                    Destroy(obj);
                    GlobalLocks.UnLock(ref GlobalLocks.uiLock);
                    yield break;
                }

                (int xPos, int yPos) = SurfaceManager.WorldPosToMapPos((int)buildingObject.transform.position.x, (int)buildingObject.transform.position.y);
                FactoryBuilding building = null;

                if (isBuildMode && !TryBuild(type, xPos, yPos, out building))
                {
                    Debug.Log("Build Failure");
                    Destroy(obj);
                    isBuildMode = false;
                    GlobalLocks.UnLock(ref GlobalLocks.uiLock);
                    yield break;
                }
                else
                {
                    Debug.Assert(building != null);

                    // TODO: surface 다름

                    building.RegisterBuildingObject(buildingObject);
                    building.buildingObject.transform.SetParent(surfaceTransform);
                    buildingObject.gameObject.transform.position += Vector3.forward;
                    buildingObject.Activate();
                }

                building.prototype.FinishBuildMode();
                // isBuildMode = false;
            }

        }
        GlobalLocks.UnLock(ref GlobalLocks.uiLock);
    }

    public bool TryBuild(ResourceType buildingType, int xPos, int yPos, out FactoryBuilding building)
    {
        Debug.Log($"TryBuild {buildingType} {xPos} {yPos}");
        var selectedMap = SurfaceManager.selectedSurface;
        if (!selectedMap.IsBuildAble(buildingType, xPos, yPos))
        {
            building = null;
            return false;
        }

        building = selectedMap.AddBuilding(buildingType, xPos, yPos, curRot);
        return true;

        // // 왼쪽
        // {
        //     int xOffset = -buildingXCenter - 1;
        //     for (int yOffset = -buildingYCenter - 1; yOffset < buildingYSize - buildingYCenter + 1; ++yOffset)
        //     {
        //         int checkXPos = xPos + xOffset;
        //         int checkYPos = yPos + yOffset;
        //         var checkTile = mapManager.mapQuadrants.GetTile(checkXPos, checkYPos);
        //         building.ConnectBuilding(checkTile.building);
        //     }
        // }

        // // 오른쪽
        // {
        //     int xOffset = buildingXSize - buildingXCenter;
        //     for (int yOffset = -buildingYCenter - 1; yOffset < buildingYSize - buildingYCenter + 1; ++yOffset)
        //     {
        //         int checkXPos = xPos + xOffset;
        //         int checkYPos = yPos + yOffset;
        //         var checkTile = mapManager.mapQuadrants.GetTile(checkXPos, checkYPos);
        //         building.ConnectBuilding(checkTile.building);
        //     }
        // }

        // // 위쪽
        // {
        //     int yOffset = -buildingYCenter - 1;
        //     for (int xOffset = -buildingXCenter; xOffset < buildingXSize - buildingXCenter && buildAble; ++xOffset)
        //     {
        //         int checkXPos = xPos + xOffset;
        //         int checkYPos = yPos + yOffset;
        //         var checkTile = mapManager.mapQuadrants.GetTile(checkXPos, checkYPos);
        //         building.ConnectBuilding(checkTile.building);
        //     }
        // }

        // // 아래쪽
        // {
        //     int yOffset = buildingYSize - buildingYCenter;
        //     for (int xOffset = -buildingXCenter; xOffset < buildingXSize - buildingXCenter && buildAble; ++xOffset)
        //     {
        //         int checkXPos = xPos + xOffset;
        //         int checkYPos = yPos + yOffset;
        //         var checkTile = mapManager.mapQuadrants.GetTile(checkXPos, checkYPos);
        //         building.ConnectBuilding(checkTile.building);
        //     }
        // }

    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BuildingRot
{
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3
}

public static class BuildingRotExtention
{
    public static BuildingRot RotCW(this BuildingRot rot)
    {
        return (BuildingRot)(((int)rot + 1) % 4);
    }

    public static BuildingRot RotCCW(this BuildingRot rot)
    {
        return (BuildingRot)(((int)rot + 3) % 4);
    }

    public static Quaternion ToQuaternion(this BuildingRot rot)
    {
        return Quaternion.Euler(0f, 0f, (float)((int)rot * -90));
    }
}

public class MapTile
{
    public FactoryBuilding building;
    public MapTile buildingParentTile;

    public int x;
    public int y;
    public BuildingRot rot;
    public MapTile(FactoryBuilding building, MapTile buildingParentTile, int x, int y, BuildingRot rot)
    {
        this.building = building;
        this.buildingParentTile = buildingParentTile;
        this.x = x;
        this.y = y;
        this.rot = rot;
    }
}


// TODO: 시간복잡도 올리기
// TODO: Dictionary -> 2d segtree 구현
public class Surface
{
    public List<FactoryBuilding> buildings;
    public List<MapTile> allTiles;
    public Dictionary<int, List<MapTile>> xTiles;
    public Dictionary<int, List<MapTile>> yTiles;

    public Surface()
    {
        allTiles = new List<MapTile>();
        buildings = new List<FactoryBuilding>();
        xTiles = new Dictionary<int, List<MapTile>>();
        yTiles = new Dictionary<int, List<MapTile>>();
    }

    public List<MapTile> GetTiles(int minX, int maxX, int minY, int maxY)
    {
        IEnumerable<MapTile> result = Enumerable.Empty<MapTile>();
        for (int x = minX; x <= maxX; ++x)
        {
            List<MapTile> xList = xTiles.GetValueOrDefault(x, null);
            if (xList == null)
            {
                continue;
            }

            for (int y = minY; y <= maxY; ++y)
            {
                List<MapTile> yList = yTiles.GetValueOrDefault(y, null);
                if (yList == null)
                {
                    continue;
                }

                result = result.Concat(xList.Intersect(yList));
            }
        }

        return result.ToList();
    }

    public MapTile GetTile(int x, int y)
    {
        List<MapTile> tiles = GetTiles(x, x, y, y);
        switch (tiles.Count)
        {
            case 0:
                return null;
            case 1:
                return tiles[0];
            default:
                Debug.Assert(false);
                return null;
        }
    }

    public bool IsBuildAble(int minX, int maxX, int minY, int maxY)
    {
        return GetTiles(minX, maxX, minY, maxY).Count == 0;
    }

    public bool IsBuildAble(ResourceType buildingType, int xPos, int yPos)
    {
        var fbm = FactoryBuildingManager.Instance;
        var buildingClass = fbm.GetBuildingClass(buildingType);

        int buildingXSize = buildingClass.xSize;
        int buildingYSize = buildingClass.ySize;
        int buildingXCenter = buildingClass.xCenter;
        int buildingYCenter = buildingClass.yCenter;

        return IsBuildAble(
            xPos - buildingXCenter,
            xPos + buildingXSize - buildingXCenter - 1,
            yPos - buildingYCenter,
            yPos + buildingYSize - buildingYCenter - 1);
    }

    public MapTile MakeMapTile(FactoryBuilding building, MapTile buildingParentTile, int xPos, int yPos, BuildingRot rot)
    {
        MapTile tile = new MapTile(building, buildingParentTile, xPos, yPos, rot);
        Debug.Log($"MakeMapTile: {xPos}, {yPos}");
        allTiles.Add(tile);

        if (xTiles.ContainsKey(xPos))
        {
            xTiles[xPos].Add(tile);
        }
        else
        {
            xTiles[xPos] = new List<MapTile> { tile };
        }

        if (yTiles.ContainsKey(yPos))
        {
            yTiles[yPos].Add(tile);
        }
        else
        {
            yTiles[yPos] = new List<MapTile> { tile };
        }

        return tile;
    }

    public FactoryBuilding AddBuilding(ResourceType buildingType, int xPos, int yPos, BuildingRot rot)
    {
        var fbm = FactoryBuildingManager.Instance;
        var buildingClass = fbm.GetBuildingClass(buildingType);

        int buildingXSize = buildingClass.xSize;
        int buildingYSize = buildingClass.ySize;
        int buildingXCenter = buildingClass.xCenter;
        int buildingYCenter = buildingClass.yCenter;

        MapTile parentTile = MakeMapTile(null, null, xPos, yPos, rot);
        var building = buildingClass.Instantiate(parentTile);
        parentTile.building = building;

        for (int xOffset = -buildingXCenter; xOffset < buildingXSize - buildingXCenter; ++xOffset)
        {
            for (int yOffset = -buildingYCenter; yOffset < buildingYSize - buildingYCenter; ++yOffset)
            {
                if (xOffset == 0 && yOffset == 0)
                {
                    continue;
                }
                int curXPos = xPos + xOffset;
                int curYPos = yPos + yOffset;

                MakeMapTile(building, parentTile, curXPos, curYPos, rot);
            }
        }

        // TODO: 건물 의존성에 따라 buidlings 순서 결정
        buildings.Add(building);

        // TODO: 건물 위치에 따라 logistic system 분리
        // building.prototype.RegisterLogistics(LogisticsSystemManager.Instance.logisticsSystems[0]);

        return building;
    }

    public void DoSurfaceUpdatePhase()
    {
        for (int i = 0; i < buildings.Count; ++i)
        {
            FactoryBuilding building = buildings[i];
            building.DoFactoryBuildingUpdatePhase();
        }
    }
}

public class SurfaceManager : MonoBehaviour
{
    private static SurfaceManager surfaceManager;
    public static SurfaceManager Instance { get { return surfaceManager; } }
    public GameObject TileObject;
    public Transform cameraTransform;
    public static float MapWorldCoordMultiply = 2f;

    private LineRenderer lineRenderer;

    public List<Surface> surfaces;
    public static Surface selectedSurface;

    void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        surfaces = new List<Surface>();
        surfaces.Add(new Surface());
        surfaces.Add(new Surface());
        surfaces.Add(new Surface());
        selectedSurface = surfaces[0];
        surfaceManager = this;
    }

    void DrawGrid()
    {
        int size = 1000;
        for (int x = 0; x < size; ++x)
        {
            Vector3 posUp = new Vector3(x, -size, 0);
            Vector3 posDown = new Vector3(x, size, 0);
            // lineRenderer.SetWidth(10,10);
            // lineRenderer.SetColors(Color.red, Color.red);
            // lineRenderer
            lineRenderer.SetPosition(0, posUp);
            lineRenderer.SetPosition(1, posDown);
        }
    }

    void Start()
    {
        // DrawGrid();
    }


    public static Vector3 RoundWorldPosToGrid(Vector3 worldPos)
    {
        return new Vector3(Mathf.RoundToInt(worldPos.x / MapWorldCoordMultiply) * MapWorldCoordMultiply, Mathf.RoundToInt(worldPos.y / MapWorldCoordMultiply) * MapWorldCoordMultiply, worldPos.z);
    }

    // public static (int, int) RoundWorldPosAndToMapPos(Vector3 worldPos)
    // {
    //     return ((int)Mathf.RoundToInt(worldPos.x / MapWorldCoordMultiply), (int)Mathf.RoundToInt(worldPos.y / MapWorldCoordMultiply));
    // }

    public static (int, int) WorldPosToMapPos(float worldXPos, float worldYPos)
    {
        return ((int)(worldXPos / MapWorldCoordMultiply), (int)(worldYPos / MapWorldCoordMultiply));
    }

    public static (float, float) MapPosToWorldPos(int mapXPos, int mapYPos)
    {
        return (mapXPos * MapWorldCoordMultiply, mapYPos * MapWorldCoordMultiply);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoAllSurfacesUpdatePhase()
    {
        // TODO: Surface 의존성에 따라 update
        surfaces.ForEach((surface) =>
        {
            surface.DoSurfaceUpdatePhase();
        });
    }
}

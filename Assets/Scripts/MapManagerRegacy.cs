// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MapTile
// {
//     public FactoryBuilding building;
//     public MapTile buildingParentTile;
//     public bool buildAble;
// }

// public class MapChunk
// {
//     public const int xChunkSize = 8;
//     public const int yChunkSize = 6;
    
//     public int globalXStart;
//     public int globalYStart;
//     public int globalXEnd;
//     public int globalYEnd;

//     private MapTile[,] tiles;
//     public MapChunk()
//     {
//     }

//     public MapTile GetTile(int xIdx, int yIdx)
//     {
//         return tiles[yIdx % yChunkSize, xIdx % xChunkSize];
//     }

//     public static MapChunk MakeNewChunk()
//     {
//         MapChunk chunk = new MapChunk();
//         chunk.tiles = new MapTile[yChunkSize, xChunkSize];
//         for (int y = 0; y < yChunkSize; ++y)
//         {
//             for (int x = 0; x < xChunkSize; ++x)
//             {
//                 var tile = new MapTile();
//                 tile.buildAble = true;
//                 chunk.tiles[y, x] = tile;
//             }
//         }
//         return chunk;
//     }
// }

// public class MapQuadrants
// {
//     List<List<MapChunk>>[] quadrants;

//     public MapQuadrants()
//     {
//         quadrants = new List<List<MapChunk>>[4];
//         for (int i = 0; i < 4; ++i)
//         {
//             quadrants[i] = new List<List<MapChunk>>();
//         }
//     }
//     public int GetQuadrant(int x, int y)
//     {
//         if (x >= 0 && y >= 0)
//         {
//             return 0;
//         }
//         if (x < 0 && y >= 0)
//         {
//             return 1;
//         }
//         if (x < 0 && y < 0)
//         {
//             return 2;
//         }
//         return 3;
//     }

//     public static (int, int) GetMapIdx(int xPos, int yPos)
//     {
//         int xIdx = xPos < 0 ? -xPos - 1 : xPos;
//         int yIdx = yPos < 0 ? -yPos - 1 : yPos;
//         return (xIdx, yIdx);
//     }

//     public MapChunk GetChunk(int quadrantIdx, int xIdx, int yIdx)
//     {
//         int xChunkIdx = xIdx / MapChunk.xChunkSize;
//         int yChunkIdx = yIdx / MapChunk.yChunkSize;
//         var quadrant = quadrants[quadrantIdx];
        
//         while (yChunkIdx >= quadrant.Count)
//         {
//             quadrant.Add(new List<MapChunk>());
//         }

//         var yChunk = quadrant[yChunkIdx];

//         while (xChunkIdx >= yChunk.Count)
//         {
//             var newMapChunk = MapChunk.MakeNewChunk();
//             var newChunkXIdx = yChunk.Count;
//             switch (quadrantIdx)
//             {
//                 case 0:
//                     newMapChunk.globalXStart = newChunkXIdx * MapChunk.xChunkSize;
//                     newMapChunk.globalXEnd = (newChunkXIdx + 1) * MapChunk.xChunkSize;
//                     newMapChunk.globalYStart = yChunkIdx * MapChunk.yChunkSize;
//                     newMapChunk.globalYEnd = (yChunkIdx + 1) * MapChunk.yChunkSize;
//                     break;
//                 case 1:
//                     newMapChunk.globalXStart = -newChunkXIdx * MapChunk.xChunkSize - 1;
//                     newMapChunk.globalXEnd = -(newChunkXIdx + 1) * MapChunk.xChunkSize - 1;
//                     newMapChunk.globalYStart = yChunkIdx * MapChunk.yChunkSize;
//                     newMapChunk.globalYEnd = (yChunkIdx + 1) * MapChunk.yChunkSize;
//                     break;
//                 case 2:
//                     newMapChunk.globalXStart = -newChunkXIdx * MapChunk.xChunkSize - 1;
//                     newMapChunk.globalXEnd = -(newChunkXIdx + 1) * MapChunk.xChunkSize - 1;
//                     newMapChunk.globalYStart = -yChunkIdx * MapChunk.yChunkSize - 1;
//                     newMapChunk.globalYEnd = -(yChunkIdx + 1) * MapChunk.yChunkSize - 1;
//                     break;
//                 case 3:
//                     newMapChunk.globalXStart = newChunkXIdx * MapChunk.xChunkSize;
//                     newMapChunk.globalXEnd = (newChunkXIdx + 1) * MapChunk.xChunkSize;
//                     newMapChunk.globalYStart = -yChunkIdx * MapChunk.yChunkSize - 1;
//                     newMapChunk.globalYEnd = -(yChunkIdx + 1) * MapChunk.yChunkSize - 1;
//                     break;
//             }
            
//             Debug.Log($"New Map Chunk({quadrantIdx}, {xIdx}({newChunkXIdx}), {yIdx}({yChunkIdx})) -> ({newMapChunk.globalXStart}, {newMapChunk.globalYStart}, {newMapChunk.globalXEnd}, {newMapChunk.globalYEnd})");
//             yChunk.Add(newMapChunk);
//         }

//         return yChunk[xChunkIdx];
//     }

//     public MapChunk GetChunk(int xPos, int yPos)
//     {
//         (int xIdx, int yIdx) = GetMapIdx(xPos, yPos);

//         return GetChunk(GetQuadrant(xPos, yPos), xIdx, yIdx);
//     }

//     public List<MapChunk> GetChunks(int xPos, int yPos, int chunkLoadFarY, int chunkLoadFarX)
//     {
//         List<MapChunk> chunks = new List<MapChunk>();
//         for (int yIter = -chunkLoadFarY; yIter <= chunkLoadFarY; yIter++)
//         {
//             for (int xIter = -chunkLoadFarX; xIter <= chunkLoadFarX; xIter++)
//             {
//                 int newX = xPos + xIter * MapChunk.xChunkSize;
//                 int newY = yPos + yIter * MapChunk.yChunkSize;

//                 var newChunk = GetChunk(newX, newY);
//                 chunks.Add(newChunk);
//             }
//         }
//         return chunks;
//     }

//     public MapTile GetTile(int xPos, int yPos)
//     {
//         (int xIdx, int yIdx) = GetMapIdx(xPos, yPos);
//         int quadrantIdx = GetQuadrant(xPos, yPos);

//         var chunk = GetChunk(quadrantIdx, xIdx, yIdx);
//         return chunk.GetTile(xIdx, yIdx);
//     }
// }

// public class MapManager : MonoBehaviour
// {
//     public int chunkLoadFarX = 8;
//     public int chunkLoadFarY = 6;
//     public MapQuadrants mapQuadrants;
//     public List<MapChunk> loadedChunks;
//     public List<GameObject[]> chunkPool;
//     public MapChunk lastChunk;
//     public GameObject TileObject;
//     public Transform cameraTransform;
//     public static float MapWorldCoordMultiply = 2f;
    
//     // Start is called before the first frame update

//     void Awake() {

//         mapQuadrants = new MapQuadrants();
//         loadedChunks = new List<MapChunk>();
//         chunkPool = new List<GameObject[]>();
//         lastChunk = null;
//     }

//     void Start()
//     {
//         ChunkLoad(0, 0);
//         StartCoroutine("ChunkLoader");
//     }

//     public static Vector3 RoundWorldPosToGrid(Vector3 worldPos)
//     {
//         return new Vector3(Mathf.RoundToInt(worldPos.x / MapWorldCoordMultiply) * MapWorldCoordMultiply, Mathf.RoundToInt(worldPos.y / MapWorldCoordMultiply) * MapWorldCoordMultiply, worldPos.z);
//     }

//     public static (int, int) WorldPosToMapPos(float worldXPos, float worldYPos)
//     {
//         return ((int)(worldXPos / MapWorldCoordMultiply), (int)(worldYPos / MapWorldCoordMultiply));
//     }

//     IEnumerator ChunkLoader()
//     {
//         while (true)
//         {
//             ChunkLoad(cameraTransform.position.x, cameraTransform.position.y);
//             yield return new WaitForSeconds(0.1f);
//         }
//     }

//     bool ChunkLoad(float xWorldPos, float yWorldPos)
//     {
//         int x = (int)(xWorldPos / MapWorldCoordMultiply);
//         int y = (int)(yWorldPos / MapWorldCoordMultiply);

//         var chunk = mapQuadrants.GetChunk(x, y);
//         if (chunk == lastChunk)
//         {
//             return false;
//         }
//         lastChunk = chunk;

//         var neededChunks = mapQuadrants.GetChunks(x, y, chunkLoadFarY, chunkLoadFarX);

//         List<MapChunk> newChunks = new List<MapChunk>();
//         List<MapChunk> deleteChunks = new List<MapChunk>();
//         foreach (var targetChunk in neededChunks)
//         {
//             if (!loadedChunks.Contains(targetChunk))
//             {
//                 newChunks.Add(targetChunk);
//                 loadedChunks.Add(targetChunk);
//             }
//         }

//         // TODO: 제거 로직
        
//         foreach (var newChunk in newChunks)
//         {
//             InstantiateChunk(newChunk);
//         }

//         return true;
//     }

//     void InstantiateChunk(MapChunk chunk)
//     {
//         Debug.Log($"InstantiateChunk({chunk.globalXStart}, {chunk.globalYStart}, {chunk.globalXEnd}, {chunk.globalYEnd})");
//         int xMov = chunk.globalXStart < chunk.globalXEnd ? 1 : -1;
//         int yMov = chunk.globalYStart < chunk.globalYEnd ? 1 : -1;

//         for (int yIdx = 0; yIdx < MapChunk.yChunkSize; ++yIdx)
//         {
//             for (int xIdx = 0; xIdx < MapChunk.xChunkSize; ++xIdx)
//             {
//                 Instantiate(TileObject, new Vector3((chunk.globalXStart + xMov * xIdx) * MapWorldCoordMultiply, (chunk.globalYStart + yMov * yIdx) * MapWorldCoordMultiply, 0), Quaternion.identity, transform);
//             }
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }

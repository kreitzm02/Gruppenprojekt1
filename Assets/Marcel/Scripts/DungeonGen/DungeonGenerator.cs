using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

//TODO
public class DungeonGenerator : MonoBehaviour
{
    [HideInInspector] public static DungeonGenerator Instance;
    [HideInInspector] public bool generationFinished;

    [Header("General Settings")]
    [SerializeField] private int gridLength;
    [SerializeField] private int gridWidth;
    [SerializeField, Range(1, 16)] private int unitSize;
    [SerializeField] private GameObject voidObject;
    [SerializeField] private GameObject player;

    [Header("Room Settings")]
    [SerializeField] private int roomAmount;
    [SerializeField, Range(2, 10)] private int minRoomSize;
    [SerializeField, Range(2, 10)] private int maxRoomSize;
    [SerializeField] private bool allowOverlapingRooms;

    [Header("Enemy Settings")]
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private GameObject bossEnemy;
    [SerializeField, Range(0, 5)] private int minEnemiesPerRoom;
    [SerializeField, Range(0, 5)] private int maxEnemiesPerRoom;

    [Header("Wall Settings")]
    [SerializeField] private float wallOffset;
    [SerializeField] private GameObject wallCorner;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private List<GameObject> wallObjects;

    [Header("Floor Settings")]
    [SerializeField, Range(0, 100)] private int variationRate;
    [SerializeField] private List<GameObject> floorObjects;

    [Header("Decoration Settings")]
    [SerializeField, Range(0, 25)] private float decAngleOffset;
    [SerializeField, Range(0, 1)] private float decPosOffset;
    [SerializeField] private List<GameObject> decorationObjects;
    [SerializeField] private GameObject torch;
    [SerializeField, Range(0, 100)] int torchSpawnChance;

    private DungeonData dungeonData;

    public int progressInPercent = 0;

    private IFloorBuilder floorBuilder;
    private IWallBuilder wallBuilder;
    private IDoorBuilder doorBuilder;
    private IDecorationBuilder decorationBuilder;
    private ITorchBuilder torchBuilder;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        dungeonData = new DungeonData();
        floorBuilder = new FloorBuilder(unitSize, variationRate, floorObjects, voidObject);
        wallBuilder = new WallBuilder(unitSize, wallOffset, wallCorner, wallObjects);
        doorBuilder = new DoorBuilder(unitSize, wallOffset, doorObject);
        decorationBuilder = new DecorationBuilder(unitSize, decAngleOffset, decPosOffset, decorationObjects);
        torchBuilder = new TorchBuilder(unitSize, torch, torchSpawnChance);
    }

    private void Start()
    {
        
    }


    private IEnumerator GenerateDungeonCoroutine()
    {
        dungeonData.InitializeData(gridLength, gridWidth);

        GenerateRooms();
        progressInPercent = 20;
        yield return new WaitForSeconds(0.2f);

        GenerateCorridors();
        progressInPercent = 40;
        yield return new WaitForSeconds(0.2f);

        dungeonData.AdjacencyList = DungeonGenHelper.BuildAdjacencyList(dungeonData.allRoomCenters, dungeonData.MST);
        dungeonData.GetStartAndEndRoom(DungeonGenHelper.DetermineDungeonDiameter(dungeonData.allRoomCenters, dungeonData.AdjacencyList));
        dungeonData.GetDeadEndRooms();
        DungeonLevelManager.Instance.allRooms = dungeonData.allRooms;
        progressInPercent = 50;
        yield return new WaitForSeconds(0.2f);

        floorBuilder.BuildFloor(dungeonData);
        progressInPercent = 60;
        yield return new WaitForSeconds(0.2f);

        wallBuilder.BuildWalls(dungeonData);
        progressInPercent = 70;
        yield return new WaitForSeconds(0.2f);

        doorBuilder.BuildDoors(dungeonData);
        progressInPercent = 80;
        yield return new WaitForSeconds(0.2f);

        decorationBuilder.BuildDecorations(dungeonData);
        progressInPercent = 90;
        yield return new WaitForSeconds(0.2f);

        PlaceEnemies();
        progressInPercent = 100;

        PlacePlayer();
        yield return new WaitForSeconds(0.5f);
    }

    public void GenerateDungeon()
    {
        StartCoroutine(GenerateDungeonCoroutine());
    }

    private void GenerateRooms()
    {
        for (int i = 0; i < roomAmount; i++)
        {
            Vector2Int newRoomSize = DungeonGenHelper.GetRandomRoomSize(minRoomSize, maxRoomSize);
            Vector2Int newRoomOrigin = DungeonGenHelper.GetRandomRoomOrigin(newRoomSize, gridLength, gridWidth);
            DungeonRoom newRoom = new(newRoomSize.x, newRoomSize.y, newRoomOrigin);
            bool overlapsWithRoom = false;
            foreach (var room in dungeonData.allRooms)
            {
                overlapsWithRoom = room.OverlapsWith(newRoom);

                if (overlapsWithRoom) break;
            }
            if (overlapsWithRoom && !allowOverlapingRooms) continue;
            newRoom.SetRoomType();
            dungeonData.allRooms.Add(newRoom);
            dungeonData.allRoomCenters.Add(newRoom.GetRoomCenter());
            List<Vector2Int> affectedCells = DungeonGridBuilder.CollectAffectedCells(newRoomOrigin, newRoomSize);
            dungeonData.dungeonGrid = DungeonGridBuilder.ChangeCellsInGrid(affectedCells, CellType.Floor, dungeonData.dungeonGrid);
        }
    }

    private void GenerateCorridors()
    {
        List<(Vector2Int, Vector2Int, float)> connections = new List<(Vector2Int, Vector2Int, float)>();

        for (int i = 0; i < dungeonData.allRoomCenters.Count; i++)
        {
            for (int j = i + 1; j < dungeonData.allRoomCenters.Count; j++)
            {
                float distance = Vector2Int.Distance(dungeonData.allRoomCenters[i], dungeonData.allRoomCenters[j]);
                connections.Add((dungeonData.allRoomCenters[i], dungeonData.allRoomCenters[j], distance));
            }
        }

        connections.Sort((a, b) => a.Item3.CompareTo(b.Item3));

        dungeonData.MST = DungeonGenHelper.CreateMinimumSpanningTree(dungeonData.allRoomCenters, connections);

        List<Vector2Int> affectedCells = new();

        foreach (var corridor in dungeonData.MST)
        {
            for (int x = Mathf.Min(corridor.Item1.x, corridor.Item2.x); x <= Mathf.Max(corridor.Item1.x, corridor.Item2.x); x++)
            {
                affectedCells.Add(new Vector2Int(x, corridor.Item1.y));
            }
            for (int y = Mathf.Min(corridor.Item1.y, corridor.Item2.y); y <= Mathf.Max(corridor.Item1.y, corridor.Item2.y); y++)
            {
                affectedCells.Add(new Vector2Int(corridor.Item2.x, y));
            }
        }
        DungeonGridBuilder.ChangeCellsInGrid(affectedCells, CellType.Floor, dungeonData.dungeonGrid);
    }

    private void PlacePlayer()
    {
        player.transform.position = new Vector3(dungeonData.startRoom.GetRoomCenter().x * unitSize, 0, dungeonData.startRoom.GetRoomCenter().y * unitSize);
    }

    private void PlaceEnemies()
    {
        foreach (var room in dungeonData.allRooms)
        {
            if (room.type == RoomType.Start || room.type == RoomType.End || room.type == RoomType.Loot) continue;
            int numEnemies = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);

            for (int i = 0; i < numEnemies; i++)
            {
                room.enemyCount++;
                float offsetX = Random.Range(-1.5f, 1.5f);
                float offsetZ = Random.Range(-1.5f, 1.5f);

                Vector3 spawnPosition = new Vector3((room.GetRoomCenter().x + offsetX) * unitSize, 0, (room.GetRoomCenter().y + offsetZ) * unitSize);

                Instantiate(enemies.ElementAt(Random.Range(0, enemies.Count)), spawnPosition, Quaternion.identity);
            }
        }
    }
}

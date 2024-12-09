using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator_M : MonoBehaviour
{
    private int[,] dungeonGrid;
    private List<Vector2Int> roomCenters;
    private List<(int x, int y, int length, int width)> rooms = new List<(int, int, int, int)>();
    [SerializeField, Range(25, 250)] private int gridLength;
    [SerializeField, Range(25, 250)] private int gridWidth;
    [SerializeField, Range(1, 75)] private int roomAmount;
    [SerializeField, Range(2, 7)] private int minRoomSize;
    [SerializeField, Range(7, 40)] private int maxRoomSize;
    [SerializeField, Range(1, 16)] private int unitSize;
    [SerializeField, Range(0, 100)] private int variationRate;
    [SerializeField] private List<GameObject> floorObjects;
    [SerializeField] private List<GameObject> wallObjects;
    [SerializeField] private List<GameObject> decorationObjects;
    [SerializeField] private GameObject wallCorner;
    [SerializeField] private GameObject voidObject;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private bool allowOverlapingRooms;
    [SerializeField] private float wallOffset;
    private void Start()
    {
        GenerateRooms();
        ConnectRooms();
        BuildFloor();
        BuildWalls();
        BuildDoors();
        //BuildDecoration();
    }
    private void GenerateRooms()
    {
        roomCenters = new List<Vector2Int>();
        dungeonGrid = new int[gridLength, gridWidth];
        for (int i = 0; i < roomAmount; i++)
        {
            int roomLength = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomWidth = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomX = Random.Range(1, gridLength - roomLength - 1);
            int roomY = Random.Range(1, gridWidth - roomWidth - 1);
            Vector3 roomOrigin = new Vector3(roomX, 0, roomY);
            bool overlaps = false;
            foreach (var room in rooms)
            {
                if (IsOverlapping(roomX, roomY, roomLength, roomWidth, room.x, room.y, room.length, room.width))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps || allowOverlapingRooms)
            {
                rooms.Add((roomX, roomY, roomLength, roomWidth));
                for (int j = (int)roomOrigin.x; j < (int)roomOrigin.x + roomLength; j++)
                {
                    for (int k = (int)roomOrigin.z; k < (int)roomOrigin.z + roomWidth; k++)
                    {
                        dungeonGrid[j, k] = 1;
                    }
                }
                int centerX = roomX + roomLength / 2;
                int centerY = roomY + roomWidth / 2;
                roomCenters.Add(new Vector2Int(centerX, centerY));
            }
        }
    }

    private bool IsOverlapping(int _x1, int _y1, int _length1, int _width1, int _x2, int _y2, int _length2, int _width2)
    {
        return !(_x1 + _length1 < _x2 || _x2 + _length2 < _x1 || _y1 + _width1 < _y2 || _y2 + _width2 < _y1);
    }

    private void ConnectRooms()
    {
        List<(Vector2Int, Vector2Int, float)> connections = new List<(Vector2Int, Vector2Int, float)>();

        for (int i = 0; i < roomCenters.Count; i++)
        {
            for (int j = i + 1; j < roomCenters.Count; j++)
            {
                float distance = Vector2Int.Distance(roomCenters[i], roomCenters[j]);
                connections.Add((roomCenters[i], roomCenters[j], distance));
            }
        }

        connections.Sort((a, b) => a.Item3.CompareTo(b.Item3));

        //Minimum Spanning Tree (MST) Kruskal Algorithmus

        List<(Vector2Int, Vector2Int)> mst = new List<(Vector2Int, Vector2Int)>();
        Dictionary<Vector2Int, int> roomGroups = new Dictionary<Vector2Int, int>();
        int groupCounter = 0;

        foreach (var center in roomCenters)
        {
            roomGroups[center] = groupCounter++;
        }

        foreach (var connection in connections)
        {
            var roomA = connection.Item1;
            var roomB = connection.Item2;
            if (roomGroups[roomA] != roomGroups[roomB])
            {
                mst.Add((roomA, roomB));

                int oldGroup = roomGroups[roomB];
                int newGroup = roomGroups[roomA];

                foreach (var key in roomGroups.Keys.ToList())
                {
                    if (roomGroups[key] == oldGroup)
                    {
                        roomGroups[key] = newGroup;
                    }
                }
            }
        }

        // Gänge generieren
        foreach (var corridor in mst)
        {
            CreateCorridor(corridor.Item1, corridor.Item2);
        }
    }

    private void CreateCorridor(Vector2Int _from, Vector2Int _to)
    {
        // Horizontale Verbindung
        for (int x = Mathf.Min(_from.x, _to.x); x <= Mathf.Max(_from.x, _to.x); x++)
        {
            dungeonGrid[x, _from.y] = 1;
        }

        // Vertikale Verbindung
        for (int y = Mathf.Min(_from.y, _to.y); y <= Mathf.Max(_from.y, _to.y); y++)
        {
            dungeonGrid[_to.x, y] = 1;
        }
    }

    private void PrintDungeonDebug()
    {
        // Displaying the generated grid in the Debug Log

        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            string row = "";
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                row += dungeonGrid[i, j];
            }
            Debug.Log(row);
        }
    }

    private void BuildWalls()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                if (dungeonGrid[i, j] == 1)
                {
                    int random = Random.Range(0, wallObjects.Count);
                    try
                    {
                        // walls
                        if (dungeonGrid[i + 1, j] == 0)
                        {
                            Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, j * unitSize);
                            Quaternion rotation = Quaternion.Euler(0, -90, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }
                        if (dungeonGrid[i - 1, j] == 0)
                        {
                            Vector3 position = new Vector3(((i - 1) * unitSize) + wallOffset, 0, j * unitSize);
                            Quaternion rotation = Quaternion.Euler(0, 90, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }
                        if (dungeonGrid[i, j + 1] == 0)
                        {
                            Vector3 position = new Vector3(i * unitSize, 0, ((j + 1) * unitSize) - wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 180, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }
                        if (dungeonGrid[i, j - 1] == 0)
                        {
                            Vector3 position = new Vector3(i * unitSize, 0, ((j - 1) * unitSize) + wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 0, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }

                        // corner walls
                        if (dungeonGrid[i + 1, j - 1] == 0 && dungeonGrid[i + 1, j] == 0 && dungeonGrid[i, j - 1] == 0)
                        {
                            Vector3 position = new Vector3(((i + 1)* unitSize) - wallOffset, 0, ((j - 1) * unitSize) + wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, -90, 0);
                            Instantiate(wallCorner, position, rotation);
                        }
                        if (dungeonGrid[i + 1, j + 1] == 0 && dungeonGrid[i + 1, j] == 0 && dungeonGrid[i, j + 1] == 0)
                        {
                            Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, ((j + 1) * unitSize) - wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 180, 0);
                            Instantiate(wallCorner, position, rotation);
                        }
                        if (dungeonGrid[i - 1, j + 1] == 0 && dungeonGrid[i - 1, j] == 0 && dungeonGrid[i, j + 1] == 0)
                        {
                            Vector3 position = new Vector3(((i - 1) * unitSize) + wallOffset, 0, ((j + 1) * unitSize) - wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 90, 0);
                            Instantiate(wallCorner, position, rotation);
                        }
                        if (dungeonGrid[i - 1, j - 1] == 0 && dungeonGrid[i - 1, j] == 0 && dungeonGrid[i, j - 1] == 0)
                        {
                            Vector3 position = new Vector3(((i - 1) * unitSize) + wallOffset, 0, ((j - 1) * unitSize) + wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 0, 0);
                            Instantiate(wallCorner, position, rotation);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
    }

    private void BuildDecoration()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                try
                {
                    int random = Random.Range(1, 101);
                    if (random > 11 || dungeonGrid[i, j] != 1)
                        continue;
                    random = Random.Range(0, decorationObjects.Count);
                    if (dungeonGrid[i - 1, j] == 0 && dungeonGrid[i - 1, j - 1] == 0 && dungeonGrid[i - 1, j + 1] == 0)
                    {
                        Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Instantiate(decorationObjects.ElementAt(random), position, rotation);
                    }
                    if (dungeonGrid[i + 1, j] == 0 && dungeonGrid[i + 1, j - 1] == 0 && dungeonGrid[i + 1, j + 1] == 0)
                    {
                        Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Instantiate(decorationObjects.ElementAt(random), position, rotation);
                    }
                    if (dungeonGrid[i, j - 1] == 0 && dungeonGrid[i + 1, j - 1] == 0 && dungeonGrid[i - 1, j - 1] == 0)
                    {
                        Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Instantiate(decorationObjects.ElementAt(random), position, rotation);
                    }
                    if (dungeonGrid[i, j + 1] == 0 && dungeonGrid[i + 1, j + 1] == 0 && dungeonGrid[i - 1, j + 1] == 0)
                    {
                        Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Instantiate(decorationObjects.ElementAt(random), position, rotation);
                    }
                }
                catch
                {

                }
            }
        }
    }

    private void BuildDoors()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                try
                {
                    if (dungeonGrid[i + 1, j] == 1 && dungeonGrid[i + 1, j - 1] == 0 && dungeonGrid[i + 1, j + 1] == 0 && (dungeonGrid[i, j - 1] == 1 || dungeonGrid[i, j + 1] == 1) && dungeonGrid[i - 1, j] == 1)
                    {
                        Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset , 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 90, 0);
                        Instantiate(doorObject, position, rotation);
                    }
                    if (dungeonGrid[i - 1, j] == 1 && dungeonGrid[i - 1, j - 1] == 0 && dungeonGrid[i - 1, j + 1] == 0 && (dungeonGrid[i, j - 1] == 1 || dungeonGrid[i, j + 1] == 1) && dungeonGrid[i + 1, j] == 1)
                    {
                        Vector3 position = new Vector3(((i - 1) * unitSize) + wallOffset, 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 90, 0);
                        Instantiate(doorObject, position, rotation);
                    }
                    if (dungeonGrid[i, j + 1] == 1 && dungeonGrid[i + 1, j + 1] == 0 && dungeonGrid[i - 1, j + 1] == 0 && (dungeonGrid[i - 1, j] == 1 || dungeonGrid[i + 1, j] == 1) && dungeonGrid[i, j - 1] == 1)
                    {
                        Vector3 position = new Vector3(i * unitSize, 0, ((j + 1) * unitSize) - wallOffset);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Instantiate(doorObject, position, rotation);
                    }
                    if (dungeonGrid[i, j - 1] == 1 && dungeonGrid[i - 1, j - 1] == 0 && dungeonGrid[i + 1, j - 1] == 0 && (dungeonGrid[i - 1, j] == 1 || dungeonGrid[i + 1, j] == 1) && dungeonGrid[i, j + 1] == 1)
                    {
                        Vector3 position = new Vector3(i * unitSize, 0, ((j - 1) * unitSize) + wallOffset);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Instantiate(doorObject, position, rotation);
                    }
                }
                catch
                {

                }
            }
        }
    }

    private void BuildFloor()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                Vector3 position = new Vector3(i * unitSize, 0, j * unitSize);
                switch (dungeonGrid[i, j])
                {
                    case 0: // black plane will be placed to create a "void".
                        position.y += unitSize * 0.85f;
                        Instantiate(voidObject, position, Quaternion.identity);
                        break;
                    case 1:
                        int random = Random.Range(0, 101);
                        if (random > variationRate)
                        {
                            Instantiate(floorObjects.ElementAt(0), position, Quaternion.identity);
                        }
                        else
                        {
                            int randomTile = Random.Range(1, floorObjects.Count);
                            Instantiate(floorObjects.ElementAt(randomTile), new Vector3(position.x - 1f, 0, position.z + 1f), Quaternion.identity);
                            randomTile = Random.Range(1, floorObjects.Count);
                            Instantiate(floorObjects.ElementAt(randomTile), new Vector3(position.x - 1f, 0, position.z - 1f), Quaternion.identity);
                            randomTile = Random.Range(1, floorObjects.Count);
                            Instantiate(floorObjects.ElementAt(randomTile), new Vector3(position.x + 1f, 0, position.z + 1f), Quaternion.identity);
                            randomTile = Random.Range(1, floorObjects.Count);
                            Instantiate(floorObjects.ElementAt(randomTile), new Vector3(position.x + 1f, 0, position.z - 1f), Quaternion.identity);
                        }
                        break;
                    default:
                        goto case 0;
                }
            }
        }
    }
}

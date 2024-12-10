using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator_M : MonoBehaviour
{
    private int[,] dungeonGrid;
    private List<Vector2Int> roomCenters;
    private List<(int x, int y, int length, int width)> rooms = new List<(int, int, int, int)>();
    [Header("General Settings")]
    [SerializeField, Range(25, 250)] private int gridLength;
    [SerializeField, Range(25, 250)] private int gridWidth;
    [SerializeField, Range(1, 16)] private int unitSize;
    [SerializeField] private GameObject voidObject;
    [Header("Room Settings")]
    [SerializeField, Range(1, 75)] private int roomAmount;
    [SerializeField, Range(2, 7)] private int minRoomSize;
    [SerializeField, Range(7, 40)] private int maxRoomSize;
    [SerializeField] private bool allowOverlapingRooms;
    [Header("Wall Settings")]
    [SerializeField] private float wallOffset;
    [SerializeField] private GameObject wallCorner;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private List<GameObject> wallObjects;
    [Header("Floor Settings")]
    [SerializeField, Range(0, 100)] private int variationRate;
    [SerializeField] private List<GameObject> floorObjects;
    [Header("Decoration Settings")]
    [SerializeField, Range(0 ,25)] private float decAngleOffset;
    [SerializeField, Range(0, 1)] private float decPosOffset;
    [SerializeField] private List<GameObject> decorationObjects;

    // Neue Felder für Start-/End-Raum Berechnung
    private Dictionary<Vector2Int, List<Vector2Int>> adjacencyList;
    private List<(Vector2Int, Vector2Int)> mst = new List<(Vector2Int, Vector2Int)>();

    private Vector2Int startPoint;
    private Vector2Int endPoint;


    private void Start()
    {
        GenerateRooms();
        ConnectRooms();

        // MST ist nun berechnet. Wir ermitteln die zwei am weitesten entfernten Räume im MST.
        BuildAdjacencyList();     // Baut den Graphen auf Basis des MST
        DetermineDungeonDiameter(); // Ermittelt startPoint und endPoint

        BuildFloor();
        BuildWalls();
        BuildDoors();
        BuildDecoration();
        //PrintDungeonDebug();
        foreach (var room in roomCenters)
        {
            Debug.Log($"{room.x}, 0, {room.y}");
        }
    }
    private void GenerateRooms()
    {
        roomCenters = new List<Vector2Int>();
        dungeonGrid = new int[gridLength, gridWidth]; // 0 = empty | 1 = floor | 2 = wall | 3 = corner wall | 9 = wall with shelves
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

        //List<(Vector2Int, Vector2Int)> mst = new List<(Vector2Int, Vector2Int)>();
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

    private void BuildAdjacencyList()
    {
        adjacencyList = new Dictionary<Vector2Int, List<Vector2Int>>();
        foreach (var center in roomCenters)
        {
            adjacencyList[center] = new List<Vector2Int>();
        }

        foreach (var edge in mst)
        {
            adjacencyList[edge.Item1].Add(edge.Item2);
            adjacencyList[edge.Item2].Add(edge.Item1);
        }
    }

    private void DetermineDungeonDiameter()
    {
        // BFS von einem beliebigen Knoten (Raum) - z.B. den ersten Raum:
        Vector2Int arbitraryNode = roomCenters[0];
        Vector2Int nodeA = BFSFindFarthestNode(arbitraryNode);

        // BFS von nodeA, um nodeB zu finden (am weitesten entfernt von nodeA)
        Vector2Int nodeB = BFSFindFarthestNode(nodeA);

        startPoint = nodeA;
        endPoint = nodeB;
    }

    private Vector2Int BFSFindFarthestNode(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();

        foreach (var center in roomCenters)
            distances[center] = -1;

        distances[start] = 0;
        queue.Enqueue(start);

        Vector2Int farthestNode = start;
        int maxDistance = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDist = distances[current];

            if (currentDist > maxDistance)
            {
                maxDistance = currentDist;
                farthestNode = current;
            }

            foreach (var neighbor in adjacencyList[current])
            {
                if (distances[neighbor] == -1)
                {
                    distances[neighbor] = currentDist + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return farthestNode;
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
                        if (dungeonGrid[i + 1, j] != 1)
                        {
                            if (random != 10 && random != 6) // 6 and 10 will be walls that can´t have additional decorations on it.
                                dungeonGrid[i + 1, j] = 2;
                            else
                                dungeonGrid[i + 1, j] = 9;
                            Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, j * unitSize);
                            Quaternion rotation = Quaternion.Euler(0, -90, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }
                        if (dungeonGrid[i - 1, j] != 1)
                        {
                            if (random != 10 && random != 6)
                                dungeonGrid[i - 1, j] = 2;
                            else
                                dungeonGrid[i - 1, j] = 9;
                            Vector3 position = new Vector3(((i - 1) * unitSize) + wallOffset, 0, j * unitSize);
                            Quaternion rotation = Quaternion.Euler(0, 90, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }
                        if (dungeonGrid[i, j + 1] != 1)
                        {
                            if (random != 10 && random != 6)
                                dungeonGrid[i, j + 1] = 2;
                            else
                                dungeonGrid[i, j + 1] = 9;
                            Vector3 position = new Vector3(i * unitSize, 0, ((j + 1) * unitSize) - wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 180, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }
                        if (dungeonGrid[i, j - 1] != 1)
                        {
                            if (random != 10 && random != 6)
                                dungeonGrid[i, j - 1] = 2;
                            else
                                dungeonGrid[i, j - 1] = 9;
                            Vector3 position = new Vector3(i * unitSize, 0, ((j - 1) * unitSize) + wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 0, 0);
                            Instantiate(wallObjects.ElementAt(random), position, rotation);
                        }

                        // corner walls
                        if (dungeonGrid[i + 1, j - 1] != 1 && dungeonGrid[i + 1, j] != 1 && dungeonGrid[i, j - 1] != 1)
                        {
                            dungeonGrid[i + 1, j - 1] = 3;
                            Vector3 position = new Vector3(((i + 1)* unitSize) - wallOffset, 0, ((j - 1) * unitSize) + wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, -90, 0);
                            Instantiate(wallCorner, position, rotation);
                        }
                        if (dungeonGrid[i + 1, j + 1] != 1 && dungeonGrid[i + 1, j] != 1 && dungeonGrid[i, j + 1] != 1)
                        {
                            dungeonGrid[i + 1, j + 1] = 3;
                            Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset, 0, ((j + 1) * unitSize) - wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 180, 0);
                            Instantiate(wallCorner, position, rotation);
                        }
                        if (dungeonGrid[i - 1, j + 1] != 1 && dungeonGrid[i - 1, j] != 1 && dungeonGrid[i, j + 1] != 1)
                        {
                            dungeonGrid[i - 1, j + 1] = 3;
                            Vector3 position = new Vector3(((i - 1) * unitSize) + wallOffset, 0, ((j + 1) * unitSize) - wallOffset);
                            Quaternion rotation = Quaternion.Euler(0, 90, 0);
                            Instantiate(wallCorner, position, rotation);
                        }
                        if (dungeonGrid[i - 1, j - 1] != 1 && dungeonGrid[i - 1, j] != 1 && dungeonGrid[i, j - 1] != 1)
                        {
                            dungeonGrid[i - 1, j - 1] = 3;
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
        int avgRoomSize = (minRoomSize + maxRoomSize) / 2;

        // testing start and end rooms
        Instantiate(doorObject, new Vector3(startPoint.x * unitSize, 0, startPoint.y * unitSize), Quaternion.Euler(0, 0, 0));
        Instantiate(voidObject, new Vector3(endPoint.x * unitSize, 1, endPoint.y * unitSize), Quaternion.Euler(0, 0, 0));
        //


        for (int i = 0; i < roomCenters.Count; i++)
        {
            int roomStartX = roomCenters.ElementAt(i).x - (avgRoomSize / 2);
            int roomStartY = roomCenters.ElementAt(i).y - (avgRoomSize / 2);
            int roomType = 0;
            int roomBudget = 0;

            for (int j = roomStartX - 1; j < roomStartX + avgRoomSize + 2; j++)
            {
                for (int k = roomStartY - 1; k < roomStartY + avgRoomSize + 2; k++)
                {
                    if (roomBudget >= 100)
                        continue;
                    try
                    {
                        int random = Random.Range(0, 7);
                        if (dungeonGrid[j, k] == 1 && random == 0 && roomType == 0 && roomBudget < 70)
                        {
                            switch (Random.Range(0, 2))
                            {
                                case 0: // large table with 4 chairs will be generated with random position and angle offset.
                                    roomBudget += 35;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(0, 3)), new Vector3(j * unitSize, 0, k * unitSize), Quaternion.Euler(0, GetRndAvg(90, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j - GetRndAvg(0.25f, decPosOffset)) * unitSize, 0, (k - GetRndAvg(0.4f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(-90, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j + GetRndAvg(0.25f, decPosOffset)) * unitSize, 0, (k - GetRndAvg(0.4f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(-90, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j + GetRndAvg(0.25f, decPosOffset)) * unitSize, 0, (k + GetRndAvg(0.4f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(90, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j - GetRndAvg(0.25f, decPosOffset)) * unitSize, 0, (k + GetRndAvg(0.4f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(90, decAngleOffset), 0));
                                    break;
                                case 1: // same as case 0 but everything is rotated by 90 degrees.
                                    roomBudget += 35;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(0, 3)), new Vector3(j * unitSize, 0, k * unitSize), Quaternion.Euler(0, GetRndAvg(0, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j - GetRndAvg(0.4f, decPosOffset)) * unitSize, 0, (k - GetRndAvg(0.25f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(0, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j + GetRndAvg(0.4f, decPosOffset)) * unitSize, 0, (k - GetRndAvg(0.25f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(180, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j + GetRndAvg(0.4f, decPosOffset)) * unitSize, 0, (k + GetRndAvg(0.25f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(180, decAngleOffset), 0));
                                    Instantiate(decorationObjects.ElementAt(3), new Vector3((j - GetRndAvg(0.4f, decPosOffset)) * unitSize, 0, (k + GetRndAvg(0.25f, decPosOffset)) * unitSize), Quaternion.Euler(0, GetRndAvg(0, decAngleOffset), 0));
                                    break;
                            }
                        }
                        else if (dungeonGrid[j - 1, k] == 2 && dungeonGrid[j, k] == 1 && random == 1 && roomType == 0)
                        {
                            switch (Random.Range(0, 2))
                            {
                                case 0: // banner
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(4, 6)), new Vector3((j - 0.6f) * unitSize, 0, k * unitSize), Quaternion.Euler(0, 90, 0));
                                    break;
                                case 1: // shields
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(6, 8)), new Vector3((j - 0.55f) * unitSize, 1.75f, k * unitSize), Quaternion.Euler(0, 90, 0));
                                    break;
                            }
                        }
                        else if (dungeonGrid[j + 1, k] == 2 && dungeonGrid[j, k] == 1 && random == 2 && roomType == 0)
                        {
                            switch (Random.Range(0, 2))
                            {
                                case 0: // banner
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(4, 6)), new Vector3((j + 0.6f) * unitSize, 0, k * unitSize), Quaternion.Euler(0, -90, 0));
                                    break;
                                case 1: // shields
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(6, 8)), new Vector3((j + 0.55f) * unitSize, 1.75f, k * unitSize), Quaternion.Euler(0, -90, 0));
                                    break;
                            }
                        }
                        else if (dungeonGrid[j, k - 1] == 2 && dungeonGrid[j, k] == 1 && random == 3 && roomType == 0)
                        {
                            switch (Random.Range(0, 2))
                            {
                                case 0: // banner
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(4, 6)), new Vector3(j * unitSize, 0, (k - 0.6f) * unitSize), Quaternion.Euler(0, 0, 0));
                                    break;
                                case 1: // shields
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(6, 8)), new Vector3(j * unitSize, 1.75f, (k - 0.55f) * unitSize), Quaternion.Euler(0, 0, 0));
                                    break;
                            }
                        }
                        else if (dungeonGrid[j, k + 1] == 2 && dungeonGrid[j, k] == 1 && random == 4 && roomType == 0)
                        {
                            switch (Random.Range(0, 2))
                            {
                                case 0: // banner
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(4, 6)), new Vector3(j * unitSize, 0, (k + 0.6f) * unitSize), Quaternion.Euler(0, 180, 0));
                                    break;
                                case 1: // shields
                                    roomBudget += 8;
                                    Instantiate(decorationObjects.ElementAt(Random.Range(6, 8)), new Vector3(j * unitSize, 1.75f, (k + 0.55f) * unitSize), Quaternion.Euler(0, 180, 0));
                                    break;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
    }

    private float GetRndAvg(float _average, float _offset)
    {
        return Random.Range(_average - _offset, _average + _offset);
    }
    private void BuildDoors()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                try
                {
                    if (dungeonGrid[i + 1, j] == 1 && dungeonGrid[i + 1, j - 1] != 1 && dungeonGrid[i + 1, j + 1] != 1 && (dungeonGrid[i, j - 1] == 1 || dungeonGrid[i, j + 1] == 1) && dungeonGrid[i - 1, j] == 1)
                    {
                        Vector3 position = new Vector3(((i + 1) * unitSize) - wallOffset , 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 90, 0);
                        Instantiate(doorObject, position, rotation);
                    }
                    if (dungeonGrid[i - 1, j] == 1 && dungeonGrid[i - 1, j - 1] != 1 && dungeonGrid[i - 1, j + 1] != 1 && (dungeonGrid[i, j - 1] == 1 || dungeonGrid[i, j + 1] == 1) && dungeonGrid[i + 1, j] == 1)
                    {
                        Vector3 position = new Vector3(((i - 1) * unitSize) + wallOffset, 0, j * unitSize);
                        Quaternion rotation = Quaternion.Euler(0, 90, 0);
                        Instantiate(doorObject, position, rotation);
                    }
                    if (dungeonGrid[i, j + 1] == 1 && dungeonGrid[i + 1, j + 1] != 1 && dungeonGrid[i - 1, j + 1] != 1 && (dungeonGrid[i - 1, j] == 1 || dungeonGrid[i + 1, j] == 1) && dungeonGrid[i, j - 1] == 1)
                    {
                        Vector3 position = new Vector3(i * unitSize, 0, ((j + 1) * unitSize) - wallOffset);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Instantiate(doorObject, position, rotation);
                    }
                    if (dungeonGrid[i, j - 1] == 1 && dungeonGrid[i - 1, j - 1] != 1 && dungeonGrid[i + 1, j - 1] != 1 && (dungeonGrid[i - 1, j] == 1 || dungeonGrid[i + 1, j] == 1) && dungeonGrid[i, j + 1] == 1)
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

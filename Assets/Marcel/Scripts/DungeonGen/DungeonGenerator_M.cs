using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator_M : MonoBehaviour
{
    private int[,] dungeonGrid;
    private List<Vector2Int> roomCenters;
    [SerializeField, Range(25, 250)] private int gridLength;
    [SerializeField, Range(25, 250)] private int gridWidth;
    [SerializeField, Range(1, 75)] private int roomAmount;
    [SerializeField, Range(2, 7)] private int minRoomSize;
    [SerializeField, Range(7, 40)] private int maxRoomSize;
    [SerializeField, Range(1, 16)] private int unitSize;
    [SerializeField, Range(0, 100)] private int variationRate;
    [SerializeField] private List<GameObject> floorObjects;
    [SerializeField] private List<GameObject> wallObjects;
    private void Start()
    {
        GenerateDungeon();
        ConnectRooms();
        BuildDungeon();
        GenerateWalls();
    }
    private void GenerateDungeon()
    {
        roomCenters = new List<Vector2Int>();
        dungeonGrid = new int[gridLength, gridWidth];
        for (int i = 0; i < roomAmount; i++)
        {
            int roomLength = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomWidth = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomX = Random.Range(0, gridLength - roomLength);
            int roomY = Random.Range(0, gridWidth - roomWidth);
            Vector3 roomOrigin = new Vector3(roomX, 0, roomY);
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

    private void GenerateWalls()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                if (dungeonGrid[i, j] == 1)
                {
                    try
                    {
                        if (dungeonGrid[i + 1, j] == 0)
                        {
                            Vector3 position = new Vector3(((i + 1) * unitSize) - 1.5f, 0, j * unitSize);
                            Quaternion rotation = Quaternion.Euler(0, 90, 0);
                            Instantiate(wallObjects.ElementAt(0), position, rotation);
                        }
                        if (dungeonGrid[i - 1, j] == 0)
                        {
                            Vector3 position = new Vector3(((i - 1) * unitSize) + 1.5f, 0, j * unitSize);
                            Quaternion rotation = Quaternion.Euler(0, -90, 0);
                            Instantiate(wallObjects.ElementAt(0), position, rotation);
                        }
                        if (dungeonGrid[i, j + 1] == 0)
                        {
                            Vector3 position = new Vector3(i * unitSize, 0, ((j + 1) * unitSize) - 1.5f);
                            Quaternion rotation = Quaternion.Euler(0, 0, 0);
                            Instantiate(wallObjects.ElementAt(0), position, rotation);
                        }
                        if (dungeonGrid[i, j - 1] == 0)
                        {
                            Vector3 position = new Vector3(i * unitSize, 0, ((j - 1) * unitSize) + 1.5f);
                            Quaternion rotation = Quaternion.Euler(0, 180, 0);
                            Instantiate(wallObjects.ElementAt(0), position, rotation);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
    }

    private void BuildDungeon()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                Vector3 position = new Vector3(i * unitSize, 0, j * unitSize);
                switch (dungeonGrid[i, j])
                {
                    case 0:
                        break;
                    case 1:
                        int random = Random.Range(0, 101);
                        if (random > variationRate)
                        {
                            Instantiate(floorObjects.ElementAt(0), position, Quaternion.identity);
                        }
                        else
                        {
                            int randomTile = Random.Range(1, 6);
                            Instantiate(floorObjects.ElementAt(randomTile), position, Quaternion.identity);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

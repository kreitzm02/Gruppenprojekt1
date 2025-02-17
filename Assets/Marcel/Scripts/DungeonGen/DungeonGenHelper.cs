using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class DungeonGenHelper
{
    public static Vector2Int GetRandomRoomSize(int _min, int _max)
    {
        return new Vector2Int(Random.Range(_min, _max + 1), Random.Range(_min, _max + 1));
    }

    public static Vector2Int GetRandomRoomOrigin(Vector2Int _roomSize, int _gridLength, int _gridWidth)
    {
        return new Vector2Int(Random.Range(1, _gridLength - _roomSize.x - 1), Random.Range(1, _gridWidth - _roomSize.y - 1));
    }

    public static bool CellIsNeighbourOfPosition(int _x, int _y, CellType _cell, CellType[,] _grid, bool _diagonalIncluded = true)
    {
        if (_diagonalIncluded)
        {
            return (_grid[_x - 1, _y + 1] == _cell ||
            _grid[_x, _y + 1] == _cell ||
            _grid[_x + 1, _y + 1] == _cell ||
            _grid[_x + 1, _y] == _cell ||
            _grid[_x + 1, _y - 1] == _cell ||
            _grid[_x, _y - 1] == _cell ||
            _grid[_x - 1, _y - 1] == _cell ||
            _grid[_x - 1, _y] == _cell);
        }
        else
        {
            return (_grid[_x, _y + 1] == _cell ||
            _grid[_x + 1, _y] == _cell ||
            _grid[_x, _y - 1] == _cell ||
            _grid[_x - 1, _y] == _cell);
        }
    }

    public static Dictionary<Vector2Int, List<Vector2Int>> BuildAdjacencyList(List<Vector2Int> _roomCenters, List<(Vector2Int, Vector2Int)> _mst)
    {
        Dictionary<Vector2Int, List<Vector2Int>> adjacencyList = new();
        foreach (var center in _roomCenters)
        {
            adjacencyList[center] = new List<Vector2Int>();
        }

        foreach (var edge in _mst)
        {
            adjacencyList[edge.Item1].Add(edge.Item2);
            adjacencyList[edge.Item2].Add(edge.Item1);
        }

        return adjacencyList;
    }

    public static int GetRandomRotation()
    {
        return Random.Range(-179, 181);
    }

    public static int GetOrthogonalRotationBasedOnCenter(Vector3 _position, Vector3 _center)
    {
        float differenceX = _position.x - _center.x;
        float differenceY = _position.y - _center.y;
        if (Mathf.Abs(differenceX) > Mathf.Abs(differenceY))
            return differenceX > 0 ? 270 : 90;
        else
            return differenceY > 0 ? 180 : 0;
    }

    public static (Vector2Int, Vector2Int) DetermineDungeonDiameter(List<Vector2Int> _roomCenters, Dictionary<Vector2Int, List<Vector2Int>> _adjacencyList)
    {
        Vector2Int arbitraryNode = _roomCenters.ElementAt(0);
        Vector2Int nodeA = BFSFindFarthestNode(arbitraryNode, _roomCenters, _adjacencyList);
        Vector2Int nodeB = BFSFindFarthestNode(nodeA, _roomCenters, _adjacencyList);

        return (nodeA, nodeB);
    }

    private static Vector2Int BFSFindFarthestNode(Vector2Int _start, List<Vector2Int> _roomCenters, Dictionary<Vector2Int, List<Vector2Int>> _adjacencyList)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();

        foreach (var center in _roomCenters)
            distances[center] = -1;

        distances[_start] = 0;
        queue.Enqueue(_start);

        Vector2Int farthestNode = _start;
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

            foreach (var neighbor in _adjacencyList[current])
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

    public static List<(Vector2Int, Vector2Int)> CreateMinimumSpanningTree(List<Vector2Int> _roomCenters, List<(Vector2Int, Vector2Int, float)> _connections)
    {
        //Minimum Spanning Tree (MST) Kruskal Algorithmus

        List<(Vector2Int, Vector2Int)> mst = new();
        Dictionary<Vector2Int, int> roomGroups = new Dictionary<Vector2Int, int>();
        int groupCounter = 0;

        foreach (var center in _roomCenters)
        {
            roomGroups[center] = groupCounter++;
        }

        foreach (var connection in _connections)
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

        return mst;
    }
}

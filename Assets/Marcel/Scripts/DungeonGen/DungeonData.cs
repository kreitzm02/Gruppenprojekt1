using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public List<DungeonRoom> allRooms;
    public List<Vector2Int> allRoomCenters;
    public DungeonRoom startRoom;
    public DungeonRoom endRoom;
    public List<DungeonRoom> deadEndRooms;
    public List<(Vector2Int, Vector2Int)> MST;
    public Dictionary<Vector2Int, List<Vector2Int>> AdjacencyList;
    public CellType[,] dungeonGrid;

    public void InitializeData(int _length, int _width)
    {
        dungeonGrid = new CellType[_length, _width];
        allRooms = new List<DungeonRoom>();
        allRoomCenters = new List<Vector2Int>();
        MST = new List<(Vector2Int, Vector2Int)>();
        AdjacencyList = new();
        deadEndRooms = new();
    }

    public void GetStartAndEndRoom((Vector2Int, Vector2Int) _positions)
    {
        foreach (DungeonRoom room in allRooms)
        {
            if (room.GetRoomCenter() == _positions.Item1)
            {
                room.SetRoomType(RoomType.Start);
                startRoom = room;
            }

            if (room.GetRoomCenter() == _positions.Item2)
            {
                room.SetRoomType(RoomType.End);
                endRoom = room;
            }
        }
    }

    public void GetDeadEndRooms()
    {
        foreach (DungeonRoom room in allRooms)
        {
            if (AdjacencyList[room.GetRoomCenter()].Count == 1)
            {
                if (room.type == RoomType.End || room.type == RoomType.Start) continue;
                deadEndRooms.Add(room);
                room.SetRoomType(RoomType.Loot);
            }
        }
    }
}

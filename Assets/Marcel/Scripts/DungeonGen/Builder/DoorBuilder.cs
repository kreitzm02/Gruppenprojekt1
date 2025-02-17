using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class DoorBuilder : IDoorBuilder
{
    private int unitSize;
    private float wallOffset;
    private GameObject doorPrefab;

    public DoorBuilder(int _unitSize, float _wallOffset, GameObject _doorPrefab)
    {
        unitSize = _unitSize;
        wallOffset = _wallOffset;
        doorPrefab = _doorPrefab;
    }

    public void BuildDoors(DungeonData _data) 
    {
        foreach (var room in _data.allRooms)
        {
            for (int i = room.origin.x; i < room.origin.x + room.length; i++)
            {
                for (int j = room.origin.y; j < room.origin.y + room.height; j++)
                {
                    TryPlaceDoor(_data, room, i, j);
                }
            }
        }
    }

    private void TryPlaceDoor(DungeonData _data, DungeonRoom _room, int _x, int _y)
    {
        if ((_data.dungeonGrid[_x + 1, _y] == CellType.Floor || _data.dungeonGrid[_x + 1, _y] == CellType.FloorWithDoor) &&
            _data.dungeonGrid[_x + 1, _y - 1] != CellType.Floor && _data.dungeonGrid[_x + 1, _y + 1] != CellType.Floor &&
            (_data.dungeonGrid[_x, _y - 1] == CellType.Floor || _data.dungeonGrid[_x, _y + 1] == CellType.Floor) &&
            _data.dungeonGrid[_x - 1, _y] == CellType.Floor)
        {
            Vector3 position = new Vector3(((_x + 1) * unitSize) - wallOffset, 0, _y * unitSize);
            Quaternion rotation = Quaternion.Euler(0, 90, 0);
            GameObject door = Object.Instantiate(doorPrefab, position, rotation);
            _room.doors.Add(door);
            _data.dungeonGrid[_x + 1, _y] = CellType.FloorWithDoor;
        }
        else if ((_data.dungeonGrid[_x - 1, _y] == CellType.Floor || _data.dungeonGrid[_x - 1, _y] == CellType.FloorWithDoor) &&
            _data.dungeonGrid[_x - 1, _y - 1] != CellType.Floor && _data.dungeonGrid[_x - 1, _y + 1] != CellType.Floor &&
            (_data.dungeonGrid[_x, _y - 1] == CellType.Floor || _data.dungeonGrid[_x, _y + 1] == CellType.Floor) &&
            _data.dungeonGrid[_x + 1, _y] == CellType.Floor)
        {
            Vector3 position = new Vector3(((_x - 1) * unitSize) + wallOffset, 0, _y * unitSize);
            Quaternion rotation = Quaternion.Euler(0, -90, 0);
            GameObject door = Object.Instantiate(doorPrefab, position, rotation);
            _room.doors.Add(door);
            _data.dungeonGrid[_x - 1, _y] = CellType.FloorWithDoor;
        }
        else if ((_data.dungeonGrid[_x, _y + 1] == CellType.Floor || _data.dungeonGrid[_x, _y + 1] == CellType.FloorWithDoor) &&
            _data.dungeonGrid[_x + 1, _y + 1] != CellType.Floor && _data.dungeonGrid[_x - 1, _y + 1] != CellType.Floor &&
            (_data.dungeonGrid[_x - 1, _y] == CellType.Floor || _data.dungeonGrid[_x + 1, _y] == CellType.Floor) &&
            _data.dungeonGrid[_x, _y - 1] == CellType.Floor)
        {
            Vector3 position = new Vector3(_x * unitSize, 0, ((_y + 1) * unitSize) - wallOffset);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            GameObject door = Object.Instantiate(doorPrefab, position, rotation);
            _room.doors.Add(door);
            _data.dungeonGrid[_x, _y + 1] = CellType.FloorWithDoor;
        }
        else if ((_data.dungeonGrid[_x, _y - 1] == CellType.Floor || _data.dungeonGrid[_x, _y - 1] == CellType.FloorWithDoor) &&
            _data.dungeonGrid[_x - 1, _y - 1] != CellType.Floor && _data.dungeonGrid[_x + 1, _y - 1] != CellType.Floor &&
            (_data.dungeonGrid[_x - 1, _y] == CellType.Floor || _data.dungeonGrid[_x + 1, _y] == CellType.Floor) &&
            _data.dungeonGrid[_x, _y + 1] == CellType.Floor)
        {
            Vector3 position = new Vector3(_x * unitSize, 0, ((_y - 1) * unitSize) + wallOffset);
            Quaternion rotation = Quaternion.Euler(0, 180, 0);
            GameObject door = Object.Instantiate(doorPrefab, position, rotation);
            _room.doors.Add(door);
            _data.dungeonGrid[_x, _y - 1] = CellType.FloorWithDoor;
        }
    }
}

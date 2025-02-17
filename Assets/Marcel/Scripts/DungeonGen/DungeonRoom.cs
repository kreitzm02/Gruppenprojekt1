using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom
{
    public int length;
    public int height;
    public Vector2Int origin;
    public RoomType type;
    public int enemyCount = 0;
    public List<GameObject> doors = new List<GameObject>();

    public DungeonRoom(int _length, int _height, Vector2Int _origin)
    {
        length = _length;
        height = _height;
        origin = _origin;
    }

    public void DeactivateAllDoorTriggers()
    {
        foreach(var door in doors)
        {
            var trigger = door.GetComponentInChildren<DungeonDoorTrigger>();
            trigger.gameObject.SetActive(false);
        }
    }

    public bool OverlapsWith(DungeonRoom _other)
    {
        return !(origin.x + length < _other.origin.x || _other.origin.x + _other.length < origin.x || origin.y + height < _other.origin.y || _other.origin.y + _other.height < origin.y);
    }

    public Vector2Int GetRoomCenter()
    {
        return new(origin.x + length / 2, origin.y + height / 2);
    }

    public Vector3 GetRoomCenterVec3()
    {
        return new(origin.x + length / 2, 0, origin.y + height / 2);
    }

    public void SetRoomType()
    {
        int roomTypeCount = Enum.GetValues(typeof(RoomType)).Length;
        type = (RoomType)UnityEngine.Random.Range(0, roomTypeCount - 4); // the last 4 enum values are reserved for set rooms.
    }

    public void SetRoomType(RoomType _roomType)
    {
        type = _roomType;
    }
}

public enum RoomType
{
    Empty, Sleep, Storage, Dinner, Loot, Start, End, Boss
}


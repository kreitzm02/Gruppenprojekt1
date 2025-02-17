using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;

public class DecorationBuilder : IDecorationBuilder
{
    private int unitSize;
    private float angleOffset, posOffset;
    private List<GameObject> decorationPrefabs;

    public DecorationBuilder(int _unitSize, float _angleOffset, float _posOffset, List<GameObject> _decorationPrefabs)
    {
        this.unitSize = _unitSize;
        this.angleOffset = _angleOffset;
        this.posOffset = _posOffset;
        this.decorationPrefabs = _decorationPrefabs;
    }

    public void BuildDecorations(DungeonData _data)
    {
        // Beispiel: Platziere Dekoration an Start- und Endpunkt.
        foreach (DungeonRoom room in _data.allRooms)
        {
            switch (room.type)
            {
                case RoomType.Empty:
                    break;
                case RoomType.Sleep:
                    BuildSleepDecorations(room, _data);
                    break;
                case RoomType.Loot:
                    BuildLootDecorations(room, _data);
                    break;
                case RoomType.Storage:
                    BuildStorageDecorations(room, _data);
                    break;
                case RoomType.Start:
                    BuildStartDecorations(room);
                    break;
                case RoomType.End:
                    BuildEndDecorations(room);
                    break;
                case RoomType.Dinner:
                    BuildDinnerDecoration(room, _data);
                    break;
            }
        }
    }

    private void BuildDinnerDecoration(DungeonRoom _room, DungeonData _data)
    {
        Vector2Int roomCenter = _room.GetRoomCenter();
        Object.Instantiate(decorationPrefabs.ElementAt(3), new Vector3(roomCenter.x * unitSize, 0, roomCenter.y * unitSize), Quaternion.Euler(0, 90, 0));
        Object.Instantiate(decorationPrefabs.ElementAt(14), new Vector3((roomCenter.x - 0.25f) * unitSize, 0, (roomCenter.y - 0.4f) * unitSize), Quaternion.Euler(0, -90, 0));
        Object.Instantiate(decorationPrefabs.ElementAt(14), new Vector3((roomCenter.x + 0.25f) * unitSize, 0, (roomCenter.y - 0.4f) * unitSize), Quaternion.Euler(0, -90, 0));
        Object.Instantiate(decorationPrefabs.ElementAt(14), new Vector3((roomCenter.x + 0.25f) * unitSize, 0, (roomCenter.y + 0.4f) * unitSize), Quaternion.Euler(0, 90,  0));
        Object.Instantiate(decorationPrefabs.ElementAt(14), new Vector3((roomCenter.x - 0.25f) * unitSize, 0, (roomCenter.y + 0.4f) * unitSize), Quaternion.Euler(0, 90, 0));

        int randomBannerId = Random.Range(10, 14);
        for (int i = _room.origin.x; i < _room.origin.x + _room.length; i++)
        {
            for (int j = _room.origin.y; j < _room.origin.y + _room.height; j++)
            {
                TryPlacePrefabNearWallOrthographicRotation(_data.dungeonGrid, i, j, 30, _room, decorationPrefabs[Random.Range(15, 17)]);
                TryPlaceBannerOnWall(_data.dungeonGrid, i, j, _room, decorationPrefabs[randomBannerId]);
            }
        }
    }

    private void BuildSleepDecorations(DungeonRoom _room, DungeonData _data)
    {
        int randomBannerId = Random.Range(10, 14);
        for (int i = _room.origin.x; i < _room.origin.x + _room.length; i++)
        {
            for (int j = _room.origin.y; j < _room.origin.y + _room.height; j++)
            {
                _ = TryPlacePrefabNearWallFacingCenter(_data.dungeonGrid, i, j, 10, _room, decorationPrefabs[9]) ||
                    TryPlacePrefabNearWallOrthographicRotation(_data.dungeonGrid, i, j, 25, _room, decorationPrefabs[Random.Range(17, 19)]) ||
                    TryPlacePrefabSurroundedByFloor(_data.dungeonGrid, i, j, 45, _room, decorationPrefabs[4]);
                TryPlaceBannerOnWall(_data.dungeonGrid, i, j, _room, decorationPrefabs[randomBannerId]);
            }
        }
    }

    private void BuildLootDecorations(DungeonRoom _room, DungeonData _data)
    {
        int randomBannerId = Random.Range(10, 14);
        for (int i = _room.origin.x; i < _room.origin.x + _room.length; i++)
        {
            for (int j = _room.origin.y; j < _room.origin.y + _room.height; j++)
            {
                _ = TryPlacePrefabNearWallFacingCenter(_data.dungeonGrid, i, j, 67, _room, decorationPrefabs[Random.Range(0, 3)]) ||
                    TryPlacePrefabSurroundedByFloor(_data.dungeonGrid, i, j, 20, _room, decorationPrefabs[Random.Range(1, 3)]);
                TryPlaceBannerOnWall(_data.dungeonGrid, i, j, _room, decorationPrefabs[randomBannerId]);
            }
        }
    }

    private void BuildStorageDecorations(DungeonRoom _room, DungeonData _data)
    {
        int randomBannerId = Random.Range(10, 14);
        for (int i = _room.origin.x; i < _room.origin.x + _room.length; i++)
        {
            for (int j = _room.origin.y; j < _room.origin.y + _room.height; j++)
            {
                _ = TryPlacePrefabNearWallFacingCenter(_data.dungeonGrid, i, j, 20, _room, decorationPrefabs[8]) ||
                    TryPlacePrefabNearWallFacingCenter(_data.dungeonGrid, i, j, 20, _room, decorationPrefabs[0]) ||
                    TryPlacePrefabSurroundedByFloor(_data.dungeonGrid, i, j, 15, _room, decorationPrefabs[7]);
                TryPlaceBannerOnWall(_data.dungeonGrid, i, j, _room, decorationPrefabs[randomBannerId]);
            }
        }
    }

    private void BuildStartDecorations(DungeonRoom _room)
    {
        Object.Instantiate(decorationPrefabs[3], new Vector3(_room.GetRoomCenter().x * unitSize, 0, _room.GetRoomCenter().y * unitSize), Quaternion.identity);
    }

    private void BuildEndDecorations(DungeonRoom _room)
    {
        Object.Instantiate(decorationPrefabs[3], new Vector3(_room.GetRoomCenter().x * unitSize, 0, _room.GetRoomCenter().y * unitSize), Quaternion.identity);
    }

    private bool TryPlacePrefabNearWallFacingCenter(CellType[,] _grid, int _x, int _y, int _probability, DungeonRoom _room, GameObject _prefab)
    {
        if (_grid[_x, _y] == CellType.Floor && !DungeonGenHelper.CellIsNeighbourOfPosition(_x, _y, CellType.FloorWithDoor, _grid, false) &&
                    DungeonGenHelper.CellIsNeighbourOfPosition(_x, _y, CellType.Wall, _grid))
        {
            if (Random.Range(0, 101) < 100 - _probability) return false;
            Vector3 targetPosition = new Vector3(_x * unitSize, 0, _y * unitSize);
            Vector3 direction = (_room.GetRoomCenterVec3() * unitSize - targetPosition).normalized;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Object.Instantiate(_prefab, targetPosition, targetRotation);
            return true;
        }
        return false;
    }

    private bool TryPlacePrefabNearWallOrthographicRotation(CellType[,] _grid, int _x, int _y, int _probability, DungeonRoom _room, GameObject _prefab)
    {
        if (_grid[_x, _y] == CellType.Floor && !DungeonGenHelper.CellIsNeighbourOfPosition(_x, _y, CellType.FloorWithDoor, _grid, false) &&
                    DungeonGenHelper.CellIsNeighbourOfPosition(_x, _y, CellType.Wall, _grid))
        {
            if (Random.Range(0, 101) < 100 - _probability) return false;
            Vector3 targetPosition = new Vector3(_x * unitSize, 0, _y * unitSize);
            float rotationY = DungeonGenHelper.GetOrthogonalRotationBasedOnCenter(targetPosition, _room.GetRoomCenterVec3() * unitSize);
            Quaternion targetRotation = Quaternion.Euler(0, rotationY, 0);
            Object.Instantiate(_prefab, targetPosition, targetRotation);
            return true;
        }
        return false;
    }

    private bool TryPlacePrefabSurroundedByFloor(CellType[,] _grid, int _x, int _y, int _probability, DungeonRoom _room, GameObject _prefab)
    {
        if (_grid[_x, _y] == CellType.Floor && !DungeonGenHelper.CellIsNeighbourOfPosition(_x, _y, CellType.Wall, _grid))
        {
            if (Random.Range(0, 101) < 100 - _probability) return false;
            Object.Instantiate(_prefab, new Vector3(_x * unitSize, 0, _y * unitSize), Quaternion.Euler(0, DungeonGenHelper.GetRandomRotation(), 0));
            return true;
        }
        return false;
    }

    private void TryPlaceBannerOnWall(CellType[,] _grid, int _x, int _y, DungeonRoom _room, GameObject _prefab)
    {
        if (_grid[_x - 1, _y + 1] == CellType.WallCorner)
        {
            Object.Instantiate(_prefab, new Vector3((_x - 0.62f) * unitSize, 0, _y * unitSize), Quaternion.Euler(0, 90, 0));
            Object.Instantiate(_prefab, new Vector3(_x * unitSize, 0, (_y + 0.62f) * unitSize), Quaternion.Euler(0, 180, 0));
        }
        else if (_grid[_x - 1, _y - 1] == CellType.WallCorner)
        {
            Object.Instantiate(_prefab, new Vector3((_x - 0.62f) * unitSize, 0, _y * unitSize), Quaternion.Euler(0, 90, 0));
            Object.Instantiate(_prefab, new Vector3(_x * unitSize, 0, (_y - 0.62f) * unitSize), Quaternion.Euler(0, 0, 0));
        }
        else if (_grid[_x + 1, _y + 1] == CellType.WallCorner)
        {
            Object.Instantiate(_prefab, new Vector3((_x + 0.62f) * unitSize, 0, _y * unitSize), Quaternion.Euler(0, -90, 0));
            Object.Instantiate(_prefab, new Vector3(_x * unitSize, 0, (_y + 0.62f) * unitSize), Quaternion.Euler(0, 180, 0));
        }
        else if (_grid[_x + 1, _y - 1] == CellType.WallCorner)
        {
            Object.Instantiate(_prefab, new Vector3((_x + 0.62f) * unitSize, 0, _y * unitSize), Quaternion.Euler(0, -90, 0));
            Object.Instantiate(_prefab, new Vector3(_x * unitSize, 0, (_y - 0.62f) * unitSize), Quaternion.Euler(0, 0, 0));
        }
    }
}

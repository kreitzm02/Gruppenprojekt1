using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder : IWallBuilder
{
    private int unitSize;
    private float wallOffset;
    private GameObject wallCornerPrefab;
    private List<GameObject> wallPrefabs;

    public WallBuilder(int _unitSize, float _wallOffset, GameObject _wallCornerPrefab, List<GameObject> _wallPrefabs)
    {
        unitSize = _unitSize;
        wallOffset = _wallOffset;
        wallCornerPrefab = _wallCornerPrefab;
        wallPrefabs = _wallPrefabs;
    }

    public void BuildWalls(DungeonData _data)
    {
        int length = _data.dungeonGrid.GetLength(0);
        int width = _data.dungeonGrid.GetLength(1);
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (_data.dungeonGrid[x, y] == CellType.Floor)
                {
                    int randomIndex = Random.Range(0, wallPrefabs.Count);
                    TryPlaceWall(_data, x + 1, y, new Vector3(((x + 1) * unitSize) - wallOffset, 0, y * unitSize), Quaternion.Euler(0, -90, 0), randomIndex);
                    TryPlaceWall(_data, x - 1, y, new Vector3(((x - 1) * unitSize) + wallOffset, 0, y * unitSize), Quaternion.Euler(0, 90, 0), randomIndex);
                    TryPlaceWall(_data, x, y + 1, new Vector3(x * unitSize, 0, ((y + 1) * unitSize) - wallOffset), Quaternion.Euler(0, 180, 0), randomIndex);
                    TryPlaceWall(_data, x, y - 1, new Vector3(x * unitSize, 0, ((y - 1) * unitSize) + wallOffset), Quaternion.identity, randomIndex);
    
                    // corner walls
                    if (_data.dungeonGrid[x + 1, y - 1] != CellType.Floor && _data.dungeonGrid[x + 1, y] != CellType.Floor && _data.dungeonGrid[x, y - 1] != CellType.Floor)
                    {
                        _data.dungeonGrid[x + 1, y - 1] = CellType.WallCorner;
                        Vector3 position = new Vector3(((x + 1) * unitSize) - wallOffset, 0, ((y - 1) * unitSize) + wallOffset);
                        Quaternion rotation = Quaternion.Euler(0, -90, 0);
                        Object.Instantiate(wallCornerPrefab, position, rotation);
                    }
                    if (_data.dungeonGrid[x + 1, y + 1] != CellType.Floor && _data.dungeonGrid[x + 1, y] != CellType.Floor && _data.dungeonGrid[x, y + 1] != CellType.Floor)
                    {
                        _data.dungeonGrid[x + 1, y + 1] = CellType.WallCorner;
                        Vector3 position = new Vector3(((x + 1) * unitSize) - wallOffset, 0, ((y + 1) * unitSize) - wallOffset);
                        Quaternion rotation = Quaternion.Euler(0, 180, 0);
                        Object.Instantiate(wallCornerPrefab, position, rotation);
                    }
                    if (_data.dungeonGrid[x - 1, y + 1] != CellType.Floor && _data.dungeonGrid[x - 1, y] != CellType.Floor && _data.dungeonGrid[x, y + 1] != CellType.Floor)
                    {
                        _data.dungeonGrid[x - 1, y + 1] = CellType.WallCorner;
                        Vector3 position = new Vector3(((x - 1) * unitSize) + wallOffset, 0, ((y + 1) * unitSize) - wallOffset);
                        Quaternion rotation = Quaternion.Euler(0, 90, 0);
                        Object.Instantiate(wallCornerPrefab, position, rotation);
                    }
                    if (_data.dungeonGrid[x - 1, y - 1] != CellType.Floor && _data.dungeonGrid[x - 1, y] != CellType.Floor && _data.dungeonGrid[x, y - 1] != CellType.Floor)
                    {
                        _data.dungeonGrid[x - 1, y - 1] = CellType.WallCorner;
                        Vector3 position = new Vector3(((x - 1) * unitSize) + wallOffset, 0, ((y - 1) * unitSize) + wallOffset);
                        Quaternion rotation = Quaternion.Euler(0, 0, 0);
                        Object.Instantiate(wallCornerPrefab, position, rotation);
                    }
                }
            }
        }
    }

    private void TryPlaceWall(DungeonData _data, int _x, int _y, Vector3 _pos, Quaternion _rot, int _wallIndex)
    {
        if (!IsWithinBounds(_data, _x, _y)) return;
        if (_data.dungeonGrid[_x, _y] == CellType.Floor) return;
        _data.dungeonGrid[_x, _y] = CellType.Wall;
        Object.Instantiate(wallPrefabs[_wallIndex], _pos, _rot);
    }

    private void TryPlaceCorner(DungeonData _data, int _x, int _y, Vector3 _pos, Quaternion _rot)
    {
        if (!IsWithinBounds(_data, _x, _y)) return;
        if (_data.dungeonGrid[_x, _y] == CellType.Floor) return;
        _data.dungeonGrid[_x, _y] = CellType.WallCorner;
        Object.Instantiate(wallCornerPrefab, _pos, _rot);
    }

    private bool IsWithinBounds(DungeonData _data, int _x, int _y)
    {
        return _x >= 0 && _y >= 0 && _x < _data.dungeonGrid.GetLength(0) && _y < _data.dungeonGrid.GetLength(1);
    }
}

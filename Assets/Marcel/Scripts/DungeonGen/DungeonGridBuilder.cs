using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGridBuilder
{
    public static List<Vector2Int> CollectAffectedCells(Vector2Int _origin, Vector2Int _size)
    {
        List<Vector2Int> affectedCells = new();
        for (int i = _origin.x; i < _origin.x + _size.x; i++)
        {
            for (int j = _origin.y; j < _origin.y + _size.y; j++)
            {
                affectedCells.Add(new Vector2Int(i, j));
            }
        }
        return affectedCells;   
    }

    public static CellType[,] ChangeCellsInGrid(List<Vector2Int> _cells, CellType _cellType, CellType[,] _grid)
    {
        foreach (var cell in _cells)
        {
            if (_grid[cell.x, cell.y] != CellType.Empty) continue;
            _grid[cell.x, cell.y] = _cellType;
        }
        return _grid;
    }
}

public enum CellType
{
    Empty = 0, Floor, Wall, WallCorner, WallDecorated, FloorCorridor, FloorWithDoor
}

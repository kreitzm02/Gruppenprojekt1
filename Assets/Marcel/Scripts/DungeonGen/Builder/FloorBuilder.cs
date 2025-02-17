using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBuilder : IFloorBuilder
{
    private int unitSize, variationRate;
    private List<GameObject> floorPrefabs;
    private GameObject voidPrefab;

    public FloorBuilder(int _unitSize, int _variationRate, List<GameObject> _floorPrefabs, GameObject _voidPrefab)
    {
        unitSize = _unitSize;
        variationRate = _variationRate;
        floorPrefabs = _floorPrefabs;
        voidPrefab = _voidPrefab;
    }

    public void BuildFloor(DungeonData _data)
    {
        for (int x = 0; x < _data.dungeonGrid.GetLength(0); x++)
        {
            for (int y = 0; y < _data.dungeonGrid.GetLength(1); y++)
            {
                Vector3 pos = new Vector3(x * unitSize, 0, y * unitSize);
                if (_data.dungeonGrid[x, y] == CellType.Empty)
                {
                    pos.y += unitSize * 0.85f;
                    Object.Instantiate(voidPrefab, pos, Quaternion.identity);
                }
                else if (_data.dungeonGrid[x, y] == CellType.Floor || _data.dungeonGrid[x, y] == CellType.FloorCorridor)
                {
                    Object.Instantiate(floorPrefabs[0], pos, Quaternion.identity);
    
                    //TODO random floor tile placement needs to be redone
                    
                }
            }
        }
    }
}

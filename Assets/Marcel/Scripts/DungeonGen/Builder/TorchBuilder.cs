using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBuilder : ITorchBuilder
{
    private int unitSize;
    private GameObject torchPrefab;
    private int torchSpawnChance;

    public TorchBuilder(int _unitSize, GameObject _torchPrefab, int _torchSpawnChance)
    {
        unitSize = _unitSize;
        torchPrefab = _torchPrefab;
        torchSpawnChance = _torchSpawnChance;
    }

    public void BuildTorches(DungeonData _data)
    {
        
    }
}

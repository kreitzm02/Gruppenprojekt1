using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Generation : MonoBehaviour
{
    [SerializeField]
    float hexRadius = 1.0f;

    [SerializeField]
    GameObject hexTilePrefab;

    [SerializeField]
    int worldWidth = 10;

    [SerializeField]
    int worldHeight = 10;

    private void Start()
    {
        GenerateHexGrid(worldWidth,worldHeight);
    }
    Vector3 HexPosition(int _width, int _height)
    {
        float z = hexRadius * (Mathf.Sqrt(12) / 2 * _height);
        float x = hexRadius * (Mathf.Sqrt(4) * _width + Mathf.Sqrt(4) / 2 * _height%2);
        return new Vector3(x, 0, z);
    }

    void GenerateHexGrid(int _width, int _height)
    {
        for (int i = 0; i < _height; i++)
        {
            for(int j = 0; j < _width; j++)
            {
                Vector3 hexPos = HexPosition(j, i);

                GameObject hexTile = Instantiate(hexTilePrefab, hexPos, Quaternion.identity);
                hexTile.name = $"Hex_{j}_{i}";
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator_M : MonoBehaviour
{
    private int[,] dungeonGrid;
    [SerializeField, Range(25, 100)] private int gridLength;
    [SerializeField, Range(25, 100)] private int gridWidth;
    [SerializeField, Range(1, 6)] private int roomAmount;
    [SerializeField, Range(2, 4)] private int minRoomSize;
    [SerializeField, Range(5, 7)] private int maxRoomSize;
    private void Start()
    {
        GenerateDungeon();
    }
    private void GenerateDungeon()
    {
        dungeonGrid = new int[gridLength, gridWidth];
        for (int i = 0; i < roomAmount; i++)
        {
            int roomLength = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomWidth = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomX = Random.Range(0, gridLength + 1);
            int roomY = Random.Range(0, gridWidth * 1);
            Vector3 roomOrigin = new Vector3(roomX, 0, roomY);
            for (int j = (int)roomOrigin.x; j < (int)roomOrigin.x + roomLength; j++)
            {
                for (int k = (int)roomOrigin.z; k < (int)roomOrigin.z + roomWidth; k++)
                {
                    dungeonGrid[j, k] = 1;
                }
            }
        }
        for (int i = 0; i < dungeonGrid.GetLength(0);i++)
        {
            for (int j = 0;  j < dungeonGrid.GetLength(1);j++)
            {
                if (dungeonGrid[i, j] != 1)
                {
                    dungeonGrid[i, j] = 0;
                }
            }
        }
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            string row = "";
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                row += dungeonGrid[i, j];
            }
            Debug.Log(row);
        }
    }
}

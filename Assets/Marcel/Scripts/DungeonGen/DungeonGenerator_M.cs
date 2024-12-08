using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator_M : MonoBehaviour
{
    private int[,] dungeonGrid;
    [SerializeField, Range(25, 100)] private int gridLength;
    [SerializeField, Range(25, 100)] private int gridWidth;
    [SerializeField, Range(1, 25)] private int roomAmount;
    [SerializeField, Range(2, 7)] private int minRoomSize;
    [SerializeField, Range(7, 20)] private int maxRoomSize;
    [SerializeField, Range(1, 16)] private int unitSize;
    [SerializeField, Range(0, 100)] private int variationRate;
    [SerializeField] private List<GameObject> floorObjects;
    private void Start()
    {
        GenerateDungeon();
        BuildDungeon();
    }
    private void GenerateDungeon()
    {
        dungeonGrid = new int[gridLength, gridWidth];
        for (int i = 0; i < roomAmount; i++)
        {
            int roomLength = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomWidth = Random.Range(minRoomSize, maxRoomSize + 1);
            int roomX = Random.Range(0, gridLength - roomLength);
            int roomY = Random.Range(0, gridWidth - roomWidth);
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
        
        // Displaying the generated grid in the Debug Log

        //for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        //{
        //    string row = "";
        //    for (int j = 0; j < dungeonGrid.GetLength(1); j++)
        //    {
        //        row += dungeonGrid[i, j];
        //    }
        //    Debug.Log(row);
        //}
    }

    private void BuildDungeon()
    {
        for (int i = 0; i < dungeonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < dungeonGrid.GetLength(1); j++)
            {
                Vector3 position = new Vector3(i * unitSize, 0, j * unitSize);
                switch (dungeonGrid[i, j])
                {
                    case 0:
                        break;
                    case 1:
                        int random = Random.Range(0, 101);
                        if (random > variationRate)
                        {
                            Instantiate(floorObjects.ElementAt(0), position, Quaternion.identity);
                        }
                        else
                        {
                            int randomTile = Random.Range(1, 6);
                            Instantiate(floorObjects.ElementAt(randomTile), position, Quaternion.identity);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
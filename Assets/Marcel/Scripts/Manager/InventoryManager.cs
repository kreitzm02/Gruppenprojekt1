using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int playerGold = 0;
    public int daggerPieces = 0;
    public int swordPieces = 0;
    public int bladePieces = 0;
    public bool obtainedAxe = true;
    public bool obtainedSword = false;
    public bool obtainedBlade = false;
    public bool obtainedDagger = false;
    public static InventoryManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (daggerPieces == 8)
        {
            obtainedDagger = true;
        }
        if (swordPieces == 8)
        { 
            obtainedSword = true; 
        }
        if (bladePieces == 8)
        {
            obtainedBlade = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonLevelManager : MonoBehaviour
{
    public static DungeonLevelManager Instance { get; private set; }
    public List<DungeonRoom> allRooms = new List<DungeonRoom>();
    public DungeonRoom activeRoom;

    public List<DoorGateBehaviour> doorGateBehaviours = new List<DoorGateBehaviour>();
    public bool closeAllDoors = false;

    public bool registeredKilledEnemy = false;

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
        CheckAliveEnemiesInActiveRoom();
        if (closeAllDoors)
        {
            foreach (var doorGate in doorGateBehaviours)
            {
                doorGate.gateIsActivated = true;
            }
        }
        else
        {
            foreach (var doorGate in doorGateBehaviours)
            {
                doorGate.gateIsActivated = false;
            }
        }
    }

    public void DeactivateDoorTriggersInRoom(GameObject _doorTrigger)
    {
        foreach (var room in allRooms)
        {
            for (int i = 0; i < room.doors.Count; i++)
            {
                if (room.doors.ElementAt(i) != _doorTrigger.transform.parent.gameObject) continue;
                room.DeactivateAllDoorTriggers();
                activeRoom = room;
            }
        }
    }

    private void CheckAliveEnemiesInActiveRoom()
    {
        if (activeRoom == null)
        {
            return;
        }
        if (activeRoom.enemyCount > 0 && registeredKilledEnemy)
        {
            registeredKilledEnemy = false;
            activeRoom.enemyCount -= 1;
        }
        if (activeRoom.enemyCount == 0)
        {
            closeAllDoors = false;
            activeRoom = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DungeonLevelManager.Instance.closeAllDoors = true;
            DungeonLevelManager.Instance.DeactivateDoorTriggersInRoom(this.gameObject);
        }
    }
}

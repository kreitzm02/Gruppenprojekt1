using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachineRefsCedrik : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> waypoints;
    public List<GameObject> waypointsRef { get; private set; }


    [SerializeField]
    private bool spawnPointRefNeeded = false;
    public Vector3 spawnPointRef {  get; private set; }


    [SerializeField]
    private GameObject player;
    public GameObject playerRef {  get; private set; }


    public Animator animatorRef { get; private set; }

    private void Awake()
    {
        if (waypoints != null)
        {
            waypointsRef = new List<GameObject>();
            for (int i = 0; i < waypoints.Count; i++)
            {
                waypointsRef.Add(waypoints[i]);
                Debug.Log(i);
            }
        }

        if (spawnPointRefNeeded == true)
        {
            spawnPointRef = this.transform.position;
        }

        if (player != null)
        {
            playerRef = player;
        }

        if(this.GetComponent<Animator>() != null)
        {
            animatorRef = GetComponent<Animator>();
        }
    }
}

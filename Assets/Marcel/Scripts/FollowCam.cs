using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private Camera cam;
    public Transform objectToFollow;
    public float offsetY = 10;
    public float offsetZ = 10;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (objectToFollow != null)
        {
            transform.position = new Vector3(objectToFollow.position.x, objectToFollow.position.y + offsetY, objectToFollow.position.z - offsetZ);
        }
    }
}

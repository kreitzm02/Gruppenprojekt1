using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance { get; private set; }

    public bool crosshairIsActive = false;
    public bool crosshairCreated = false;
    [SerializeField] private Camera playerCam;
    public Transform throwOrigin;
    [SerializeField] private GameObject crosshairPrefab;
    [SerializeField] private float maxThrowDistance;
    private GameObject crosshairInstance;
    [SerializeField] private LayerMask floorLayer;
    public Transform crosshairTransform;
    [SerializeField] private Collider infiniteFloorCollider;
    private Plane infiniteFloorPlane = new Plane(Vector3.up, 0f);

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
        if (crosshairTransform == null)
        {
            crosshairTransform = this.transform;
        }
        if (crosshairIsActive)
        {
            if (!crosshairCreated) // flag to create only one crosshair
            {
                crosshairInstance = Instantiate(crosshairPrefab);
                crosshairCreated = true;
            }
            UpdateAimPosition();
        }
        else if (!crosshairIsActive && crosshairInstance != null)
        {
            Destroy(crosshairInstance);
            crosshairCreated= false;
        }
    }

    private void UpdateAimPosition()
    {
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool placedCrosshair = false;

        if (Physics.Raycast(ray, out hit, 100, floorLayer))
        {
            Vector3 normal = hit.normal;
            float angle = Vector3.Dot(normal, Vector3.up);
            if (angle >= 0.8f)
            {
                Vector3 centerOnGround = new Vector3(throwOrigin.position.x, hit.point.y, throwOrigin.position.z);
                Vector3 offset = hit.point - centerOnGround;
                float dist = offset.magnitude;
                if (dist > maxThrowDistance)
                {
                    offset = offset.normalized * maxThrowDistance;
                }
                crosshairInstance.transform.position = centerOnGround + offset;
                crosshairInstance.transform.position += Vector3.up * 0.01f;
                crosshairInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                placedCrosshair = true;
                crosshairTransform = crosshairInstance.transform;
            }
        }
    }
}

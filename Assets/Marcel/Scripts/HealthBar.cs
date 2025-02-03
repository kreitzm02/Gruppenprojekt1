using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool isPlayerHealthBar = false;

    private void Start()
    {
        slider = GetComponent<Slider>(); 
    }
    void Update()
    {
        if (!isPlayerHealthBar)
        {
            cam = Camera.main;
            slider.transform.rotation = cam.transform.rotation;
            slider.transform.position = target.position + offset;
        }
    }

    public void UpdateHealthBar(float _current, float _max)
    {
        slider.value = _current / _max;
    }
}

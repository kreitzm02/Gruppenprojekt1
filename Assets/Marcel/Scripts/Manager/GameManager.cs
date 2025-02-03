using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera playerCam;
    [SerializeField] private Camera menuCam;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject blurEffect;

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

    private void Start()
    {
        SetPlayerCamActive();
        SetPlayerActive(true);
        ActivateBlur(false);
    }

    public void SetPlayerCamActive()
    {
        playerCam.gameObject.SetActive(true);
        menuCam.gameObject.SetActive(false);
    }

    public void SetMenuCamActive()
    {
        playerCam.gameObject.SetActive(false);
        menuCam.gameObject.SetActive(true);
    }

    public void SetPlayerActive(bool _setActive)
    {
        if (_setActive)
            player.SetActive(true);
        else player.SetActive(false);
    }


    public void ActivateBlur(bool _setActive)
    {
        if (_setActive)
            blurEffect.SetActive(true);
        else blurEffect.SetActive(false);
    }
}

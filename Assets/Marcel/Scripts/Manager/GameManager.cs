using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera playerCam;
    [SerializeField] private Camera menuCam;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject blurEffect;
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private bool dungeonGenTest = false;
    [SerializeField] private Canvas loadingScreen;
    [SerializeField] private Slider loadingSlider;

    private bool dungeonIsGenerating = false;

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

    private void Update()
    {
        if (dungeonGenTest)
            StartDungeonGeneration();
        if (dungeonIsGenerating)
            UpdateLoadingScreenDungeonGen();
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

    public void SetLoadingScreenActive(bool _setActive)
    {
        if (_setActive)
            loadingScreen.gameObject.SetActive(true);
        else loadingScreen.gameObject.SetActive(false);
    }

    public void SetCreditsScreenActive(bool _setActive)
    {

    }

    public void SetMainMenuScreenActive(bool _setActive)
    {

    }

    public void SetPauseScreenActive(bool _setActive)
    {

    }

    public void SetOptionsScreenActive(bool _setActive)
    {

    }

    public void StartDungeonGeneration()
    {
        dungeonGenerator.GenerateDungeon();
        dungeonIsGenerating = true;
        dungeonGenTest = false;
    }

    public void UpdateLoadingScreenDungeonGen()
    {
        loadingSlider.value = dungeonGenerator.progressInPercent;
        if (dungeonGenerator.progressInPercent == 100)
        {
            dungeonIsGenerating = false;
            SetLoadingScreenActive(false);
            ActivateBlur(false);
            SetPlayerActive(true);
            SetPlayerCamActive();
        }
    }
}


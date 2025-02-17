using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;
    public bool showHealthBar = true;
    public bool showHealthText = true;
    public bool showInteractText = false;
    public bool showFpsText = true;
    private int interactableCount = 0;

    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI fpsText;

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

    private void OnEnable()
    {
        if (fpsText != null)
        {
            StartCoroutine(RecalcFPS());
        }
    }

    private void Update()
    {
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(showFpsText);
        }
    }

    IEnumerator RecalcFPS ()
    {
        while (true)  
        {
            fpsText.text = ((int)(1.0f / Time.deltaTime)).ToString() + " FPS";
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void RegisterInteractable()
    {
        interactableCount++;
        UpdateInteractText();
    }

    public void UnregisterInteractable()
    {
        interactableCount--;
        if (interactableCount < 0)
            interactableCount = 0;

        UpdateInteractText();
    }

    private void UpdateInteractText()
    {
        if (interactText == null) return;
        interactText.gameObject.SetActive(interactableCount > 0);
    }
}

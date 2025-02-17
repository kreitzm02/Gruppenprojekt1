using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntranceBehaviour : MonoBehaviour
{
    private bool interactionPossible;
    private bool hasInteracted;
    private bool unregisterPossible = false;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] cols = Physics.OverlapSphere(this.transform.position, 2f);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                interactionPossible = true;
                unregisterPossible = true;
                PlayerUI.Instance.RegisterInteractable();
                playerTransform = col.gameObject.transform;
                break;
            }
            else
            {
                interactionPossible = false;
                if (unregisterPossible) PlayerUI.Instance.UnregisterInteractable();
            }
        }
        if (interactionPossible && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.ActivateBlur(true);
            GameManager.Instance.SetMenuCamActive();
            GameManager.Instance.SetPlayerActive(false);
            GameManager.Instance.SetLoadingScreenActive(true);
            GameManager.Instance.StartDungeonGeneration();
            hasInteracted = true;
        }
    }
}

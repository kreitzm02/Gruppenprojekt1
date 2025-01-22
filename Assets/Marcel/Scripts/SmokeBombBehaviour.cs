using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SmokeBombBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject smokeEffectPrefab;
    private VisualEffect smokeEffect;
    private float fuseTime = 1f;
    private float smokeDuration = 0.01f;
    private bool hasLanded = false;
    private Vector3 impactPosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && smokeEffectPrefab != null && !collision.gameObject.CompareTag("Player"))
        {
            hasLanded = true;
            GameObject newSmokeObject = Instantiate(smokeEffectPrefab, this.transform.position, Quaternion.identity);
            smokeEffect = newSmokeObject.GetComponent<VisualEffect>();
            StartCoroutine(StartSmoke());
            Rigidbody rigidb = GetComponent<Rigidbody>();
            rigidb.velocity = Vector3.zero;
        }
    }
    private System.Collections.IEnumerator StartSmoke()
    {
        yield return new WaitForSeconds(fuseTime);
        if (smokeEffect != null)
        {
            smokeEffect.Play();
        }
        yield return new WaitForSeconds(smokeDuration);
        smokeEffect.Stop();
        Destroy(gameObject);
    }
}

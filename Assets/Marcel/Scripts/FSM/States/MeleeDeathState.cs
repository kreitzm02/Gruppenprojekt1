using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDeathState : BaseState
{
    // DEATH STATE
    //
    // Intented behaviour: This state gets called if the object is supposed to "Die". A death animation will play
    //                     and after that, the object will fade away and will be destroyed.
    // Intended condition: The death state should not have an exit condition, as the object will get destroyed.

    private string animName;
    private float currentTime;
    private float maxDeathTime = 2f;
    private float fadeDuration = 1f;
    private float fadeTimer;
    private List<Material> materials = new();
    private Renderer[] renderers;
    private Color color;
    private float alphaValue;

    public MeleeDeathState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        sm.animator.CrossFade(animName, 0.1f);
        currentTime = 0;
        fadeTimer = 0;
        renderers = sm.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                materials.Add(mat);
            }
        }
        Debug.Log("Entering Death State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Leaving Death State");
        // should never be called
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating Death State");
        currentTime += Time.deltaTime;
        if (currentTime >= maxDeathTime * 0.5)
        {
            fadeTimer += Time.deltaTime;
            alphaValue = Mathf.Lerp(1, 0, fadeTimer / fadeDuration);
            foreach (Material mat in materials)
            {
                Color color = mat.color;
                mat.color = new Color(color.r, color.g, color.b, alphaValue);
            }
        }
        if (currentTime >= maxDeathTime)
        {
            Object.Destroy(sm.gameObject);
            Debug.Log("Death");
        }
    }
}

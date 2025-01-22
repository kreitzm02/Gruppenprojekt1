using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public int maxLights = 4; // Maximale Anzahl der unterstützten Lichtquellen

    void Update()
    {
        Light[] lights = FindObjectsOfType<Light>();
        int lightCount = Mathf.Min(lights.Length, maxLights);

        // Reset global light count
        Shader.SetGlobalInt("_LightCount", lightCount);

        for (int i = 0; i < lightCount; i++)
        {
            Light light = lights[i];
            Shader.SetGlobalVector($"_LightPosition{i}", light.transform.position);
            Shader.SetGlobalVector($"_LightDirection{i}", light.transform.forward);
            Shader.SetGlobalColor($"_LightColor{i}", light.color * light.intensity);
            Shader.SetGlobalFloat($"_LightIntensity{i}", light.intensity);
            Shader.SetGlobalFloat($"_LightType{i}", (float)light.type);
            Shader.SetGlobalFloat($"_LightRange{i}", light.range);
            Shader.SetGlobalFloat($"_LightSpotAngle{i}", light.spotAngle);
            Shader.SetGlobalFloat($"_LightSpotFalloff{i}", light.spotAngle); // Optional: Anpassen, falls erforderlich
        }

        // Deaktiviere überschüssige Lichter
        for (int i = lightCount; i < maxLights; i++)
        {
            Shader.SetGlobalVector($"_LightPosition{i}", Vector3.zero);
            Shader.SetGlobalVector($"_LightDirection{i}", Vector3.forward);
            Shader.SetGlobalColor($"_LightColor{i}", Color.black);
            Shader.SetGlobalFloat($"_LightIntensity{i}", 0.0f);
            Shader.SetGlobalFloat($"_LightType{i}", 0.0f);
            Shader.SetGlobalFloat($"_LightRange{i}", 0.0f);
            Shader.SetGlobalFloat($"_LightSpotAngle{i}", 0.0f);
            Shader.SetGlobalFloat($"_LightSpotFalloff{i}", 0.0f);
        }
    }
}

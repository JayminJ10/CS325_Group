using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public Light playerLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 5f;

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
        playerLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}

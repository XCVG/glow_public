using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowstickHitEffectScript : MonoBehaviour
{
    [SerializeField]
    private Light Light;
    [SerializeField]
    private ParticleSystem ParticleSystem;

    public void SetHue(float hue)
    {
        //ParticleSystem.startColor =

        {
            var gsColor = ParticleSystem.startColor;
            Color.RGBToHSV(gsColor, out float h, out float s, out float v);
            var gsNewColor = Color.HSVToRGB(hue, s, v);
            ParticleSystem.startColor = new Color(gsNewColor.r, gsNewColor.g, gsNewColor.b, gsColor.a);
        }

        //glowstick light
        {
            var lColor = Light.color;
            Color.RGBToHSV(lColor, out float h, out float s, out float v);
            var lNewColor = Color.HSVToRGB(hue, s, v);
            Light.color = lNewColor;
        }
    }
}

using CommonCore.ObjectActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowstickRandomizer : MonoBehaviour
{
    [SerializeField]
    private Renderer GlowstickRenderer;
    [SerializeField]
    private Light GlowstickLight;
    [SerializeField]
    private GenericRotateScript GenericRotateScript;

    public float Hue;

    private void Start()
    {
        float hue = UnityEngine.Random.Range(0f, 1f);
        Hue = hue;

        //glowstick color
        {
            var gsColor = GlowstickRenderer.material.color;
            Color.RGBToHSV(gsColor, out float h, out float s, out float v);
            var gsNewColor = Color.HSVToRGB(hue, s, v);
            GlowstickRenderer.material.color = new Color(gsNewColor.r, gsNewColor.g, gsNewColor.b, gsColor.a);
        }

        //glowstick emissive
        {
            var gsColor = GlowstickRenderer.material.GetColor("_EmissionColor");
            Color.RGBToHSV(gsColor, out float h, out float s, out float v);
            var gsNewColor = Color.HSVToRGB(hue, s, v);
            GlowstickRenderer.material.SetColor("_EmissionColor", new Color(gsNewColor.r, gsNewColor.g, gsNewColor.b, gsColor.a));
        }

        //glowstick light
        {
            var lColor = GlowstickLight.color;
            Color.RGBToHSV(lColor, out float h, out float s, out float v);
            var lNewColor = Color.HSVToRGB(hue, s, v);
            GlowstickLight.color = lNewColor;
        }

        //rotation
        GenericRotateScript.RotationAxis = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
    }
}

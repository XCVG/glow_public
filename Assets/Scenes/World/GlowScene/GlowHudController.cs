using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlowHudController : MonoBehaviour
{
    public Slider TimeSlider;
    public Text ScoreText;

    private GlowSceneController GlowSceneController;

    private void Start()
    {
        GlowSceneController = CoreUtils.GetWorldRoot().GetComponent<GlowSceneController>();

    }

    private void LateUpdate()
    {
        ScoreText.text = $"Score: {Mathf.RoundToInt(GlowSceneController.Score)}";
        //TimeSlider.value = Mathf.Clamp01(1- ((GlowSceneController.StartingTime - GlowSceneController.TimeLeft) / GlowSceneController.TimeLeft)); //no I don't know
        TimeSlider.value = MathUtils.ScaleRange(GlowSceneController.TimeLeft, 0, GlowSceneController.StartingTime, 0f, 1f);
    }
}

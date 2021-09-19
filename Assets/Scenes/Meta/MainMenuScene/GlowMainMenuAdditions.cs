using CommonCore;
using CommonCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowMainMenuAdditions : MonoBehaviour
{
    public void HandleStartGameClicked()
    {
        CoreUtils.GetUIRoot().GetComponentInChildren<DifficultyPanelController>(true).Show(() =>
        {
            SharedUtils.StartGame("IntroScene");
        });
        
    }

    public void HandleSkipIntroClicked()
    {
        CoreUtils.GetUIRoot().GetComponentInChildren<DifficultyPanelController>(true).Show(() =>
        {
            SharedUtils.StartGame();
        });
    }

    public void HandleShowScoreClicked()
    {
        Debug.LogError("NOT IMPLEMENTED");
    }
}

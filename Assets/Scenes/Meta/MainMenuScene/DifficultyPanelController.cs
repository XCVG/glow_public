using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonCore;
using CommonCore.Config;
using CommonCore.RpgGame;
using CommonCore.UI;

public class DifficultyPanelController : MonoBehaviour
{
    public Slider DifficultySlider;

    public Action Callback;

    private void Start()
    {
        if (CoreParams.UIThemeMode == UIThemePolicy.Auto)
            CCBase.GetModule<UIModule>().ApplyThemeRecurse(transform.GetChild(0));
    }

    private void OnEnable()
    {
        //paint difficulty
        DifficultySlider.value = (int)ConfigState.Instance.GetGameplayConfig().DifficultySetting;
    }

    public void HandleConfirmButtonClicked()
    {
        //set difficulty
        ConfigState.Instance.GetGameplayConfig().DifficultySetting = (DifficultyLevel)(int)DifficultySlider.value;
        ConfigState.Save();

        //disable
        gameObject.SetActive(false);

        //return
        Callback?.Invoke();

        Callback = null;
    }

    public void HandleCancelButtonClicked()
    {
        //disable
        gameObject.SetActive(false);
    }

    public void Show(Action callback)
    {
        Callback = callback;
        gameObject.SetActive(true);
    }
}

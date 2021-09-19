using CommonCore;
using CommonCore.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.State;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HighScorePanelController : MonoBehaviour
{
    [SerializeField]
    private RectTransform HighScoresContainer;
    [SerializeField]
    private RectTransform HighScoreTemplate;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);

        //clear scores
        for(int i = HighScoresContainer.childCount - 1; i >= 0; i--)
        {
            var t = HighScoresContainer.GetChild(i);
            if (t.gameObject != HighScoreTemplate.gameObject)
                Destroy(t.gameObject);
        }

        //paint scores
        var scores = PersistState.Instance.HighScores;
        var sScores = scores
            .OrderByDescending(s => s.Score)
            .Take(Math.Min(scores.Count, 10));

        foreach(var score in sScores)
        {
            var so = GameObject.Instantiate(HighScoreTemplate.gameObject, HighScoresContainer);

            so.transform.FindDeepChild("Name").GetComponent<Text>().text = score.Name;
            so.transform.FindDeepChild("Score").GetComponent<Text>().text = Mathf.RoundToInt(score.Score).ToString();
            so.SetActive(true);
        }

        HighScoreTemplate.gameObject.SetActive(false);

        //theme
        if (CoreParams.UIThemeMode == UIThemePolicy.Auto)
            CCBase.GetModule<UIModule>().ApplyThemeRecurse(transform.GetChild(0));
    }
}

using CommonCore;
using CommonCore.State;
using CommonCore.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlowGameOverAdditions : MonoBehaviour
{
    [SerializeField]
    private Text ScoreText = null;
    [SerializeField]
    private InputField NameText = null;

    private DateTimeOffset NowIsh;

    private void Start()
    {
        var score = Mathf.RoundToInt(GameState.Instance.CampaignState.GetVar<float>("score"));
        ScoreText.text = $"Your score: {score}";

        NameText.text = MetaState.Instance.SessionData.GetOrDefault("Name", string.Empty) as string;

        NowIsh = DateTimeOffset.Now;
    }

    public void HandleReturnButtonClicked() //mislabeled slightly
    {
        // grab name from text box
        var name = NameText.text;
        MetaState.Instance.SessionData["Name"] = name;
        

        var scoreNode = new HighScoreNode() { Name = name, DateTime = NowIsh, Score = GameState.Instance.CampaignState.GetVar<float>("score"), Time = GameState.Instance.CampaignState.GetVar<float>("time") };
        PersistState.Instance.HighScores.Add(scoreNode);
        PersistState.Save();

        GetComponent<GameOverMenuController>().HandleExitButtonClicked();
    }
}

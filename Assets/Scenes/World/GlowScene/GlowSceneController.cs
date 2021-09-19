using CommonCore;
using CommonCore.Audio;
using CommonCore.Config;
using CommonCore.LockPause;
using CommonCore.RpgGame;
using CommonCore.State;
using CommonCore.Experimental;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowSceneController : MonoBehaviour
{

    public GlowSceneState State;
    public bool HandleEndGame = true;

    [Header("Segment handling")]
    public GameObject SegmentTemplate;
    public GameObject CurrentSegment;
    public GameObject PreviousSegment;
    public float SegmentMoveSpeedBase = 5f;
    public float SegmentMoveSpeedLinear = 1f;
    public float SegmentMoveSpeedExpo = 0.5f;
    public float SegmentReplaceThreshold = -70f;
    public int BuildingKeepMin = 5;
    public int BuildingKeepMax = 8;
    public int ObstacleKeepMin = 2;
    public int ObstacleKeepMax = 4;

    [Header("Timeout handling")]
    public float StartingTime = 60.0f;
    public float TimeLossRate = 1f;
    public float TimeLossFromMovementSpeed = 0.1f;
    public float TimeGainFromHit = 10f;
    public float TimeLostFromCollision = 10f;

    [Header("Scoring")]
    public float ScoreFromHit = 100f;
    public float ScoreFromTime = 1f;
    public float ScoreLostFromCollision = 100f;

    [Header("public because jam")]
    public float SegmentCurrentMoveSpeed;
    public int NumSegments = 0;
    public float TimeLeft;
    public float ScoreDebug;
    public float DifficultySpeedFactor = 1f; //TODO get this from somewhere
    public float DifficultyScoreFactor = 1f;

    private void Start()
    {
        TimeLeft = StartingTime;

        var difficulty = ConfigState.Instance.GetGameplayConfig().Difficulty;
        DifficultySpeedFactor = difficulty.EnvironmentLevelBias * ConfigState.Instance.GameSpeed;

        DifficultyScoreFactor = difficulty.EnvironmentLevelBias * ConfigState.Instance.GameSpeed * difficulty.EnvironmentEnemyFrequency * (1f / difficulty.EnvironmentLootFrequency);

        SetMusic();

        SpawnSegment();

        SegmentTemplate.SetActive(false);        
        UpdateSegmentMoveSpeed();

        StartCoroutine(CoStartSequence());
    }

    private void FixedUpdate()
    {
        if (LockPauseModule.IsPaused())
            return;

        if (State == GlowSceneState.Running)
        {
            //move segments
            if (PreviousSegment != null)
            {
                PreviousSegment.transform.Translate(Vector3.back * SegmentCurrentMoveSpeed * Time.fixedDeltaTime, Space.World);
            }

            
            if (CurrentSegment != null)
            {
                CurrentSegment.transform.Translate(Vector3.back * SegmentCurrentMoveSpeed * Time.fixedDeltaTime, Space.World);

                //handle spawn segment

                if(CurrentSegment.transform.position.z < SegmentReplaceThreshold)
                {
                    SpawnSegment();
                    UpdateSegmentMoveSpeed();
                }
            }


        }
    }

    private void Update()
    {
        if (LockPauseModule.IsPaused())
            return;

        if(State == GlowSceneState.Running)
        {
            //WIP timeout
            TimeLeft -= Time.deltaTime * TimeLossRate + (TimeLossFromMovementSpeed * SegmentCurrentMoveSpeed);

            //WIP scoring
            Score += Time.deltaTime * ScoreFromTime * (SegmentCurrentMoveSpeed / SegmentMoveSpeedBase) * DifficultyScoreFactor;

            if(HandleEndGame)
            {
                if(TimeLeft <= 0)
                {
                    State = GlowSceneState.Ended;
                    StartCoroutine(CoEndSequence());
                }
            }
            
        }

        ScoreDebug = Score;
    }

    private IEnumerator CoStartSequence()
    {
        State = GlowSceneState.Started;
        var slideshow = SlideshowControllerEx.GetInstance();
        var audioPlayer = CCBase.GetModule<AudioModule>().AudioPlayer;

        /*
        Subtitle.Show("3", 5f);
        yield return new WaitForSeconds(1f);
        Subtitle.Show("2", 5f);
        yield return new WaitForSeconds(1f);
        Subtitle.Show("1", 5f);
        yield return new WaitForSeconds(1f);
        Subtitle.Show("GO!", 1f);
        */

        slideshow.ShowImage("ready");
        audioPlayer.PlaySound("ready1", SoundType.Voice, false);
        yield return new WaitForSeconds(1.5f);

        slideshow.ShowImage("go");
        audioPlayer.PlaySound("ready2", SoundType.Voice, false);
        yield return new WaitForSeconds(1.5f);

        yield return null;

        SlideshowControllerEx.ClearInstance();

        //SetMusic();
        MusicFader.FadeTo(MusicSlot.Ambient, 0.75f, 1f);

        //ugly place to put this but oh well
        CoreUtils.GetUIRoot().GetComponentInChildren<GlowCursorController>(true).gameObject.SetActive(true);

        State = GlowSceneState.Running;
    }

    private IEnumerator CoEndSequence()
    {
        //ugly place to put this but oh well
        CoreUtils.GetUIRoot().GetComponentInChildren<GlowCursorController>(true).gameObject.SetActive(false);

        yield return null;

        MusicFader.FadeTo(MusicSlot.Ambient, 0.25f, 0.5f);
        yield return new WaitForSeconds(0.25f);

        State = GlowSceneState.Started;
        var slideshow = SlideshowControllerEx.GetInstance();
        var audioPlayer = CCBase.GetModule<AudioModule>().AudioPlayer;

        slideshow.ShowImage("end");
        audioPlayer.PlaySound("end1", SoundType.Voice, false);
        yield return new WaitForSeconds(1.5f);

        yield return null;

        SlideshowControllerEx.ClearInstance();
        SharedUtils.ShowGameOver();
    }

    private void SpawnSegment()
    {
        Debug.Log("Spawn segment!");

        DifficultyValues difficulty = ConfigState.Instance.GetGameplayConfig().Difficulty;

        bool translate = (CurrentSegment != null);
        if (PreviousSegment != null)
        {            
            Destroy(PreviousSegment);
        }

        PreviousSegment = CurrentSegment;

        CurrentSegment = GameObject.Instantiate(SegmentTemplate, CoreUtils.GetWorldRoot());
        if (translate)
        {
            CurrentSegment.transform.Translate(Vector3.forward * (100f + PreviousSegment.transform.position.z), Space.World); //woo fixed!
        }
        CurrentSegment.SetActive(true);

        //building handling
        {
            var buildingsRoot = CurrentSegment.transform.FindDeepChild("DynamicBuildings");
            int buildingsToKeep = UnityEngine.Random.Range(BuildingKeepMin, BuildingKeepMax + 1);
            
            buildingsToKeep = MathUtils.Clamp(Mathf.RoundToInt((float)buildingsToKeep * (float)difficulty.EnvironmentLootFrequency), BuildingKeepMin, BuildingKeepMax);

            int buildings = buildingsRoot.childCount;
            buildingsToKeep = Math.Min(buildingsToKeep, buildings);
            int buildingsToRemove = buildings - buildingsToKeep;

            for(int i = 0; i < buildingsToRemove; i++)
            {
                int buildingToRemove = UnityEngine.Random.Range(0, buildingsRoot.childCount);
                Transform b = buildingsRoot.GetChild(buildingToRemove);
                b.gameObject.SetActive(false);
                b.parent = CoreUtils.GetWorldRoot();
                Destroy(b.gameObject);
            }
        }

        //obstacle handling
        {
            var obstaclesRoot = CurrentSegment.transform.FindDeepChild("DynamicObstacles");

            int obstaclesToKeep = UnityEngine.Random.Range(ObstacleKeepMin, ObstacleKeepMax + 1);

            obstaclesToKeep = Mathf.RoundToInt((float)obstaclesToKeep * (float)difficulty.EnvironmentEnemyFrequency); //unlike buildings we don't clamp this

            foreach(Transform t in obstaclesRoot)
            {
                t.gameObject.SetActive(false);
            }

            for (int i = 0; i < obstaclesToKeep; i++)
            {
                int obstacleToActivate = UnityEngine.Random.Range(0, obstaclesRoot.childCount);
                for(int j = obstacleToActivate; ; j++)
                {
                    var obstacle = obstaclesRoot.GetChild(j);
                    if(!obstacle.gameObject.activeSelf)
                    {
                        obstacle.gameObject.SetActive(true);
                        break;
                    }

                    if (j >= obstaclesRoot.childCount - 1) //possible off by one
                        j = 0;
                }
            }
        }


        NumSegments++;
    }

    private void SetMusic()
    {
        string music = "level" + UnityEngine.Random.Range(1, 4);
        CCBase.GetModule<AudioModule>().AudioPlayer.PlayMusic(music, MusicSlot.Ambient, 0.25f, true, false);
    }

    public float Score
    {
        get
        {
            return GameState.Instance.CampaignState.GetVar<float>("score");
        }
        set
        {
            GameState.Instance.CampaignState.SetVar("score", value);
        }
    }

    public void HandleTargetHit()
    {
        Score += ScoreFromHit * DifficultyScoreFactor;

        TimeLeft += TimeGainFromHit;
        TimeLeft = Mathf.Min(TimeLeft, StartingTime);

    }

    public void HandleObstacleHit()
    {
        Score = Mathf.Max(0, Score - ScoreLostFromCollision);

        TimeLeft -= TimeLostFromCollision;
    }

    private void UpdateSegmentMoveSpeed()
    {
        SegmentCurrentMoveSpeed = (SegmentMoveSpeedBase + (NumSegments * SegmentMoveSpeedLinear) + (NumSegments * NumSegments * SegmentMoveSpeedExpo)) * DifficultySpeedFactor;
    }

    public enum GlowSceneState
    {
        Started, Running, Ended
    }

}



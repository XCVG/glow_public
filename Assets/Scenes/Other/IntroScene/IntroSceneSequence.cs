using CommonCore;
using CommonCore.Audio;
using CommonCore.LockPause;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneSequence : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CoIntroSequence());
    }

    private IEnumerator CoIntroSequence()
    {
        var audioPlayer = CCBase.GetModule<AudioModule>().AudioPlayer;
        var slideshow = SlideshowControllerEx.GetInstance();

        audioPlayer.PlayMusic("intro", MusicSlot.Ambient, 1.0f, true, false);

        slideshow.ShowImage("intro1");
        ScreenFader.FadeFrom(Color.black, 1f, false);

        yield return new WaitForSeconds(1f);
        yield return null;

        yield return new WaitForSeconds(5f);
        yield return new WaitForSecondsEx(10f, true, PauseLockType.AllowCutscene, false);

        slideshow.ShowImage("intro2");
        yield return new WaitForSeconds(5f);
        yield return new WaitForSecondsEx(10f, true, PauseLockType.AllowCutscene, false);

        slideshow.ShowImage("instructions");
        yield return new WaitForSeconds(10f);
        yield return new WaitForSecondsEx(20f, true, PauseLockType.AllowCutscene, false);

        ScreenFader.FadeTo(Color.black, 1.0f, false);
        SharedUtils.ChangeScene(CoreParams.InitialScene);
    }
}

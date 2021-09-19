using CommonCore;
using CommonCore.Scripting;
using CommonCore.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GlowScripts
{
    [CCScript, CCScriptHook(Hook = ScriptHook.AfterAddonsLoaded)]
    private static void OnGameInitialize()
    {
        MetaState.Instance.SessionData["Name"] = Environment.UserName;
    }

    [CCScript, CCScriptHook(Hook = ScriptHook.OnGameStart)]
    private static void OnGameStart()
    {
        //TODO setup ALL the state
        GameState.Instance.SaveLocked = true;
    }

}
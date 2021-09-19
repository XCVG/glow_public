using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.State
{
    public partial class PersistState
    {

        [JsonProperty]
        public List<HighScoreNode> HighScores { get; private set; } = new List<HighScoreNode>();
    }

    public class HighScoreNode
    {
        public string Name;
        public float Score;
        public float Time;
        public DateTimeOffset DateTime;
    }

}
